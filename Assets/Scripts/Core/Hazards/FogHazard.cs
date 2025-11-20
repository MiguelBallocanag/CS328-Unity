using UnityEngine;

public class FogHazard : MonoBehaviour
{
    public SpriteRenderer fogSprite; // assign the Fog SpriteRenderer here
    public float fogAlpha = 0.7f;    // target alpha when fog is active
    public float fadeSpeed = 2f;     // how fast it fades

    float currentTargetAlpha = 0f;   // start invisible

    void Start()
    {
        // Force fog to be invisible at start
        if (fogSprite != null)
        {
            Color c = fogSprite.color;
            c.a = 0f;
            fogSprite.color = c;
        }
    }

    void Update()
    {
        if (fogSprite != null)
        {
            Color c = fogSprite.color;

            // cleaner fade logic:
            c.a = Mathf.MoveTowards(c.a, currentTargetAlpha, Time.deltaTime * fadeSpeed);

            fogSprite.color = c;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>())
        {
            currentTargetAlpha = fogAlpha;  // fade IN
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerController>())
        {
            currentTargetAlpha = 0f;        // fade OUT
        }
    }
}

