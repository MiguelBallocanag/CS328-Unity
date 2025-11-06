using UnityEngine;

public class CameraZoneTrigger : MonoBehaviour
{
    public CameraFollow cameraFollow;  // drag your Main Camera here
    public float targetYOffset = 3f;
    public bool revertOnExit = false;
    public float transitionTime = 1f;

    private float originalYOffset;

    void Start()
    {
        if (cameraFollow != null)
            originalYOffset = cameraFollow.yOffset;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && cameraFollow != null)
        {
            cameraFollow.StartCoroutine(cameraFollow.SmoothYOffset(targetYOffset, transitionTime));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (revertOnExit && other.CompareTag("Player") && cameraFollow != null)
        {
            cameraFollow.StartCoroutine(cameraFollow.SmoothYOffset(originalYOffset, transitionTime));
        }
    }
}