using UnityEngine;
using System.Collections;

public class BulletHellBoss_Phase1 : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform attackPoint;
    public Animator animator;
    public LayerMask groundLayer;

    [Header("Room Bounds (Teleport Rectangle)")]
    public Vector2 roomMin;   // bottom-left of room (set in Inspector)
    public Vector2 roomMax;   // top-right of room (set in Inspector)

    [Header("Projectile Settings")]
    public GameObject orbPrefab;
    public float orbSpeed = 12f;
    public float orbSpawnOffset = 2.5f; // how far from boss orbs spawn

    [Header("Light Beam Settings")]
    public GameObject lightBeamPrefab;
    public float beamWarningTime = 1.5f;
    public float beamDamageTime = 2.5f;
    public int beamCount = 3;
    public float beamSpacing = 3f;

    [Header("Melee Settings (Circle)")]
    public float meleeRange = 1.8f;   // <– make this smaller if melee feels too big
    public int meleeDamage = 20;
    public float meleeKnockback = 10f;

    [Header("Attack Timing")]
    public float timeBetweenAttacks = 2f;

    [Header("Pattern 1: Circular Burst")]
    public int burstOrbCount = 8;

    [Header("Pattern 2: Aimed Shot")]
    public int aimedShotCount = 3;

    [Header("Pattern 3: Spiral")]
    public float spiralDuration = 3f;
    public float spiralOrbInterval = 0.1f;
    public float spiralRotationSpeed = 60f;

    [Header("Teleport Settings")]
    public bool enableTeleport = true;
    public float teleportCooldown = 1.2f;
    public float teleportFlashDuration = 0.12f;
    public GameObject teleportEffectPrefab;

    private int currentPatternIndex = 0;
    private float lastTeleportTime = -999f;
    private bool isStunned = false;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Color originalColor;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if (sr != null)
            originalColor = sr.color;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null) animator = GetComponentInChildren<Animator>();
        }

        // IMPORTANT: we DO NOT auto-start attacks here.
        // BulletHellBoss.cs will call StartPhase1().
    }

    // ============================================================
    // PHASE ENTRY (called by BulletHellBoss)
    // ============================================================
    public void StartPhase1()
    {
        Debug.Log("[Phase1] StartPhase1 called");
        isStunned = false;
        currentPatternIndex = 0;
        StopAllCoroutines();
        StartCoroutine(AttackPattern());
    }

    // ============================================================
    // STUN SUPPORT (called by BulletHellBoss_Health)
    // ============================================================
    public void Stun(float duration)
    {
        if (!gameObject.activeInHierarchy) return;
        StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (animator != null)
            animator.SetBool("IsWalking", false);

        yield return new WaitForSeconds(duration);

        isStunned = false;
    }

    // ============================================================
    // MAIN ATTACK LOOP
    // ============================================================
    private IEnumerator AttackPattern()
    {
        while (true)
        {
            // Small guard if stunned – just wait a frame
            if (isStunned)
            {
                yield return null;
                continue;
            }

            yield return new WaitForSeconds(timeBetweenAttacks);
            if (isStunned) continue;

            // Teleport sometimes before attacking
            if (enableTeleport && Time.time - lastTeleportTime >= teleportCooldown)
            {
                yield return StartCoroutine(TeleportSequence());
                if (isStunned) continue;
            }

            // Melee if player is within melee circle
            if (player != null)
            {
                float dist = Vector2.Distance(transform.position, player.position);
                if (dist <= meleeRange)
                {
                    yield return StartCoroutine(PerformMeleeAttack());
                    continue;
                }
            }

            // Otherwise use ranged pattern
            switch (currentPatternIndex)
            {
                case 0:
                    yield return StartCoroutine(PerformCircularBurst());
                    break;
                case 1:
                    yield return StartCoroutine(PerformAimedShot());
                    break;
                case 2:
                    yield return StartCoroutine(PerformSpiral());
                    break;
                case 3:
                    yield return StartCoroutine(PerformLightBeams());
                    break;
            }

            currentPatternIndex = (currentPatternIndex + 1) % 4;
        }
    }

    // ============================================================
    // MELEE (CIRCULAR RANGE)
    // ============================================================
    private IEnumerator PerformMeleeAttack()
    {
        if (animator != null)
            animator.SetTrigger("Melee");

        yield return new WaitForSeconds(0.15f);

        if (player == null) yield break;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > meleeRange) yield break;

        PlayerHealth hp = player.GetComponent<PlayerHealth>();
        if (hp != null)
            hp.TakeDamage(meleeDamage);

        Rigidbody2D prb = player.GetComponent<Rigidbody2D>();
        if (prb != null)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            prb.linearVelocity = dir * meleeKnockback;
        }
    }

    // ============================================================
    // CIRCULAR BURST
    // ============================================================
    private IEnumerator PerformCircularBurst()
    {
        if (animator != null)
            animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.15f);

        for (int i = 0; i < burstOrbCount; i++)
        {
            float ang = i * (360f / burstOrbCount);
            SpawnOrbAngle(ang);
        }
    }

    // ============================================================
    // AIMED SHOT
    // ============================================================
    private IEnumerator PerformAimedShot()
    {
        if (animator != null)
            animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.15f);

        if (player == null || attackPoint == null) yield break;

        for (int i = 0; i < aimedShotCount; i++)
        {
            Vector2 dir = (player.position - attackPoint.position).normalized;
            float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            SpawnOrbAngle(ang);
            yield return new WaitForSeconds(0.1f);
        }
    }

    // ============================================================
    // SPIRAL
    // ============================================================
    private IEnumerator PerformSpiral()
    {
        if (animator != null)
            animator.SetTrigger("Attack");

        float elapsed = 0f;
        float angle = 0f;

        while (elapsed < spiralDuration)
        {
            SpawnOrbAngle(angle);
            angle += spiralRotationSpeed * spiralOrbInterval;
            elapsed += spiralOrbInterval;
            yield return new WaitForSeconds(spiralOrbInterval);
        }
    }

    // ============================================================
    // LIGHT BEAMS ALIGNED WITH PLAYER Y
    // ============================================================
    private IEnumerator PerformLightBeams()
    {
        if (animator != null)
            animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.2f);

        if (player == null || lightBeamPrefab == null) yield break;

        float px = player.position.x;
        float py = player.position.y;
        float startX = px - ((beamCount - 1) * beamSpacing / 2f);

        for (int i = 0; i < beamCount; i++)
        {
            float xPos = startX + (i * beamSpacing);
            Vector3 pos = new Vector3(xPos, py, 0);

            GameObject beam = Instantiate(lightBeamPrefab, pos, Quaternion.identity);
            LightBeam lb = beam.GetComponent<LightBeam>();
            if (lb != null)
                lb.Initialize(beamWarningTime, beamDamageTime);
        }
    }

    // ============================================================
    // ORB SPAWN (NO PARENTING, NO “STUCK” BEHAVIOR)
    // ============================================================
    private void SpawnOrbAngle(float angleDegrees)
    {
        if (orbPrefab == null || attackPoint == null) return;

        float rad = angleDegrees * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        Vector3 spawnPos = attackPoint.position + (Vector3)(dir * orbSpawnOffset);

        GameObject orb = Instantiate(orbPrefab, spawnPos, Quaternion.identity);
        orb.transform.parent = null; // make sure it isn't attached to the boss

        EnemyProjectile ep = orb.GetComponent<EnemyProjectile>();
        if (ep != null)
        {
            ep.Initialize(dir, orbSpeed);
        }
        else
        {
            Rigidbody2D orbRB = orb.GetComponent<Rigidbody2D>();
            if (orbRB != null)
            {
                orbRB.bodyType = RigidbodyType2D.Dynamic;
                orbRB.gravityScale = 0f;
                orbRB.linearVelocity = dir * orbSpeed;
            }
        }
    }

    // ============================================================
    // TELEPORT INSIDE ROOM RECTANGLE
    // ============================================================
    private IEnumerator TeleportSequence()
    {
        lastTeleportTime = Time.time;

        if (teleportEffectPrefab != null)
            Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(teleportFlashDuration);

        Vector3 pos = GetTeleportPosition();
        transform.position = pos;

        // 🔥 After teleport, immediately face the player
        FacePlayer();

        if (teleportEffectPrefab != null)
            Instantiate(teleportEffectPrefab, transform.position, Quaternion.identity);
    }


    private Vector3 GetTeleportPosition()
    {
        // Simple rectangular area: always inside room
        float x = Random.Range(roomMin.x, roomMax.x);
        float y = Random.Range(roomMin.y, roomMax.y);
        return new Vector3(x, y, 0f);
    }

    // ============================================================
    // FLASH RED
    // ============================================================
    private void FlashRed()
    {
        if (sr == null) return;
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        sr.color = originalColor;
    }
    private void FacePlayer()
    {
        if (player == null || sr == null) return;

        // If player is to the right of the boss
        if (player.position.x > transform.position.x)
        {
            // Face right (assuming default sprite faces right)
            sr.flipX = false;
        }
        else
        {
          // Face left
         sr.flipX = true;
        }
    }


    // ============================================================
    // STOP ATTACKING (CALLED ON DEATH)
    // ============================================================
    public void StopAttacking()
    {
        StopAllCoroutines();

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (animator != null)
            animator.SetBool("IsWalking", false);

        Debug.Log("[Phase1] StopAttacking called – boss is dead or disabled.");
    }

    // ============================================================
    // GIZMOS (so you can see melee circle in Scene view)
    // ============================================================
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);

        // Optional: draw room rectangle in yellow
        Gizmos.color = Color.yellow;
        Vector3 bottomLeft = new Vector3(roomMin.x, roomMin.y, 0f);
        Vector3 topRight = new Vector3(roomMax.x, roomMax.y, 0f);
        Vector3 topLeft = new Vector3(roomMin.x, roomMax.y, 0f);
        Vector3 bottomRight = new Vector3(roomMax.x, roomMin.y, 0f);

        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
    }
}
