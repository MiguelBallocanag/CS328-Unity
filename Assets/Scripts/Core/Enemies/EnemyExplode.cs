using UnityEngine;

public class BurningGhoul : MonoBehaviour
{
    [Header("Ranges")]
    public float detectionRange = 6f;
    public float explodeRange = 1f;

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Explosion")]
    public int explosionDamage = 50;
    public LayerMask playerLayer;
    public float destroyDelay = 0.8f;   // match explosion animation length

    [Header("Facing")]
    public bool facesRightByDefault = true; // flip this in Inspector if backwards

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;

    private bool isChasing = false;
    private bool isExploding = false;
    private Vector3 baseScale;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        rb.gravityScale = 1f;
        rb.freezeRotation = true;

        // Remember the original scale so we stop compounding flips
        baseScale = transform.localScale;
    }

    void Update()
    {
        if (isExploding || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange)
            isChasing = true;

        if (!isChasing) return;

        float dir = player.position.x - transform.position.x;
        if (Mathf.Approximately(dir, 0f))
            return;

        float dirSign = Mathf.Sign(dir);

        // Move toward player
        rb.linearVelocity = new Vector2(dirSign * moveSpeed, rb.linearVelocity.y);

        // Compute what direction we *want* to face visually
        float facingSign = facesRightByDefault ? dirSign : -dirSign;

        // Apply flip based ONLY on the original scale
        transform.localScale = new Vector3(
            Mathf.Abs(baseScale.x) * facingSign,
            baseScale.y,
            baseScale.z
        );

        // Start explosion when close enough
        if (distance <= explodeRange)
        {
            StartExplosion();
        }
    }

    void StartExplosion()
    {
        if (isExploding) return;

        isExploding = true;
        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger("Explode");

        // Guaranteed damage + delete
        Invoke(nameof(DoExplosionDamage), 0.35f);
        Destroy(gameObject, destroyDelay);
    }

    void DoExplosionDamage()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            transform.position,
            explodeRange,
            playerLayer
        );

        if (hit != null)
        {
            PlayerHealth ph = hit.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage(explosionDamage);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explodeRange);
    }
}
