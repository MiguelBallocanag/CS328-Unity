using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FallingSpikes : MonoBehaviour
{
    [Header("Falling Spikes References")]
    public Rigidbody2D rb;
    public Collider2D damageCollider;


    [Header("Falling Spikes Settings")]
    public float fallDelay = 0.5f;
    public float destroyAfterSeconds = 5f;
    public layerMask playerLayer;

    bool hasFallen = false;
    // Start is called before the first frame update
    void Awake() 
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();//   Get the Rigidbody2D component
        if (damageCollider == null)//   If no specific damage collider is assigned
            damageCollider = GetComponent<Collider2D>();// Assuming the damage collider is the same as the main collider

        rb.bodyType = RigidbodyType2D.Kinematic;// Start as kinematic
        rb.gravityScale = 0f;// No gravity initially
    }
    // Method to make the spikes fall
    void OnTriggerEnter2D(Collider2D other) 
    {
        if(hasFallen)
            return;
        if (((1 << other.gameObject.layer) & playerLayer) != 0) 
        {
            hasFallen = true;
            invoke(nameof(Fall), fallDelay);// Invoke the Fall method after the specified delay
        }// Check if the colliding object is in the player layer
    }
    void Fall() 
    {
        rb.bodyType = RigidbodyType2D.Dynamic;// Change to dynamic to enable gravity
        rb.gravityScale = 1f;// Enable gravity
        if(destroyAfterSeconds > 0f)
            Destroy(gameObject, destroyAfterSeconds);// Destroy the spikes after a set time
    }

}
