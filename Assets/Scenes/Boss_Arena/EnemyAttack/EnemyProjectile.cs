using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Settings")]
    public int damage = 10;
    public float lifetime = 10f;
    
    [Header("Visual")]
    public TrailRenderer trail; // Optional
    
    private Rigidbody2D rb;
    private bool hasHit = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Auto-destroy after lifetime
        Destroy(gameObject, lifetime);
    }
    
    public void Initialize(Vector2 direction, float speed)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        
        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * speed;
        }
        
        // Rotate sprite to face direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;
        
        // Check if hit player using tag
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[EnemyProjectile] Hit player!");
            
            // Deal damage using PlayerHealth system
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            
            hasHit = true;
            DestroyProjectile();
        }
        
        // Check if hit wall/ground using layer name
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log($"[EnemyProjectile] Hit wall");
            hasHit = true;
            DestroyProjectile();
        }
    }
    
    void DestroyProjectile()
    {
        // Optional: Spawn impact effect
        // Instantiate(impactEffect, transform.position, Quaternion.identity);
        
        // Optional: Play sound
        // AudioManager.Instance?.PlayProjectileHit();
        
        Destroy(gameObject);
    }
}