using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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

            StartCoroutine(TriggerEndSequence());
        }
    }

    private IEnumerator TriggerEndSequence()
    {
        EndScreenController endScreen = Object.FindFirstObjectByType<EndScreenController>();

        if (endScreen != null)
        {
            endScreen.gameObject.SetActive(true);
            yield return StartCoroutine(endScreen.FadeToBlack());
        }

        // Small delay before loading next level
        yield return new WaitForSeconds(1.5f);

        // LOAD LEVEL 2
        SceneManager.LoadScene("Level2_Cathedral");
    }
}

