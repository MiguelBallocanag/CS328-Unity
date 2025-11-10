using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Parameters")]
    [SerializeField] private float attackcooldown;
    [SerializeField] private int attackdamage;
    [SerializeField] private float attackrange=1f;

    
    private Animator anim;
    private Transform player;
    private float attackTime;
    [Header("Player Parameters")]
    private float attacktimer = Mathf.Infinity;
    public LayerMask playerlayer;
    public LayerMask obstaclelayer;
    //private Health playerHealth;

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
        attackcooldown += Time.deltaTime;
        if( PlayerInSight() )
        {
            if(attackcooldown>=attackTime)
            {
                
                attackTime= 0;
                anim.SetTrigger("MeleeAttack");
            }
        }
        if(enemyPatrol!=null)
        {
            enemyPatrol.enabled = !PlayerInSight();
        }


    }
    // Check if the player is in sight using raycasting
    bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (player.position - transform.position).normalized, attackrange, playerlayer);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
           
            return true;// Player is in sight
        }
        return false;// Something is blocking the view
    }   
    void Attack()
    {
        // Play attack animation
        anim.SetTrigger("MeleeAttack");
        Collider2D hit=Physics2D.OverlapCircle(transform.position, attackrange, playerlayer);
        
    }
    //private void DamagePlayer()
    //{
    //    if(PlayerInSight())
    //    {// Deal damage to the player
    //        player.GetComponent<playerHealth>().TakeDamage(attackdamage);
    //    }   
    //}
    // Visualize the attack range in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackrange);
    }
}
