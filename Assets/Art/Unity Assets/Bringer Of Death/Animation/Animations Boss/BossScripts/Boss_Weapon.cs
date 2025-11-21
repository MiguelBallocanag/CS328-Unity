using UnityEngine;

public class Boss_Weapon : MonoBehaviour
{
    [Header("Attack Settings")]
    public int attackDamage = 20;
    public int magicDamage = 30;

    [Header("Attack Hitbox")]
    public Vector3 attackOffset = new Vector3(1.5f, 0f, 0f);
    public float attackRange = 1.5f;
    public LayerMask playerLayer;
    
    [Header("Direction Detection")]
    [Tooltip("Use sprite flip detection (recommended for 2D bosses)")]
    public bool useSpriteFlipForDirection = true;
    
    private SpriteRenderer spriteRenderer;
    private BossReaper bossScript;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        bossScript = GetComponent<BossReaper>();
    }

    // Called by animation event during Boss_Attacking animation
    public void Attack()
    {
        Debug.Log("[Boss_Weapon] Attack() called!");
        PerformAttack(attackDamage, "Attack");
        AudioManager.Instance.PlayBossAttack();    
    }

    // Called by animation event during Boss_Magic animation (second phase)
    public void SecondPhaseAttack()
    {
        Debug.Log("[Boss_Weapon] SecondPhaseAttack() called!");
        PerformAttack(magicDamage, "Magic");
        AudioManager.Instance.PlayBossMagic();
    }

    // FIXED: Unified attack logic with proper direction handling
    private void PerformAttack(int damage, string attackType)
    {
        // Determine facing direction
        bool facingRight = GetFacingDirection();
        
        // Calculate attack position with correct direction
        Vector3 localOffset = attackOffset;
        if (!facingRight)
        {
            localOffset.x = -localOffset.x;  // Flip offset if facing left
        }
        
        Vector3 attackPos = transform.position + localOffset;

        // Check for player in attack range
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPos, attackRange, playerLayer);
        
        if (hitPlayer != null && hitPlayer.CompareTag("Player"))
        {
            Debug.Log($"[Boss_Weapon] {attackType} hit player: {hitPlayer.name}");
            
            PlayerHealth playerHealth = hitPlayer.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log($"[Boss_Weapon] Dealt {damage} {attackType} damage to player!");
            }
        }
        else
        {
            Debug.Log($"[Boss_Weapon] {attackType} missed - no player in range");
        }
    }

    // FIXED: Proper direction detection for 2D bosses
    private bool GetFacingDirection()
    {
        if (useSpriteFlipForDirection && spriteRenderer != null)
        {
            // Use sprite flip (most reliable for 2D)
            return !spriteRenderer.flipX;
        }
        else if (bossScript != null)
        {
            // Use boss script's flip state
            return !bossScript.isFlipped;
        }
        else
        {
            // Fallback: use transform scale
            return transform.localScale.x > 0;
        }
    }

    // Visualize attack range in editor with direction awareness
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            // In editor, show both directions
            Gizmos.color = Color.red;
            Vector3 rightPos = transform.position + attackOffset;
            Gizmos.DrawWireSphere(rightPos, attackRange);
            
            Gizmos.color = Color.yellow;
            Vector3 leftOffset = new Vector3(-attackOffset.x, attackOffset.y, attackOffset.z);
            Vector3 leftPos = transform.position + leftOffset;
            Gizmos.DrawWireSphere(leftPos, attackRange);
        }
        else
        {
            // In play mode, show only active direction
            bool facingRight = GetFacingDirection();
            Vector3 localOffset = attackOffset;
            if (!facingRight)
            {
                localOffset.x = -localOffset.x;
            }
            
            Vector3 attackPos = transform.position + localOffset;
            Gizmos.color = facingRight ? Color.red : Color.blue;
            Gizmos.DrawWireSphere(attackPos, attackRange);
        }
    }
}