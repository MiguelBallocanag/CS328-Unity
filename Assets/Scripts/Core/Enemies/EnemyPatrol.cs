using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    [Header("Enemy")]
    [SerializeField] private Transform enemy;

    [Header("Movement parameters")]
    [SerializeField] private float speed = 2f;
    private Vector3 initScale;
    private bool movingLeft;
    private Rigidbody2D rb;

    [Header("Idle Behavior")]
    [SerializeField] private float idleDuration = 2f;
    private float idleTimer;

    [Header("Enemy Animation")]
    [SerializeField] private Animator anim;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        initScale = enemy.localScale;
    }
    
    
    private void OnDisable()
    {
        if(anim != null) { 
            anim.SetBool("Moving", false);
        }
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        
    }

    private void FixedUpdate()
    {
        if (IsPerformingAction())
        {
            anim.SetBool("Moving", false);
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (movingLeft)
        {
            if (enemy.position.x >= leftEdge.position.x)
                MoveInDirections(-1, GetRb());
            else
                DirectionChange();
        }
        else
        {
            if (enemy.position.x <= rightEdge.position.x)
                MoveInDirections(1, GetRb());
            else
                DirectionChange();
        }
    }
    private bool IsPerformingAction()
    {
        var stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsTag("MeleeAttack") || stateInfo.IsTag("EnemyHurt") || stateInfo.IsTag("EnemyDie");
    }

    private void DirectionChange()
    {
        anim.SetBool("Moving", false);
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);


        idleTimer += Time.deltaTime;
        if (idleTimer >= idleDuration)
        {
            movingLeft = !movingLeft;
            idleTimer = 0;
        }
    }

    private Rigidbody2D GetRb()
    {
        return rb;
    }

    private void MoveInDirections(int _direction, Rigidbody2D rb)
    {
        idleTimer = 0;
        anim.SetBool("Moving", true);
        enemy.localScale = new Vector3(Mathf.Abs(initScale.x) * _direction, initScale.y, initScale.z);
        rb.linearVelocity = new Vector2(_direction * speed, rb.linearVelocity.y);

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(leftEdge.transform.position, 0.1f);
        Gizmos.DrawWireSphere(rightEdge.transform.position, 0.1f);
        Gizmos.DrawLine(leftEdge.transform.position, rightEdge.transform.position);
    }

}
