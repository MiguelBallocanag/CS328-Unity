using UnityEngine;

public class EnemyProjectile : MonoBehaviour, IDamageable
{
    [Header("Settings")]
    public int damage = 10;
    public float lifetime = 5f;

    private Vector2 direction;
    private float speed;
    private bool hasHit = false;

    void Awake()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0f;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    public void Initialize(Vector2 dir, float spd)
    {
        direction = dir.normalized;
        speed = spd;
    }

    void Update()
    {
        if (!hasHit)
        {
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
        }

        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            hasHit = true;
            Destroy(gameObject);
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            hasHit = true;
            Destroy(gameObject);
        }
    }

    // Player sword can destroy orbs!
    public void TakeHit(DamageContext ctx)
    {
        Debug.Log("[EnemyProjectile] Orb destroyed by player!");
        Destroy(gameObject);
    }
}
