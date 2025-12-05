using UnityEngine;
using System.Collections;

public class BulletHellBoss_Phase2 : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    
    [Header("Rush Settings")]
    public float rushSpeed = 15f;
    public float telegraphTime = 1f;
    public float stunTime = 0.5f;
    public float maxRushDistance = 20f;
    
    [Header("Attack Pattern")]
    public float timeBetweenRushes = 2f;
    public int rushesPerCombo = 1;
    public float comboDelay = 0.3f;
    
    [Header("Damage")]
    public int rushDamage = 25;
    
    [Header("Visual Effects")]
    public Color telegraphColor = Color.red;
    
    private Rigidbody2D rb;
    private Color originalColor;
    private Vector2 rushDirection;
    private bool isRushing = false;
    private bool isTelegraphing = false;
    private float rushDistanceTraveled = 0f;
    private Vector3 rushStartPosition;
    private bool hasHitPlayer = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
        
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null) animator = GetComponentInChildren<Animator>();
        }
        
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
    }
    
    public void StartPhase2()
    {
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        
        if (animator != null)
        {
            animator.SetBool("IsPhase2", true);
            animator.SetBool("IsWalking", false);
        }
        
        StartCoroutine(RushAttackPattern());
    }
    
    IEnumerator RushAttackPattern()
    {
        while (true)
        {
            if (animator != null) animator.SetBool("IsWalking", false);
            yield return new WaitForSeconds(timeBetweenRushes);
            
            for (int i = 0; i < rushesPerCombo; i++)
            {
                yield return StartCoroutine(Telegraph());
                yield return StartCoroutine(Rush());
                yield return StartCoroutine(Stun());
                if (i < rushesPerCombo - 1) yield return new WaitForSeconds(comboDelay);
            }
        }
    }
    
    IEnumerator Telegraph()
    {
        isTelegraphing = true;
        if (animator != null) animator.SetBool("IsWalking", true);
        
        if (player != null) rushDirection = (player.position - transform.position).normalized;
        else rushDirection = Vector2.right;
        
        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (spriteRenderer != null) StartCoroutine(FlashTelegraph());
        
        yield return new WaitForSeconds(telegraphTime);
        
        if (spriteRenderer != null) spriteRenderer.color = originalColor;
        isTelegraphing = false;
    }
    
    IEnumerator FlashTelegraph()
    {
        float elapsed = 0f;
        while (elapsed < telegraphTime && isTelegraphing)
        {
            float t = Mathf.PingPong(elapsed * 5f, 1f);
            if (spriteRenderer != null) spriteRenderer.color = Color.Lerp(originalColor, telegraphColor, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    
    IEnumerator Rush()
    {
        if (animator != null) animator.SetTrigger("Attack");
        
        isRushing = true;
        hasHitPlayer = false;
        rushDistanceTraveled = 0f;
        rushStartPosition = transform.position;
        
        if (rb != null) rb.linearVelocity = rushDirection * rushSpeed;
        while (isRushing) yield return null;
    }
    
    void Update()
    {
        if (!isRushing) return;
        rushDistanceTraveled = Vector3.Distance(rushStartPosition, transform.position);
        if (rushDistanceTraveled >= maxRushDistance) StopRush();
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isRushing) return;
        
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!hasHitPlayer)
            {
                PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
                if (playerHealth != null) playerHealth.TakeDamage(rushDamage);
                hasHitPlayer = true;
            }
            StopRush();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            StopRush();
        }
    }
    
    void StopRush()
    {
        if (!isRushing) return;
        isRushing = false;
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }
    
    IEnumerator Stun()
    {
        if (animator != null) animator.SetBool("IsWalking", false);
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(originalColor.r * 0.7f, originalColor.g * 0.7f, originalColor.b * 0.7f, originalColor.a);
        }
        
        yield return new WaitForSeconds(stunTime);
        
        if (spriteRenderer != null) spriteRenderer.color = originalColor;
    }
    
    public void StopAttacking()
    {
        StopAllCoroutines();
        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (animator != null) animator.SetBool("IsWalking", false);
    }
}