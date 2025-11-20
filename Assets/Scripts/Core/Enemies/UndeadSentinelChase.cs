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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
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
}