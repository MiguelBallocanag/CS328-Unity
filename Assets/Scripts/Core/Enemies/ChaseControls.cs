using UnityEngine;

public class ChaseControls : MonoBehaviour
{
    public FlyingEnemy[] enemyArray;
    public UndeadSentinelChase[] sentinelArray;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (FlyingEnemy enemy in enemyArray)
            {
                enemy.isChasing = true;
            }
            foreach (UndeadSentinelChase sentinel in sentinelArray)
            {
                sentinel.isChasing = true;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (FlyingEnemy enemy in enemyArray)
            {
                enemy.isChasing = false;
            }
            foreach (UndeadSentinelChase sentinel in sentinelArray)
            {
                sentinel.isChasing = false;
            }
        }
    }


}
