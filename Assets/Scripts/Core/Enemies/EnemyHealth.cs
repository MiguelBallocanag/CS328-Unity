using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Enemy Health Settings")]
    public int maxHealth = 30;
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
        
        Debug.Log($"[TakeHit] {gameObject.name} took {ctx.damage} damage. Health: {currentHealth}/{maxHealth}");

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
        AudioManager.Instance.PlayEnemyHurt();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // OLD METHOD - DISABLED FOR NOW
    public void TakeDamage(int damage)
    {
        Debug.LogWarning($"[OLD TakeDamage] called on {gameObject.name} - this should NOT be happening!");
        // DO NOTHING - we only use TakeHit now
        return;
        
        /* COMMENTED OUT TO PREVENT DOUBLE DAMAGE
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
        */
    }

    public void Die()
    {
        if(isDead) return;
        isDead = true;
        
        Debug.Log($"[Die] {gameObject.name} died!");
        
        if (animator != null)
        {
            animator.SetTrigger(paramDeath);
        }

        if(enemyAttack != null)
        {
            enemyAttack.OnEnemyDeath();
        }
        AudioManager.Instance.PlayEnemyDeath();

        // FIXED: Freeze physics instead of disabling collider
        // This keeps the corpse on the ground during death animation
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static; // Freeze in place
        }

        // Make collider a trigger so player can walk through corpse
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;

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