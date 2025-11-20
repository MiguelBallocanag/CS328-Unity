using UnityEngine;

public class SwampHazard : MonoBehaviour
{
    public float slowMultiplier = 0.4f;
    public float jumpMultiplier = 0.6f;  // <--- NEW

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.swampSpeedMult = slowMultiplier;
            pc.swampJumpMult = jumpMultiplier;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.swampSpeedMult = 1f;
            pc.swampJumpMult = 1f;
        }
    }
}


