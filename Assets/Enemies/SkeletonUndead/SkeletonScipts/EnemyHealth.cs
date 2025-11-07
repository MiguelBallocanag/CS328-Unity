using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;
    private Animator animator;
    private bool EnemyDead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }


    // Method to apply damage to the enemy
    public void TakeDamage(int damage)
    {
        if (EnemyDead) return;

        currentHealth -= damage;
        animator.SetTrigger("EnemyHurt");
        if (currentHealth <= 0)
        {
           Destroy(gameObject);
        }
    }
    // Method to handle enemy death
    void Die()
    {
        EnemyDead = true;
        animator.SetTrigger("EnemyDead");
        // Disable the enemy's collider and other components to prevent further interaction
        GetComponent<Collider>().enabled = false;
        this.enabled = true;

    }
}
