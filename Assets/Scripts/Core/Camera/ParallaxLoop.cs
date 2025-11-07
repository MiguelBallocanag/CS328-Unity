using System.Linq;
using UnityEngine;

public class ParallaxCarousel : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;          // Main Camera

    [Header("Parallax")]
    [Range(0f, 1f)] public float parallaxFactor = 0.5f; // 0 = static, 1 = follow camera
    public bool parallaxY = false;

    Transform[] segments;          // children in x-order
    float segWidth;                // width of one segment in world units
    Vector3 startPos, camStart;

    void Awake()
    {
        if (cameraTransform == null) cameraTransform = Camera.main.transform;

        // Get children ordered by x
        segments = GetComponentsInChildren<SpriteRenderer>(true)
                   .Select(sr => sr.transform)
                   .Where(t => t != transform)
                   .OrderBy(t => t.position.x)
                   .ToArray();

        if (segments.Length < 3)
            Debug.LogWarning($"{name}: Need 3 child segments for carousel.");

        // Assume all segments same width; read from first sprite bounds
        var sr = segments[0].GetComponent<SpriteRenderer>();
        segWidth = sr.bounds.size.x;

        startPos = transform.position;
        camStart = cameraTransform.position;
    }

    void LateUpdate()
    {
        if (!cameraTransform) return;

        // --- Parallax motion relative to where we started ---
        Vector3 camDelta = cameraTransform.position - camStart;
        float dx = camDelta.x * parallaxFactor;
        float dy = parallaxY ? camDelta.y * parallaxFactor : 0f;
        transform.position = new Vector3(startPos.x + dx, startPos.y + dy, startPos.z);

        // --- Carousel looping (horizontal only) ---
        // Keep children array up-to-date (left..right)
        segments = segments.OrderBy(t => t.position.x).ToArray();
        Transform left = segments[0];
        Transform mid = segments[1];
        Transform right = segments[2];

        // If camera moved past (towards right) the midpoint of the right segment,
        // move the left segment to the far right.
        float rightEdgeTrigger = right.position.x - segWidth * 0.5f;
        if (cameraTransform.position.x > rightEdgeTrigger)
        {
            left.position = new Vector3(right.position.x + segWidth, left.position.y, left.position.z);
            // rotate array: new order is mid, right, left
            segments = new Transform[] { mid, right, left };
        }

        // If camera moved past (towards left) the midpoint of the left segment,
        // move the right segment to the far left.
        float leftEdgeTrigger = left.position.x + segWidth * 0.5f;
        if (cameraTransform.position.x < leftEdgeTrigger)
        {
            right.position = new Vector3(left.position.x - segWidth, right.position.y, right.position.z);
            // rotate array: new order is right, left, mid
            segments = new Transform[] { right, left, mid };
        }
    }
}
