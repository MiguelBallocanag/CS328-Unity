using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BulletHellBoss_Health : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public int maxHealth = 200;
    public int currentHealth;
    
    [Header("References")]
    public HealthBar healthBar;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    
    [Header("Damage Flash")]
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;
    private Color originalColor;
    private bool isFlashing = false;
    
    [Header("Stun on Damage")]
    public float stunDuration = 0.3f;
    
    [Header("Invulnerability")]
    public bool isInvulnerable = false;
    
    [Header("Victory Settings")]
    public string mainMenuScene = "GMainmenu";
    public float victoryDelay = 3f;
    
    private BulletHellBoss_Phase1 phase1Controller;
    private bool isDead = false;
    
    void Start()
    {
        currentHealth = maxHealth;
        phase1Controller = GetComponent<BulletHellBoss_Phase1>();
        
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null) animator = GetComponentInChildren<Animator>();
        }
        
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }
    }
    
    public void TakeHit(DamageContext ctx)
    {
        TakeDamage(ctx.damage);
    }
    
    public void TakeDamage(int damageAmount)
    {
        if (isInvulnerable || isDead) return;
        
        currentHealth -= damageAmount;
        
        if (spriteRenderer != null && !isFlashing)
        {
            StartCoroutine(FlashRed());
        }
        
        if (phase1Controller != null)
        {
            phase1Controller.Stun(stunDuration);
        }
        
        if (animator != null) animator.SetTrigger("Hit");
        if (healthBar != null) healthBar.SetHealth(currentHealth);
        
        Debug.Log($"[BossHealth] Boss took {damageAmount} damage. Health: {currentHealth}/{maxHealth}");
        
        // NO PHASE 2 - Just die when health reaches 0
        if (currentHealth <= 0) Die();
    }
    
    IEnumerator FlashRed()
    {
        if (spriteRenderer == null) yield break;
        
        isFlashing = true;
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
        isFlashing = false;
    }
    
    void Die()
    {
        if (isDead) return;
        isDead = true;
        
        Debug.Log("[BossHealth] BOSS DEFEATED! YOU WIN!");
        
        if (animator != null) animator.SetTrigger("Die");
        
        // Stop attacks
        BulletHellBoss_Phase1 phase1 = GetComponent<BulletHellBoss_Phase1>();
        if (phase1 != null) phase1.StopAttacking();
        
        // Destroy all projectiles
        foreach (EnemyProjectile proj in FindObjectsByType<EnemyProjectile>(FindObjectsSortMode.None))
        {
            Destroy(proj.gameObject);
        }
        
        // Destroy all light beams
        foreach (LightBeam beam in FindObjectsByType<LightBeam>(FindObjectsSortMode.None))
        {
            Destroy(beam.gameObject);
        }
        
        StartCoroutine(VictorySequence());
    }
    
    IEnumerator VictorySequence()
    {
        EndScreenController endScreen = FindFirstObjectByType<EndScreenController>();
        
        if (endScreen != null)
        {
            if (endScreen.endText != null)
            {
                endScreen.endText.text = "VICTORY";
            }
            endScreen.gameObject.SetActive(true);
            yield return StartCoroutine(endScreen.FadeToBlack());
        }
        
        yield return new WaitForSeconds(victoryDelay);
        
        SceneManager.LoadScene(mainMenuScene);
    }
}
