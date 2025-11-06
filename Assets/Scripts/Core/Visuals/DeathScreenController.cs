using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; // only if you’re using TextMeshPro

public class DeathScreenController : MonoBehaviour
{
    public Image fadePanel;
    public TextMeshProUGUI deathText; // use Text if not TMP
    public float fadeDuration = 1.5f;

    private void Awake()
    {
        // Ensure the panel and text start invisible
        Color panelColor = fadePanel.color;
        panelColor.a = 0;
        fadePanel.color = panelColor;

        Color textColor = deathText.color;
        textColor.a = 0;
        deathText.color = textColor;
    }

    public IEnumerator FadeToBlack()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.SmoothStep(0f, 1f, elapsed / fadeDuration);

            fadePanel.color = new Color(0, 0, 0, alpha);
            deathText.color = new Color(deathText.color.r, deathText.color.g, deathText.color.b, alpha);

            yield return null;
        }
    }
    public IEnumerator FadeFromBlack()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - Mathf.SmoothStep(0f, 1f, elapsed / fadeDuration);

            fadePanel.color = new Color(0, 0, 0, alpha);
            deathText.color = new Color(deathText.color.r, deathText.color.g, deathText.color.b, alpha);

            yield return null;
        }

        // Make sure they’re completely invisible at the end
        Color panelColor = fadePanel.color;
        panelColor.a = 0;
        fadePanel.color = panelColor;

        Color textColor = deathText.color;
        textColor.a = 0;
        deathText.color = textColor;
    }
}