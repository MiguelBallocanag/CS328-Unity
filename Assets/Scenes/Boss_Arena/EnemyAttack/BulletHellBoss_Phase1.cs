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
    
    private int currentPatternIndex = 0;
    
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
        if (animator != null) animator.SetBool("IsWalking", false);
        StartCoroutine(AttackPattern());
    }
    
    IEnumerator AttackPattern()
    {
        while (true)
        {
            if (animator != null) animator.SetBool("IsWalking", false);
            yield return new WaitForSeconds(timeBetweenAttacks);
            
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
                Instantiate(lightBeamPrefab, new Vector3(xPos, attackPoint.position.y, 0), Quaternion.identity);
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
    
    public void StopAttacking()
    {
        StopAllCoroutines();
        if (animator != null) animator.SetBool("IsWalking", false);
    }
}