using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAttack : MonoBehaviour
{
    // Attack parameters
    [Header("Attack Parameters")]
    [SerializeField] private float attackcooldown = 2f;
    [SerializeField] private int attackdamage =10;
    [SerializeField] private float attackrange=1f;
   

    // References
    private Animator anim;
    private Transform player;
    private float attackTime;
    [Header("Player Parameters")]
    private float cooldownTimer = Mathf.Infinity;
    [SerializeField] private LayerMask Player;
    //public LayerMask obstaclelayer;
    public PlayerHealth playerHealth;

    private EnemyPatrol enemyPatrol;    



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyPatrol = GetComponent<EnemyPatrol>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

    }



    // Update is called once per frame
    void Update()
    {
       
        cooldownTimer += Time.deltaTime;
        if (enemyPatrol !=null)
   
            enemyPatrol.enabled = !PlayerInSight();

        if (PlayerInSight()) 
        {
            if (cooldownTimer >= attackcooldown)
            {
               
                cooldownTimer = 0;
                anim.SetTrigger("MeleeAttack");
            }
        }
    }
    // Check if the player is in sight using raycasting
    private bool PlayerInSight()
    {
        
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackrange, Player);
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
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackrange, Player);
        if (hit !=null)
        {// Deal damage to the player
            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                player.GetComponent<PlayerHealth>().TakeDamage(attackdamage);
        }
    }
    //Visualize the attack range in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackrange);
    }
}
