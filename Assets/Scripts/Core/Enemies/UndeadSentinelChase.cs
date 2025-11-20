using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UndeadSentinelChase : MonoBehaviour
{
    [Header("Chase Settings")]
    public float chaseSpeed = 3.5f;
    public Transform startingPoint;

    [Header("Status")]
    private GameObject player;
    public bool isChasing = false;

    private Rigidbody2D rb;
    private bool isDead = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
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
        Flip();
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

    private void Flip()
    {
        if (player == null)
            return;
        transform.rotation = player.transform.position.x < transform.position.x ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
    }

    // Called by EnemyHealth when this enemy dies
    public void OnDeath()
    {
        isDead = true;
        this.enabled = false;
    }
}