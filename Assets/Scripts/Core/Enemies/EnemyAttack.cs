using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAttack : MonoBehaviour
{
    // Attack parameters
    [Header("Attack Parameters")]
    [SerializeField] private float attackcooldown = 2f;
    [SerializeField] private int attackdamage =10;
    [SerializeField] private float attackrange=0.5f;
    [SerializeField] private Transform attackPoint;

    [Header("Enemy Type Settings")]
    [SerializeField] private bool canBlock = false;
    [SerializeField] private bool isSentinel = false;
    


    // References
    private Animator anim;
    private Transform player;
    private float attackTime;

    [Header("Player Parameters")]
    private float cooldownTimer = Mathf.Infinity;
    [SerializeField] private LayerMask Player;
    //public LayerMask obstaclelayer;
  

    private EnemyPatrol enemyPatrol;
    private bool isDead = false;

    // Animator parameters mapping
    private string paramAttack;
    private string paramMove;
    private string paramBlock;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyPatrol = GetComponent<EnemyPatrol>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (isSentinel)
        {
            // Map Sentinel-specific animator parameters
            paramAttack = "swordAttack";
            paramMove = "IsWalking";
            paramBlock = "IsBlocking";//sentinel block parameter only
        }
        else
        { // Map standard enemy animator parameters
            paramAttack = "MeleeAttack";
            paramMove = "Moving";
            paramBlock = "";//no block parameter for standard enemy
        }

    }



    // Update is called once per frame
    void Update()
    {
        if (isDead)
            return;
        cooldownTimer += Time.deltaTime;

        bool inSight = PlayerInSight();

        // Stop patrol ONLY while attacking or preparing to attack
        if (enemyPatrol != null)
            enemyPatrol.enabled = !inSight;

        if (inSight)
        {
            if (canBlock && !string.IsNullOrEmpty(paramBlock))
                TryBlock();

            if (cooldownTimer >= attackcooldown)
            {
                cooldownTimer = 0;
                anim.SetTrigger(paramAttack);
            }
        }
    }    

    // Check if the player is in sight using raycasting
    private bool PlayerInSight()
    {
        
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackrange, Player);
        return hit != null;

    }
    //void Attack()
    //{
    //    // Play attack animation
    //    anim.SetTrigger("MeleeAttack");
    //    Collider2D hit=Physics2D.OverlapCircle(transform.position, attackrange, playerlayer);
        
    //}
    // Deal damage to the player if in range
    public void DamagePlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackrange, Player);
        if (hit !=null)
        {// Deal damage to the player
            PlayerHealth ph = hit.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage(attackdamage);
        }
    }
    // Stop blocking after a short duration
    private void TryBlock()
    {
        if (string.IsNullOrEmpty(paramBlock))
            return;

       
            anim.SetBool(paramBlock, true);
            StartCoroutine(StopBlocking());
        
    }
    public void OnEnemyDeath()
    {
        isDead = true;

        // Stop any running block
        if (!string.IsNullOrEmpty(paramBlock))
            anim.SetBool(paramBlock, false);

        // Reset triggers
        anim.ResetTrigger(paramAttack);

        // Stop AI
        if (enemyPatrol != null)
        {
            enemyPatrol.enabled = false;
        }
        StopAllCoroutines();
        this.enabled = false; // fully disable attacking
    }

    private IEnumerator StopBlocking()
    {
        yield return new WaitForSeconds(1f); // Block for 1 second
        anim.SetBool(paramBlock, false);
    }

    //Visualize the attack range in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackrange);// Draw a wire sphere to represent the attack range
    }
}
