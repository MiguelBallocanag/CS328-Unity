using UnityEngine;

public class Boss : MonoBehaviour
{   
    [Header("References")]
    public Transform player;
    
    [Header("Flip Settings")]
    public bool isFlipped = false;

    void Start()
    {
        // Auto-find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("[Boss] Auto-found player");
            }
            else
            {
                Debug.LogError("[Boss] Could not find player! Make sure player has 'Player' tag.");
            }
        }
    }

    // Called by Boss_Walking state machine behavior
    public void lookAtPlayer()
    {
        if (player == null) return;
        
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;
        
        if (transform.position.x > player.position.x && isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = false;
        }
        else if (transform.position.x < player.position.x && !isFlipped)
        {
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            isFlipped = true;
        }
    }
}