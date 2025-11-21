using UnityEngine;

public class BossReaper : MonoBehaviour
{   
    [Header("References")]
    public Transform player;
    private Animator animator;
    private Boss_Health bossHealth;
    private Rigidbody2D rb;

    [Header("Flip Settings")]
    public bool isFlipped = false;

    [Header("Movement Settings")]
    public float moveSpeed = 3.5f;
    public float chaseRange = 15f;
    
    [Header("Attack Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 2.0f;
    
    [Header("Ground Check (Optional)")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded = true;
    
    private float lastAttackTime = -999f;
    private bool isAttacking = false;
    
    private bool IsDead => bossHealth != null && bossHealth.currentHealth <= 0;
    private bool IsActive { get; set; } = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        bossHealth = GetComponent<Boss_Health>();
        rb = GetComponent<Rigidbody2D>();
        
        if (rb == null)
        {
            Debug.LogError("[BossReaper] No Rigidbody2D found! Boss needs Rigidbody2D to move.");
        }
        
        // Auto-find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("[BossReaper] Auto-found player");
            }
            else
            {
                Debug.LogError("[BossReaper] Could not find player! Make sure player has 'Player' tag.");
            }
        }
    }

    void Update()
    {
        if (!IsActive || player == null || IsDead) return;
        
        // Look at player
        LookAtPlayer();
        
        // Update animator
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (!IsActive || player == null || rb == null || IsDead) return;
        
        // Check if grounded (if ground check is set up)
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
        
        float distance = Vector2.Distance(transform.position, player.position);
        
        // Move toward player (only if grounded or no ground check)
        if (groundCheck == null || isGrounded)
        {
            MoveTowardPlayer();
        }
        
        // Try to attack if in range
        TryAttack(distance);
    }

    private void MoveTowardPlayer()
    {
        if (player == null || rb == null) return;
        
        // FIXED: Use velocity instead of MovePosition so gravity works properly
        Vector2 currentVel = rb.linearVelocity;
        float targetVelX = 0f;
        
        // Calculate desired X velocity toward player
        float distanceX = player.position.x - transform.position.x;
        if (Mathf.Abs(distanceX) > 0.1f)
        {
            float direction = Mathf.Sign(distanceX);
            targetVelX = direction * moveSpeed;
        }
        
        // Apply X velocity while preserving Y velocity (gravity)
        rb.linearVelocity = new Vector2(targetVelX, currentVel.y);
        
        Debug.Log($"[BossReaper] Moving toward player. Distance: {Vector2.Distance(transform.position, player.position):F2}");
    }

    private void TryAttack(float distance)
    {
        if (isAttacking) return;
        
        float timeSinceLastAttack = Time.time - lastAttackTime;
        bool inRange = distance <= attackRange;
        bool cooldownReady = timeSinceLastAttack >= attackCooldown;
        
        if (inRange && cooldownReady)
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        
        if (animator != null && HasAnimatorParameter(animator, "Attack"))
        {
            animator.SetTrigger("Attack");
        }
        
        Debug.Log("[BossReaper] Attack started!");
        
        // Reset attacking flag after animation would complete
        // Adjust this timing to match your attack animation length
        Invoke(nameof(EndAttack), 1.0f); // Assumes 1 second attack animation
    }

    private void EndAttack()
    {
        isAttacking = false;
        Debug.Log("[BossReaper] Attack ended, ready for next attack");
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;
        
        // Set moving parameter based on whether we're moving
        if (HasAnimatorParameter(animator, "Moving"))
        {
            bool isMoving = rb != null && Mathf.Abs(rb.linearVelocity.x) > 0.1f;
            animator.SetBool("Moving", isMoving);
        }
    }

    private void LookAtPlayer()
    {
        if (player == null) return;
        
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;
        
        if (transform.position.x > player.position.x && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if (transform.position.x < player.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }
    
    // Called by ReaperBossTrigger or Boss_Health.BossActive()
    public void ActivateBoss()
    {
        IsActive = true;
        Debug.Log("[BossReaper] Boss activated!");
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBossMove();
        }
    }
    
    private bool HasAnimatorParameter(Animator anim, string paramName)
    {
        if (anim == null) return false;
        
        foreach (AnimatorControllerParameter param in anim.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }
    
    // Visualize ranges in Scene view
    void OnDrawGizmosSelected()
    {
        // Chase range (yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        
        // Attack range (red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Ground check (green)
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        
        // Show distance if player exists
        if (player != null)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            Gizmos.color = dist <= attackRange ? Color.red : Color.yellow;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}