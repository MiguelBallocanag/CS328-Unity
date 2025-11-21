using UnityEngine;

public class Boss_Walking : StateMachineBehaviour
{
    Transform player;
    Rigidbody2D rb;
    Boss boss;
    
    [Header("Movement Settings")]
    public float speed = 2.5f;
    
    [Header("Attack Settings")]
    public float attackRange = 1.5f;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = animator.GetComponent<Boss>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = animator.GetComponent<Rigidbody2D>();
        
        if (player == null)
        {
            Debug.LogError("[Boss_Walking] Could not find Player! Make sure player has 'Player' tag.");
        }
        AudioManager.Instance.PlayBossMove();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null || rb == null) return;
        
        // Move toward player (FIXED: now uses speed variable correctly)
        Vector2 target = new Vector2(player.position.x, rb.position.y);
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
        
        // Look at player
        if (boss != null)
        {
            boss.lookAtPlayer();
        }
        
        // Check if close enough to attack (FIXED: now uses attackRange variable)
        if (Vector2.Distance(player.position, rb.position) <= attackRange)
        {
            animator.SetTrigger("Attack");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
    }
}