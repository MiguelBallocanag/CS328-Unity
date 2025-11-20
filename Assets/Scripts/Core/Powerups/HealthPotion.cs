using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    private bool used = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used) return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            used = true;

            health.HealToFull();  // heals player
            gameObject.SetActive(false); // hide potion forever
        }
    }
}
