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
        Debug.Log($"[TakeHit] {gameObject.name} Health: {currentHealth}/{maxHealth}");

        //  SAFE KNOCKBACK (DamageContext is a struct, so no null check)
        if (rb != null)
        {
            rb.linearVelocity = ctx.knockback;
        }

        //  SAFE ANIMATION
        if (animator != null && !string.IsNullOrEmpty(paramHurt))
        {
            animator.SetTrigger(paramHurt);
        }

        //  SAFE AUDIO
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayEnemyHurt();

        //  GUARANTEED DEATH
        if (currentHealth <= 0)
        {
            Debug.Log("HEALTH <= 0 : CALLING DIE()");
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
        if (isDead) return;
        isDead = true;

        Debug.Log("DIE FUNCTION EXECUTED ON: " + gameObject.name);

        if (enemyAttack != null)
            enemyAttack.OnEnemyDeath();

        WizardRangedAttack wizard = GetComponent<WizardRangedAttack>();
        if (wizard != null)
            wizard.OnEnemyDeath();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayEnemyDeath();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.simulated = false;

        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;

        // FULL ROOT DELETE
        Destroy(transform.root.gameObject);
    }
}