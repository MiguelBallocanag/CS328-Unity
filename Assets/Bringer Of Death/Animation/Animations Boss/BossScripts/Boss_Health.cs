using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss_Health : MonoBehaviour
{
    public int maxHealth = 200;
    public GameObject deathEffect;
    public bool isInvulnerable = false;
    public HealthBar healthBar;
    private bool isActive=false;

    private Animator animator;
    private int currentHealth;
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        animator = GetComponent<Animator>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void BossActive()
    {
        isActive = true;
    }

    public void TakeDamage(int damage)
    {
        if (!isActive || isInvulnerable)
        
            return;
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        animator.SetTrigger("IsHurt");
        StartCoroutine(InvunberabilityFrames(animator.GetCurrentAnimatorStateInfo(11).length));





        if (maxHealth <= 100 && currentHealth>0)
        {
            GetComponent<Animator>().SetBool("CastMagic", true);
        }
        if (maxHealth <= 0)
        {

            Die();
        }
    }

    // Update is called once per frame
    private IEnumerator InvunberabilityFrames(float duration)
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(duration);
        isInvulnerable = false;
    }
    private void Die()
    {   GetComponent<Animator>().SetTrigger("Death");
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject, 1.5f);
    }
}

