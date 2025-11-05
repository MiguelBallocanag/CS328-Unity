using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 5;
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Player took damage, current health: " + health);
        if(health <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        //handle player death
        Debug.Log("Player died.");
    } 
}
