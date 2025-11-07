using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public bool lockY = true;
    private float fixedY;

    [Header("Vertical Offset Control")]
    public float yOffset = 0f;            // Base offset from player or fixedY
    public float smoothSpeed = 3f;        // How quickly camera moves to new position

    void Start()
    {
        fixedY = transform.position.y;
    }

    void LateUpdate()
    {
        float targetY = lockY ? fixedY + yOffset : player.position.y + yOffset;
        Vector3 targetPos = new Vector3(player.position.x, targetY, transform.position.z);

        // Smooth follow (so camera movement feels natural)
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);
    }

    //Allows triggers to change vertical offset instantly
    public void SetYOffset(float newOffset)
    {
        yOffset = newOffset;
    }

    //Allows triggers to change vertical offset smoothly over time
    public IEnumerator SmoothYOffset(float targetOffset, float duration = 1f)
    {
        float startOffset = yOffset;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yOffset = Mathf.Lerp(startOffset, targetOffset, elapsed / duration);
            yield return null;
        }

        yOffset = targetOffset;
    }
}
