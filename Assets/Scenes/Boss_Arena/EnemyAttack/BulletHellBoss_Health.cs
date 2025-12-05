using UnityEngine;
using System.Collections;

public class BulletHellBoss_Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 200;
    public int currentHealth;
    public int phase2HealthThreshold = 100;
    
    [Header("References")]
    public HealthBar healthBar;
    public Animator animator;
    
    [Header("Invulnerability")]
    public bool isInvulnerable = false;
    public float invulnerabilityDuration = 2f;
    
    private BulletHellBoss mainController;
    private bool hasTriggeredPhase2 = false;
    
    void Start()
    {
        currentHealth = maxHealth;
        mainController = GetComponent<BulletHellBoss>();
        
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null) animator = GetComponentInChildren<Animator>();
        }
        
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }
    }
    
    public void TakeDamage(int damageAmount)
    {
        if (isInvulnerable) return;
        
        currentHealth -= damageAmount;
        
        if (animator != null) animator.SetTrigger("Hit");
        if (healthBar != null) healthBar.SetHealth(currentHealth);
        
        if (!hasTriggeredPhase2 && currentHealth <= phase2HealthThreshold)
        {
            hasTriggeredPhase2 = true;
            if (mainController != null) mainController.TriggerPhase2Transition();
            StartCoroutine(TemporaryInvulnerability());
        }
        
        if (currentHealth <= 0) Die();
    }
    
    IEnumerator TemporaryInvulnerability()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }
    
    void Die()
    {
        if (animator != null) animator.SetTrigger("Die");
        
        BulletHellBoss_Phase1 phase1 = GetComponent<BulletHellBoss_Phase1>();
        if (phase1 != null) phase1.StopAttacking();
        
        BulletHellBoss_Phase2 phase2 = GetComponent<BulletHellBoss_Phase2>();
        if (phase2 != null) phase2.StopAttacking();
        
        StartCoroutine(DeathSequence());
    }
    
    IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }
}