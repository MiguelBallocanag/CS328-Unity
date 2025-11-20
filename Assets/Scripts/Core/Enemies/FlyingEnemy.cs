using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Flying Enemy Settings")]
    public float speed = 5f;
    public bool isChasing = false;
    public GameObject player;
    public Transform StartingPoint;

    [Header("Health Settings")]
    public int maxHealth = 1;
    private int currentHealth;


    [Header("Flashing Settings")]
    public float flashDuration = 0.1f;
    public Color flashColor = Color.red;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isFlashing = false;

    [Header("Combat Settings")]
    public int attackDamage = 1; // Damage dealt per attack
    public float attackRange = 1f; // Range within which the enemy can attack
    public float attackCooldown = 1f; // Time between attacks in seconds
    private bool isAttacking = false;

    [Header("Animator")]
    public Animator animator;
    private bool isDead = false;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        
            originalColor = spriteRenderer.color;
        if(animator == null)
            animator = GetComponent<Animator>();


    }

    // Update is called once per frame
    void Update()
    {
        if (player == null || isDead)
            return;
        if (!isAttacking)
        {
            if (isChasing == true)
                Chase();
            else
                ReturnStartPoint();
        }
        
        Flip();
    }

    private void Chase()
    {
        // Move toward player
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

        // Attack if in range and not currently attacking
        if (Vector2.Distance(transform.position, player.transform.position) <= attackRange && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private void ReturnStartPoint()
    {
        transform.position = Vector2.MoveTowards(transform.position, StartingPoint.position, speed * Time.deltaTime);
    }

    private void Flip()
    {
        if(player == null)
            return;

        if (transform.position.x > player.transform.position.x)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            transform.rotation = Quaternion.Euler(0, 180, 0);
    }
    private IEnumerator PerformAttack()
    {
        isAttacking = true;

        if (animator != null)
            animator.SetTrigger("IsAttacking");  // Trigger attack animation

        // Wait for attack animation length (adjust to match animation)
        float attackAnimLength = 0.5f;
        yield return new WaitForSeconds(attackAnimLength);

        // Optional pause between attacks
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
    }
    public void DealAttackDamage()
    {
        if (player == null) return;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (spriteRenderer != null && !isFlashing)
            StartCoroutine(Flash());

        if (currentHealth <= 0)
            Die();
    }

    private IEnumerator Flash()
    {
        isFlashing = true;
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
        isFlashing = false;
    }

    private void Die()
    {
        isDead = true;

        // Trigger the death animation
        if (animator != null)
            animator.SetTrigger("IsDead"); // Make sure this trigger exists in your Animator

        // Stop movement and disable collisions
        this.enabled = false;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;
    }
}