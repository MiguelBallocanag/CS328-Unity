using UnityEngine;

[CreateAssetMenu(menuName="Data/JumpData")]
public class JumpData : ScriptableObject {
    [Header("Heights & Timing")]
    public float jumpHeight = 0.5f;
    public float timeToApex = 0.48f;

    [Header("Jump Power")]
    [Tooltip("Multiplier for jump velocity (0.8 = 80% jump power). Use this to fine-tune jump feel.")]
    [Range(0.1f, 2.0f)]
    public float jumpPowerMultiplier = 0.8f;

    [Header("Feel")]
    public float earlyCutMult = 4.2f;
    public float fallGravityMult = 1.05f;
    public float fastFallMult = 1.28f;

    [Header("Buffers")]
    public float coyoteTime = 0.12f;
    public float jumpBuffer = 0.12f;

    [Header("Air Jumps")]
    public int maxAirJumps = 0;

    [Header("Held Cap")]
    public float heldCutTime = 0.18f; // after this many seconds, force-cut even if held

    [HideInInspector] public float gravity;
    [HideInInspector] public float jumpVelocity;
    
    public void Compute() {
        gravity = 2f * jumpHeight / (timeToApex * timeToApex);
        jumpVelocity = gravity * timeToApex * jumpPowerMultiplier; // Apply multiplier here
    }
}
