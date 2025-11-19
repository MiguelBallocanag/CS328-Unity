using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EnemyHealth : MonoBehaviour
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyAttack = GetComponent<EnemyAttack>();
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


    // Method to apply damage to the enemy
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        animator.SetTrigger(paramHurt);
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    // Method to handle enemy death
    public void Die()
    {
        if(isDead) return;
        animator.SetTrigger(paramDeath);

        if(enemyAttack != null)
        {
            enemyAttack.OnEnemyDeath();
        }
        // Disable the enemy's collider and other components to prevent further interaction
         Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;
        // Destroy the enemy game object after a delay to allow death animation to play
        Destroy(gameObject, 2f);

    }
}
