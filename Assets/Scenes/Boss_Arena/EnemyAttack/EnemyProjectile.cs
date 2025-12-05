using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Settings")]
    public int damage = 10;
    public float lifetime = 10f;

    private Rigidbody2D rb;
    private Vector2 direction;
    private bool hasHit = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.freezeRotation = true;
        }
    }

    // Called by boss script
    public void Initialize(Vector2 dir, float speed)
    {
        direction = dir.normalized;

        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
    }

    void FixedUpdate()
    {
        if (rb != null && !hasHit)
        {
            rb.linearVelocity = direction * rb.linearVelocity.magnitude;
        }
    }

    void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        Debug.Log($"[EnemyProjectile] Trigger with {other.name}, tag={other.tag}, layer={other.gameObject.layer}");

        // Try to find PlayerHealth anywhere up the hierarchy
        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth != null)
        {
            Debug.Log($"[EnemyProjectile] Dealing {damage} damage to player");
            playerHealth.TakeDamage(damage);
            hasHit = true;
            Destroy(gameObject);
            return;
        }

        // If it hits ground, walls, etc, destroy it too
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            hasHit = true;
            Destroy(gameObject);
        }
    }
}
