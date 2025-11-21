using UnityEngine;

public class Boss : MonoBehaviour
{   
    [Header("References")]
    public Transform player;
    private Animator animator;

    [Header("Flip Settings")]
    public bool isFlipped = false;

    [Header("Behavior Settings")]
    public float chaseRange = 5f;
    public float attackRange = 1.5f;
    public bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        // Auto-find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("[Boss] Auto-found player");
            }
            else
            {
                Debug.LogError("[Boss] Could not find player! Make sure player has 'Player' tag.");
            }
        }
    }

    void Update()
    {
        if (player == null || animator == null || isDead) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Walking logic
        bool shouldWalk = distance <= chaseRange && distance > attackRange;
        animator.SetBool("Moving", shouldWalk);

        // Attack logic
        if (distance <= attackRange)
        {
            animator.SetTrigger("Attack");
        }

        // Death logic (example trigger)
        if (/* your health logic here */ false)
        {
            isDead = true;
            animator.SetBool("IsDead", true);
        }
    }

    // Called by Boss_Walking state machine behavior
    public void lookAtPlayer()
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
}