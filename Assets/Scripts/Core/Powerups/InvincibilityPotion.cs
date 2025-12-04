using UnityEngine;
using System.Collections;

public class InvincibilityPowerup : MonoBehaviour
{
    [Header("Powerup Settings")]
    public float duration = 5f;
    public float flashInterval = 0.1f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        SpriteRenderer spriteRenderer = other.GetComponent<SpriteRenderer>();

        if (playerHealth != null && spriteRenderer != null)
        {
            StartCoroutine(ApplyInvincibility(playerHealth, spriteRenderer));
        }

        Destroy(gameObject);
    }

    IEnumerator ApplyInvincibility(PlayerHealth playerHealth, SpriteRenderer spriteRenderer)
    {
        playerHealth.SetInvincibility(true);  // Correct method name

        float elapsed = 0f;
        while (elapsed < duration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            elapsed += flashInterval;
            yield return new WaitForSeconds(flashInterval);
        }

        spriteRenderer.enabled = true;
        playerHealth.SetInvincibility(false);  // Correct method name
    }
}

