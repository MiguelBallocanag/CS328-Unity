using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Enemy Health Settings")]
    public int maxHealth = 3;
    public int currentHealth;
    private Animator animator;
    private bool isDead = false;

    [Header("Enemy Type Settings")]
    [SerializeField] private bool isSentinel = false;

    private string paramDeath;
    private string paramHurt;

    private EnemyAttack enemyAttack;
    private Rigidbody2D rb;

    void Start()
    {
        enemyAttack = GetComponent<EnemyAttack>();
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        if(isSentinel)
        {
            paramDeath = "IsDeath";
            paramHurt = "IsHurt";
        }
        else
        {
            paramDeath = "EnemyDie";
            paramHurt = "EnemyHurt";
        }
    }

    // NEW: Implement IDamageable interface for player combat system
    public void TakeHit(DamageContext ctx)
    {
        if (isDead) return;

        currentHealth -= ctx.damage;
        
        // Apply knockback
        if (rb != null)
        {
            rb.linearVelocity = ctx.knockback;
        }

        // Trigger hurt animation
        if (animator != null)
        {
            animator.SetTrigger(paramHurt);
        }

        Debug.Log($"{gameObject.name} took {ctx.damage} damage. Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Keep old TakeDamage for backwards compatibility (if other systems use it)
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        
        if (animator != null)
        {
            animator.SetTrigger(paramHurt);
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if(isDead) return;
        isDead = true;
        
        Debug.Log($"{gameObject.name} died!");
        
        if (animator != null)
        {
            animator.SetTrigger(paramDeath);
        }

        if(enemyAttack != null)
        {
            enemyAttack.OnEnemyDeath();
        }

        // Disable the enemy's collider to prevent further interaction
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        // Disable movement scripts
        EnemyPatrol patrol = GetComponent<EnemyPatrol>();
        if (patrol != null)
            patrol.enabled = false;

        UndeadSentinelChase chase = GetComponent<UndeadSentinelChase>();
        if (chase != null)
            chase.enabled = false;

        // Destroy the enemy after animation plays
        Destroy(gameObject, 2f);
    }
}