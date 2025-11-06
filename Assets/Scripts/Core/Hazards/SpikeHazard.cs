using UnityEngine;

public class SpikeHazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit spikes!");
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                //playerHealth.Die();
                playerHealth.TakeDamage(playerHealth.maxHealth);
            }
        }
    }
}
