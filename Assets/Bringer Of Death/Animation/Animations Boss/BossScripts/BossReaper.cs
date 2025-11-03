using UnityEngine;

public class Boss : MonoBehaviour
{   
 
    // Allow assignment in inspector, but we will also attempt to find the player by tag
    public Transform player;
    public bool isFlipped = false;
   
    // Update is called once per frame
  
    public void lookAtPlayer()
    {
               if (player != null)
        {
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
}
