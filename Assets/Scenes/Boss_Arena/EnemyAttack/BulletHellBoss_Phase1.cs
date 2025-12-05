using UnityEngine;
using System.Collections;

public class BulletHellBoss_Phase1 : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform attackPoint;
    public Animator animator;
    
    [Header("Projectile Settings")]
    public GameObject orbPrefab;
    public float orbSpeed = 5f;
    
    [Header("Light Beam Settings")]
    public GameObject lightBeamPrefab;
    public float beamWarningTime = 1f;
    public float beamDamageTime = 2f;
    
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
    
    [Header("Teleport Settings")]
    public bool enableTeleport = true;
    public float minDistanceFromPlayer = 8f;
    public float maxDistanceFromPlayer = 15f;
    public float teleportCooldown = 5f;
    public float edgeBuffer = 3f;
    public GameObject teleportEffectPrefab;
    public float teleportFlashDuration = 0.3f;
    
    private int currentPatternIndex = 0;
    private float lastTeleportTime = -999f;
    
    void Start()
    {
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
    }
    
    public void StartPhase1()
    {
        Debug.Log("[Phase1] Phase 1 activated!");
        if (animator != null) animator.SetBool("IsWalking", false);
        StartCoroutine(AttackPattern());
    }
    
    IEnumerator AttackPattern()
    {
        while (true)
        {
            if (animator != null) animator.SetBool("IsWalking", false);
            yield return new WaitForSeconds(timeBetweenAttacks);
            
            // Check for teleport before attacking
            if (enableTeleport)
            {
                CheckAndTeleport();
            }
            
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
    
    IEnumerator PerformCircularBurst()
    {
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
        if (player == null) yield break;
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
        if (animator != null) animator.SetTrigger("Attack");
        
        float elapsed = 0f;
        float currentAngle = 0f;
        
        while (elapsed < spiralDuration)
        {
            SpawnOrb(currentAngle);
            currentAngle += spiralRotationSpeed * spiralOrbInterval;
            elapsed += spiralOrbInterval;
            yield return new WaitForSeconds(spiralOrbInterval);
        }
    }
    
    IEnumerator PerformLightBeams()
    {
        if (animator != null) animator.SetTrigger("Attack");
        yield return new WaitForSeconds(0.2f);
        
        if (lightBeamPrefab != null && attackPoint != null)
        {
            float startX = attackPoint.position.x - ((beamCount - 1) * beamSpacing / 2f);
            for (int i = 0; i < beamCount; i++)
            {
                float xPos = startX + (i * beamSpacing);
                GameObject beam = Instantiate(lightBeamPrefab, new Vector3(xPos, attackPoint.position.y, 0), Quaternion.identity);
                
                // Initialize beam with timing
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
        
        GameObject orb = Instantiate(orbPrefab, attackPoint.position, Quaternion.identity);
        float angleRad = angleDegrees * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        
        Rigidbody2D rb = orb.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = direction * orbSpeed;
    }
    
    void CheckAndTeleport()
    {
        if (Time.time - lastTeleportTime < teleportCooldown) return;
        if (player == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // Too close
        if (distanceToPlayer < minDistanceFromPlayer)
        {
            Debug.Log("[Phase1] Too close, teleporting away!");
            StartCoroutine(TeleportSequence());
        }
        // Too far
        else if (distanceToPlayer > maxDistanceFromPlayer)
        {
            Debug.Log("[Phase1] Too far, teleporting closer!");
            StartCoroutine(TeleportSequence());
        }
        // Near screen edge
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
        
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-10f, 10f),
                Random.Range(-5f, 5f),
                0
            );
            
            Vector3 candidatePos = player.position + randomOffset;
            float distance = Vector2.Distance(candidatePos, player.position);
            
            if (distance >= minDistanceFromPlayer && distance <= maxDistanceFromPlayer)
            {
                Vector3 viewportPos = cam.WorldToViewportPoint(candidatePos);
                
                if (viewportPos.x > 0.2f && viewportPos.x < 0.8f &&
                    viewportPos.y > 0.2f && viewportPos.y < 0.8f)
                {
                    return candidatePos;
                }
            }
        }
        
        return transform.position;
    }
    
    public void StopAttacking()
    {
        StopAllCoroutines();
        if (animator != null) animator.SetBool("IsWalking", false);
    }
}