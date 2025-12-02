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
        // Try to damage ANY damageable object
        IDamageable dmg = other.GetComponent<IDamageable>();
        if (dmg == null)
            dmg = other.GetComponentInParent<IDamageable>();

        if (dmg != null)
        {
            DamageContext ctx = new DamageContext
            {
                damage = damage,
                knockback = Vector2.zero
            };

            dmg.TakeHit(ctx);

            // DESTROY IMMEDIATELY AFTER SUCCESSFUL DAMAGE
            Destroy(gameObject);
            return;
        }

        // If it hit something solid (wall/ground/etc)
        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}




