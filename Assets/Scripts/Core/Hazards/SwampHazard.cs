using UnityEngine;

public class SwampHazard : MonoBehaviour
{
    public float slowMultiplier = 0.4f;   // 40% normal speed

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.swampSpeedMult = slowMultiplier;
            Debug.Log("Entered swamp");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.swampSpeedMult = 1f;
            pc.swampJumpLock = false;
            Debug.Log("Exited swamp");
        }
    }
}
