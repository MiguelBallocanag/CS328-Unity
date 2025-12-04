using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 6f;
    public float lifetime = 3f;

    [Header("Damage")]
    public int damage = 10;

    private Rigidbody2D rb;
    private Vector2 direction;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Start()
    {
        rb.linearVelocity = direction * speed;
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player hit
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage);
                Debug.Log("Fireball HIT PLAYER");
            }

            Destroy(gameObject);
            return;
        }

        // Ground / Wall hit
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }

}




