using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EndScreenController : MonoBehaviour
{
    public Image fadePanel;
    public TextMeshProUGUI endText;
    public float fadeDuration = 1.5f;

    private void Awake()
    {
        SetAlpha(0f);
    }

    void SetAlpha(float a)
    {
        if (fadePanel != null)
            fadePanel.color = new Color(0f, 0f, 0f, a);
        if (endText != null)
            endText.color = new Color(endText.color.r, endText.color.g, endText.color.b, a);
    }

    public IEnumerator FadeToBlack()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.SmoothStep(0f, 1f, elapsed / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(1f);
    }

    public IEnumerator FadeFromBlack()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - Mathf.SmoothStep(0f, 1f, elapsed / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(0f);
    }
}