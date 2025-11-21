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
    public GameObject healthBar;
    
    private Animator animator;
    private Rigidbody2D rb;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        // Make sure Rigidbody2D is Dynamic
        if (rb != null && rb.bodyType != RigidbodyType2D.Dynamic)
        {
            Debug.LogWarning($"Boss {gameObject.name} Rigidbody2D should be Dynamic, not {rb.bodyType}!");
        }
    }

    // Implement IDamageable for player attacks
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
        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }

        // Update health bar
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Keep old TakeDamage for compatibility
    public void TakeDamage(int damage)
    {
        if (isDead || isInvulnerable) return;

        currentHealth -= damage;

        Debug.Log($"[Boss TakeDamage] {gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        // Update your health bar UI here if you have one
        if (healthBar != null)
        {
            // Assuming you have a script on healthBar that updates it
            // healthBar.GetComponent<HealthBarScript>().SetHealth(currentHealth, maxHealth);
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
            animator.SetTrigger("Death");
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
        Boss bossAI = GetComponent<Boss>();
        if (bossAI != null)
            bossAI.enabled = false;

        Boss_Weapon weapon = GetComponent<Boss_Weapon>();
        if (weapon != null)
            weapon.enabled = false;

        // Hide health bar
        if (healthBar != null)
            healthBar.SetActive(false);

        // Destroy after animation
        Destroy(gameObject, 3f);
    }

    // Public method to set invulnerability (for boss phases)
    public void SetInvulnerable(bool invulnerable)
    {
        isInvulnerable = invulnerable;
    }

    // Called by boss trigger to activate boss
    public void BossActive()
    {
        Debug.Log($"[Boss_Health] {gameObject.name} is now active!");
        // Enable boss AI if disabled
        Boss bossAI = GetComponent<Boss>();
        if (bossAI != null)
            bossAI.enabled = true;
    }
}