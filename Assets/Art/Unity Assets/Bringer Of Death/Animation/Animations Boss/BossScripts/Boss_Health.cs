using UnityEngine;
using System.Collections;

public class Boss_Health : MonoBehaviour, IDamageable
{
    [Header("Boss Health Settings")]
    public int maxHealth = 200;
    public int currentHealth;
    
    [Header("Death Effect")]
    public GameObject deathEffect;
    
    [Header("Invulnerability")]
    public bool isInvulnerable = false;
    
    [Header("Health Bar")]
    public HealthBar healthBar;
    
    private Animator animator;
    private Rigidbody2D rb;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        // Initialize health bar
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
            healthBar.gameObject.SetActive(false); // Hide until boss activates
        }
        
        // Make sure Rigidbody2D is Dynamic
        if (rb != null && rb.bodyType != RigidbodyType2D.Dynamic)
        {
            Debug.LogWarning($"Boss {gameObject.name} Rigidbody2D should be Dynamic, not {rb.bodyType}!");
        }
    }

    public void TakeHit(DamageContext ctx)
    {
        if (isDead || isInvulnerable) return;

        currentHealth -= ctx.damage;

        Debug.Log($"[Boss TakeHit] {gameObject.name} took {ctx.damage} damage. Health: {currentHealth}/{maxHealth}");

        // Apply knockback
        if (rb != null)
        {
            rb.linearVelocity = ctx.knockback;
        }

        // Trigger hurt animation if exists
        if (animator != null && HasAnimatorParameter(animator, "Hurt"))
        {
            animator.SetTrigger("Hurt");
        }
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBossHurt();
        }

        // Update health bar
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvulnerable) return;

        currentHealth -= damage;
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBossHurt();
        }

        Debug.Log($"[Boss TakeDamage] {gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"[Boss Die] {gameObject.name} defeated!");

        // Trigger death animation
        if (animator != null)
        {
            if (HasAnimatorParameter(animator, "Death"))
            {
                animator.SetTrigger("Death");
            }
            else if (HasAnimatorParameter(animator, "IsDead"))
            {
                animator.SetBool("IsDead", true);
            }
        }
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBossDeath();
        }

        // Spawn death effect
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Freeze physics
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        // Make collider a trigger so player can walk through
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;

        // Disable boss AI scripts
        BossReaper bossAI = GetComponent<BossReaper>();
        if (bossAI != null)
            bossAI.enabled = false;

        Boss_Weapon weapon = GetComponent<Boss_Weapon>();
        if (weapon != null)
            weapon.enabled = false;

        // Hide health bar
        if (healthBar != null)
            healthBar.gameObject.SetActive(false);

        // Destroy after animation
        Destroy(gameObject, 3f);
    }

    public void SetInvulnerable(bool invulnerable)
    {
        isInvulnerable = invulnerable;
    }

    // Called by boss trigger to activate boss
    public void BossActive()
    {
        Debug.Log($"[Boss_Health] {gameObject.name} is now active!");
        
        // Enable boss AI if disabled
        BossReaper bossAI = GetComponent<BossReaper>();
        if (bossAI != null)
        {
            bossAI.enabled = true;
            bossAI.ActivateBoss(); // NEW: Explicitly activate the boss
        }
        
        // Show health bar
        if (healthBar != null)
            healthBar.gameObject.SetActive(true);
    }
    
    private bool HasAnimatorParameter(Animator anim, string paramName)
    {
        if (anim == null) return false;
        
        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }
}