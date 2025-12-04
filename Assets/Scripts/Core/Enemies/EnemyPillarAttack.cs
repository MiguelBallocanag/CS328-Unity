using UnityEngine;

public class HolyPillar : MonoBehaviour
{
    public int damage = 25;
    public float warningTime = 1f;
    public float lifeTime = 0.2f;

    public float warningAlpha = 0.4f;
    public float hitAlpha = 1f;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        // Start translucent (warning)
        if (sr != null)
        {
            Color c = sr.color;
            c.a = warningAlpha;
            sr.color = c;
        }

        Invoke(nameof(DealDamage), warningTime);
        Destroy(gameObject, warningTime + lifeTime);
    }

    void DealDamage()
    {
        // Turn fully visible at the hit moment
        if (sr != null)
        {
            Color c = sr.color;
            c.a = hitAlpha;
            sr.color = c;
        }

        Collider2D hit = Physics2D.OverlapBox(
            transform.position,
            GetComponent<Collider2D>().bounds.size,
            0f
        );

        if (hit != null && hit.CompareTag("Player"))
        {
            PlayerHealth ph = hit.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage);
            }
        }
    }
}



