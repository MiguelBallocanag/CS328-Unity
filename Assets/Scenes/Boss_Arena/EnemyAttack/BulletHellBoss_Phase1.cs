using UnityEngine;
using System.Collections;

public class BulletHellBoss_Phase1 : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform attackPoint;
    public Animator animator;
    public LayerMask groundLayer;
    
    [Header("Projectile Settings")]
    public GameObject orbPrefab;
    public float orbSpeed = 5f;
    
    [Header("Light Beam Settings")]
    public GameObject lightBeamPrefab;
    public float beamWarningTime = 1f;
    public float beamDamageTime = 2f;
    
    [Header("Melee Attack")]
    public float meleeRange = 3f;
    public int meleeDamage = 20;
    public float meleeKnockback = 8f;
    
    [Header("Attack Timing")]
    public float timeBetweenAttacks = 2f;
    
    [Header("Pattern 1: Circular Burst")]
    public int burstOrbCount = 8;
    
    [Header("Pattern 2: Aimed Shot")]
    public int aimedShotCount = 3;
    public float aimedShotSpread = 15f;
    
    [Header("Pattern 3: Spiral")]
    public float spiralDuration = 3f;
    public float spiralOrbInterval = 0.1f;
    public float spiralRotationSpeed = 60f;
    
    [Header("Pattern 4: Light Beams")]
    public int beamCount = 3;
    public float beamSpacing = 3f;
    
    [Header("Movement Settings")]
    public bool enableTeleport = true;
    public bool enableGradualMovement = true;
    public float walkSpeed = 2f;
    public float walkChance = 0.4f; // 40% chance to walk instead of teleport
    public float walkDuration = 1.5f;
    
    [Header("Teleport Settings")]
    public float minDistanceFromPlayer = 8f;
    public float maxDistanceFromPlayer = 15f;
    public float teleportCooldown = 5f;
    public float edgeBuffer = 3f;
    public GameObject teleportEffectPrefab;
    public float teleportFlashDuration = 0.3f;
    
    [Header("Ground Check")]
    public float groundCheckDistance = 2f;
    public float minHeightAboveGround = 1f;
    
    private int currentPatternIndex = 0;
    private float lastTeleportTime = -999f;
    private bool isStunned = false;
    private Rigidbody2D rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
        
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null) animator = GetComponentInChildren<Animator>();
        }
        
        if (groundLayer == 0)
        {
            groundLayer = LayerMask.GetMask("Ground");
        }
    }
    
    public void StartPhase1()
    {
        Debug.Log("[Phase1] Phase 1 activated!");
        if (animator != null) animator.SetBool("IsWalking", false);
        StartCoroutine(AttackPattern());
    }
    
    public void Stun(float duration)
    {
        StartCoroutine(StunSequence(duration));
    }
    
    IEnumerator StunSequence(float duration)
    {
        isStunned = true;
        if (animator != null) animator.SetBool("IsWalking", false);
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }
    
    IEnumerator AttackPattern()
    {
        while (true)
        {
            if (!isStunned)
            {
                if (animator != null) animator.SetBool("IsWalking", false);
                yield return new WaitForSeconds(timeBetweenAttacks);
                
                if (isStunned) continue;
                
                // Check for melee range FIRST
                float distanceToPlayer = Vector2.Distance(transform.position, player.position);
                if (distanceToPlayer <= meleeRange)
                {
                    yield return StartCoroutine(PerformMeleeAttack());
                }
                else
                {
                    // Check if we should move (walk or teleport)
                    if (enableGradualMovement && Random.value < walkChance)
                    {
                        yield return StartCoroutine(WalkTowardsPlayer());
                    }
                    else if (enableTeleport)
                    {
                        CheckAndTeleport();
                    }
                    
                    // Perform ranged attack pattern
                    switch (currentPatternIndex)
                    {
                        case 0: yield return StartCoroutine(PerformCircularBurst()); break;
                        case 1: yield return StartCoroutine(PerformAimedShot()); break;
                        case 2: yield return StartCoroutine(PerformSpiral()); break;
                        case 3: yield return StartCoroutine(PerformLightBeams()); break;
                    }
                    
                    currentPatternIndex = (currentPatternIndex + 1) % 4;
                }
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    
    IEnumerator PerformMeleeAttack()
    {
        Debug.Log("[Phase1] Performing melee attack!");
        if (animator != null) animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.3f);
        
        // Check if player is still in range
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance <= meleeRange)
            {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(meleeDamage);
                    
                    // Apply knockback
                    Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
                    if (playerRb != null)
                    {
                        Vector2 knockbackDir = (player.position - transform.position).normalized;
                        playerRb.linearVelocity = knockbackDir * meleeKnockback;
                    }
                }
            }
        }
    }
    
    IEnumerator WalkTowardsPlayer()
    {
        if (player == null) yield break;
        
        Debug.Log("[Phase1] Walking towards player!");
        if (animator != null) animator.SetBool("IsWalking", true);
        
        float elapsed = 0f;
        while (elapsed < walkDuration && !isStunned)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(direction.x * walkSpeed, rb.linearVelocity.y);
            }
            else
            {
                transform.position += (Vector3)(direction * walkSpeed * Time.deltaTime);
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (animator != null) animator.SetBool("IsWalking", false);
    }
    
    IEnumerator PerformCircularBurst()
    {
        if (isStunned) yield break;
        
        if (animator != null) animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.2f);
        
        for (int i = 0; i < burstOrbCount; i++)
        {
            float angle = i * (360f / burstOrbCount);
            SpawnOrb(angle);
        }
    }
    
    IEnumerator PerformAimedShot()
    {
        if (isStunned || player == null) yield break;
        
        if (animator != null) animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.2f);
        
        Vector2 directionToPlayer = (player.position - attackPoint.position).normalized;
        float baseAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        
        for (int i = 0; i < aimedShotCount; i++)
        {
            float offset = (i - (aimedShotCount - 1) / 2f) * aimedShotSpread;
            SpawnOrb(baseAngle + offset);
        }
    }
    
    IEnumerator PerformSpiral()
    {
        if (isStunned) yield break;
        
        if (animator != null) animator.SetTrigger("Attack");
        
        float elapsed = 0f;
        float currentAngle = 0f;
        
        while (elapsed < spiralDuration && !isStunned)
        {
            SpawnOrb(currentAngle);
            currentAngle += spiralRotationSpeed * spiralOrbInterval;
            elapsed += spiralOrbInterval;
            yield return new WaitForSeconds(spiralOrbInterval);
        }
    }
    
    IEnumerator PerformLightBeams()
    {
        if (isStunned) yield break;
        
        if (animator != null) animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.2f);
        
        if (lightBeamPrefab != null && attackPoint != null)
        {
            float startX = attackPoint.position.x - ((beamCount - 1) * beamSpacing / 2f);
            for (int i = 0; i < beamCount; i++)
            {
                float xPos = startX + (i * beamSpacing);
                GameObject beam = Instantiate(lightBeamPrefab, new Vector3(xPos, attackPoint.position.y, 0), Quaternion.identity);
                
                LightBeam beamScript = beam.GetComponent<LightBeam>();
                if (beamScript != null)
                {
                    beamScript.Initialize(beamWarningTime, beamDamageTime);
                }
            }
        }
    }
    
    void SpawnOrb(float angleDegrees)
    {
        if (orbPrefab == null || attackPoint == null) return;
        
        // Spawn slightly away from boss to prevent collision issues
        float spawnOffset = 0.5f;
        float angleRad = angleDegrees * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        Vector3 spawnPos = attackPoint.position + (Vector3)(direction * spawnOffset);
        
        GameObject orb = Instantiate(orbPrefab, spawnPos, Quaternion.identity);
        
        Rigidbody2D rb = orb.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * orbSpeed;
            Debug.Log($"[Phase1] Spawned orb at {spawnPos} with velocity {direction * orbSpeed}");
        }
    }
    
    void CheckAndTeleport()
    {
        if (Time.time - lastTeleportTime < teleportCooldown) return;
        if (player == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if (distanceToPlayer < minDistanceFromPlayer)
        {
            Debug.Log("[Phase1] Too close, teleporting away!");
            StartCoroutine(TeleportSequence());
        }
        else if (distanceToPlayer > maxDistanceFromPlayer)
        {
            Debug.Log("[Phase1] Too far, teleporting closer!");
            StartCoroutine(TeleportSequence());
        }
        else if (IsNearScreenEdge())
        {
            Debug.Log("[Phase1] Near edge, teleporting!");
            StartCoroutine(TeleportSequence());
        }
    }
    
    bool IsNearScreenEdge()
    {
        Camera cam = Camera.main;
        if (cam == null) return false;
        
        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);
        return viewportPos.x < 0.15f || viewportPos.x > 0.85f ||
               viewportPos.y < 0.15f || viewportPos.y > 0.85f;
    }
    
    IEnumerator TeleportSequence()
    {
        lastTeleportTime = Time.time;
        
        if (animator != null) animator.SetBool("IsWalking", true);
        
        if (teleportEffectPrefab != null)
        {
            Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);
        }
        
        yield return new WaitForSeconds(teleportFlashDuration);
        
        Vector3 newPos = CalculateTeleportPosition();
        transform.position = newPos;
        
        if (teleportEffectPrefab != null)
        {
            Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);
        }
        
        yield return new WaitForSeconds(teleportFlashDuration);
        
        if (animator != null) animator.SetBool("IsWalking", false);
    }
    
    Vector3 CalculateTeleportPosition()
    {
        Camera cam = Camera.main;
        if (cam == null || player == null) return transform.position;
        
        for (int attempt = 0; attempt < 20; attempt++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-10f, 10f),
                Random.Range(-5f, 5f),
                0
            );
            
            Vector3 candidatePos = player.position + randomOffset;
            float distance = Vector2.Distance(candidatePos, player.position);
            
            // Check distance requirements
            if (distance >= minDistanceFromPlayer && distance <= maxDistanceFromPlayer)
            {
                // Check viewport bounds
                Vector3 viewportPos = cam.WorldToViewportPoint(candidatePos);
                if (viewportPos.x > 0.2f && viewportPos.x < 0.8f &&
                    viewportPos.y > 0.2f && viewportPos.y < 0.8f)
                {
                    // GROUND CHECK - Make sure we're not in the ground
                    RaycastHit2D groundHit = Physics2D.Raycast(candidatePos, Vector2.down, groundCheckDistance, groundLayer);
                    
                    if (groundHit.collider != null)
                    {
                        // Adjust to be above ground
                        float groundY = groundHit.point.y + minHeightAboveGround;
                        candidatePos.y = groundY;
                        
                        // Verify final position is still in viewport
                        viewportPos = cam.WorldToViewportPoint(candidatePos);
                        if (viewportPos.x > 0.2f && viewportPos.x < 0.8f &&
                            viewportPos.y > 0.2f && viewportPos.y < 0.8f)
                        {
                            Debug.Log($"[Phase1] Valid teleport position found at {candidatePos}");
                            return candidatePos;
                        }
                    }
                }
            }
        }
        
        Debug.LogWarning("[Phase1] Could not find valid teleport position, staying in place");
        return transform.position;
    }
    
    public void StopAttacking()
    {
        StopAllCoroutines();
        if (animator != null) animator.SetBool("IsWalking", false);
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw melee range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
        
        // Draw teleport ranges
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minDistanceFromPlayer);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, maxDistanceFromPlayer);
    }
}