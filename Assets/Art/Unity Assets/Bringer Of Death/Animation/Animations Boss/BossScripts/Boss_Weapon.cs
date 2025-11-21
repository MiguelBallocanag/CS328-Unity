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

    // Called by animation event during Boss_Attacking animation
    public void Attack()
    {
        Debug.Log("[Boss_Weapon] Attack() called!");
        
        // Calculate attack position (offset from boss position)
        Vector3 attackPos = transform.position;
        attackPos += transform.right * attackOffset.x;
        attackPos += transform.up * attackOffset.y;

        // Check for player in attack range
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPos, attackRange, playerLayer);
        
        if (hitPlayer != null && hitPlayer.CompareTag("Player"))
        {
            Debug.Log($"[Boss_Weapon] Hit player: {hitPlayer.name}");
            
            PlayerHealth playerHealth = hitPlayer.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log($"[Boss_Weapon] Dealt {attackDamage} damage to player!");
            }
        }
        else
        {
            Debug.Log("[Boss_Weapon] Attack missed - no player in range");
        }
    }

    // Called by animation event during Boss_Magic animation (second phase)
    public void SecondPhaseAttack()
    {
        Debug.Log("[Boss_Weapon] SecondPhaseAttack() called!");
        
        Vector3 attackPos = transform.position;
        attackPos += transform.right * attackOffset.x;
        attackPos += transform.up * attackOffset.y;

        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPos, attackRange, playerLayer);
        
        if (hitPlayer != null && hitPlayer.CompareTag("Player"))
        {
            Debug.Log($"[Boss_Weapon] Magic hit player: {hitPlayer.name}");
            
            PlayerHealth playerHealth = hitPlayer.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(magicDamage);
                Debug.Log($"[Boss_Weapon] Dealt {magicDamage} magic damage to player!");
            }
        }
        else
        {
            Debug.Log("[Boss_Weapon] Magic attack missed");
        }
    }

    // Visualize attack range in editor
    private void OnDrawGizmosSelected()
    {
        Vector3 attackPos = transform.position;
        attackPos += transform.right * attackOffset.x;
        attackPos += transform.up * attackOffset.y;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos, attackRange);
    }
}