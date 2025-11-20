using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyDummy : MonoBehaviour, IDamageable 
{
    [Header("Health")]
    public int hp = 30;
    
    [Header("Visual Feedback")]
    public float flashDuration = 0.1f;
    public Color flashColor = Color.red;
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isFlashing = false;

    void Awake()
    { 
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void TakeHit(DamageContext ctx) 
    {
        hp -= ctx.damage;
        rb.linearVelocity = ctx.knockback;
        
        Debug.Log($"{gameObject.name} took {ctx.damage} damage. HP remaining: {hp}");
        
        // Visual feedback
        if (spriteRenderer != null && !isFlashing)
            StartCoroutine(Flash());
        
        if (hp <= 0) 
        {
            Debug.Log($"{gameObject.name} destroyed!");
            Destroy(gameObject);
        }
    }

    private System.Collections.IEnumerator Flash()
    {
        isFlashing = true;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }
        isFlashing = false;
    }
}


