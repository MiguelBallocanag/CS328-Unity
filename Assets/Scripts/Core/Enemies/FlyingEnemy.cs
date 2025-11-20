using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlyingEnemy : MonoBehaviour, IDamageable
{
    [Header("Flying Enemy Settings")]
    public float speed = 5f;
    public bool isChasing = false;
    public GameObject player;
    public Transform startingPoint;

    [Header("Health Settings")]
    public int maxHealth = 10;
    private int currentHealth;

    [Header("Flashing Settings")]
    public float flashDuration = 0.1f;
    public Color flashColor = Color.red;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isFlashing = false;

    [Header("Combat Settings")]
    public int attackDamage = 1;
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    private bool isAttacking = false;

    [Header("Animator")]
    public Animator animator;
    private bool isDead = false;

    private Rigidbody2D rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
        
        if(animator == null)
            animator = GetComponent<Animator>();
    }

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
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, player.transform.position) <= attackRange && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private void ReturnStartPoint()
    {
        transform.position = Vector2.MoveTowards(transform.position, startingPoint.position, speed * Time.deltaTime);
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
            animator.SetTrigger("IsAttacking");

        float attackAnimLength = 0.5f;
        yield return new WaitForSeconds(attackAnimLength);
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

    public void TakeHit(DamageContext ctx)
    {
        if (isDead) return;

        currentHealth -= ctx.damage;

        Debug.Log($"[TakeHit] {gameObject.name} took {ctx.damage} damage. Health: {currentHealth}/{maxHealth}");

        if (rb != null)
        {
            rb.linearVelocity = ctx.knockback;
        }

        if (spriteRenderer != null && !isFlashing)
            StartCoroutine(Flash());

        if (currentHealth <= 0)
            Die();
    }

    public void TakeDamage(int damage)
    {
        Debug.LogWarning($"[OLD TakeDamage] called on {gameObject.name}");
        return;
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
        if (isDead) return;
        isDead = true;

        Debug.Log($"[Die] {gameObject.name} died!");

        if (animator != null)
            animator.SetTrigger("IsDead");

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;

        this.enabled = false;
        Destroy(gameObject, 2f);
    }
}