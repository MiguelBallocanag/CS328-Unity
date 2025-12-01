using UnityEngine;

public class AcidDamage : MonoBehaviour
{
    [SerializeField] int damagePerTick = 5;
    [SerializeField] float tickInterval = 1.0f;

    float timer;
    bool isPlayerInAcid=false;
    PlayerHealth playerHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInAcid = true;
            playerHealth = other.GetComponent<PlayerHealth>();
            timer = 0f; // Reset timer when player enters acid
        }

    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInAcid = false;
            playerHealth = null;
        }
    }

    // Update is called once per frame
    void Update()
    { 
        if(!isPlayerInAcid || playerHealth == null)
            return;
        timer += Time.deltaTime;

        if (timer >= tickInterval)
        {
            playerHealth.TakeDamage(damagePerTick);
            timer = 0f; // Reset timer after applying damage
        }

    }
}
