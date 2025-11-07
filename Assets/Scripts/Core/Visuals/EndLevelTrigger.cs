using UnityEngine;

public class EndLevelTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            Debug.Log("Level Complete!");

            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.StartCoroutine(TriggerEndScreen(playerHealth));
            }
        }
    }

    private System.Collections.IEnumerator TriggerEndScreen(PlayerHealth playerHealth)
    {
        EndScreenController endScreen = Object.FindFirstObjectByType<EndScreenController>();
        if (endScreen != null)
        {
            endScreen.gameObject.SetActive(true);
            yield return playerHealth.StartCoroutine(endScreen.FadeToBlack());
            yield return new WaitForSeconds(2f);

            // Optional: respawn player at start
            playerHealth.Respawn();

            yield return playerHealth.StartCoroutine(endScreen.FadeFromBlack());
            endScreen.gameObject.SetActive(false);
        }

        hasTriggered = false;
    }
}