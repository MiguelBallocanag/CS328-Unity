using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UndeadSentinelChase : MonoBehaviour
{
    [Header("animator")]
    private Animator animator;
    [Header("Chase Settings")]
    public float chaseSpeed = 3.5f;
    public Transform startingPoint;

    [Header("Status")]
    private GameObject player;
    public bool isChasing = false;

    private Rigidbody2D rb;
    private bool isDead = false;
    private Vector2 previousPosition;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        previousPosition = transform.position;
    }

    void Update()
    {
        if (player == null || isDead)
            return;

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            ReturnToStartingPoint();
        }

        UpdateAnimation();
        Flip();
        previousPosition = transform.position;
    }

    private void ChasePlayer()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, chaseSpeed * Time.deltaTime);
    }

    private void ReturnToStartingPoint()
    {
        if (startingPoint != null)
            transform.position = Vector2.MoveTowards(transform.position, startingPoint.position, chaseSpeed * Time.deltaTime);
    }

    private void UpdateAnimation()
    {
        float distanceMoved = Vector2.Distance(previousPosition, transform.position);
        bool isMoving = distanceMoved > 0.01f;

        if (animator != null)
        {
            animator.SetBool("IsWalking", isMoving);
        }
    }

    private void Flip()
    {
        if (player == null)
            return;
        if (transform.position.x > player.transform.position.x)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    // Called by EnemyHealth when this enemy dies
    public void OnDeath()
    {
        isDead = true;
        if(animator != null)
            animator.SetBool("IsWalking", false);

        this.enabled = false;
    }
}