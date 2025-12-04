using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Needed for UI elements like Image

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public Image healthBarFill; // Drag your HealthBarFill image here in the Inspector
    public PlayerController playerController; // Assign your player movement script here

    private bool isDead = false; // Prevents multiple death triggers
    private bool isInvincible = false; // For future invincibility power-ups
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void SetInvincibility(bool invincible)
    {
        isInvincible = invincible;
    }
    public void TakeDamage(int damage)
    {
        if (isInvincible) return; // Don't take damage if invincible
        if (isDead) return; // Don't take damage if already dead

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
        AudioManager.Instance.PlayPlayerHurt();
       
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
    }

    public void Die()
    {
        if (isDead) return; // Avoid double death calls
        isDead = true;

        Debug.Log("Player Died!");
        StartCoroutine(HandleDeath());
        AudioManager.Instance.PlayPlayerDeath();
    }

    public void Respawn()
    {
        // Reload the current scene to fully reset enemies, hazards, etc.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator HandleDeath()
    {
        if (playerController != null)
            playerController.enabled = false;

        // Get the death screen
        DeathScreenController deathScreen = Object.FindFirstObjectByType<DeathScreenController>();
        if (deathScreen != null)
        {
            deathScreen.gameObject.SetActive(true);
            yield return StartCoroutine(deathScreen.FadeToBlack());
        }

        yield return new WaitForSeconds(1.5f);
        Respawn();

        if (deathScreen != null)
        {
            yield return StartCoroutine(deathScreen.FadeFromBlack());
            deathScreen.gameObject.SetActive(false);
        }

        if (playerController != null)
            playerController.enabled = true;

        // Reset death state
        isDead = false;
    }
    public void HealToFull()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }
}