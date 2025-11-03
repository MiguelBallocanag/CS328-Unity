using UnityEngine;

[CreateAssetMenu(menuName="Data/JumpData")]
public class JumpData : ScriptableObject {
    [Header("Heights & Timing")]
    public float jumpHeight = 4f;
    public float timeToApex = 0.28f;

    [Header("Feel")]
    public float earlyCutMult = 2.6f;   // higher = shorter short-hop
    public float fallGravityMult = 1.15f;
    public float fastFallMult = 1.7f;   // when holding Down

    [Header("Buffers")]
    public float coyoteTime = 0.10f;
    public float jumpBuffer = 0.10f;

    [Header("Air Jumps")]
    public int maxAirJumps = 1;         // 1 = double-jump

    // derived (computed at runtime)
    [HideInInspector] public float gravity; 
    [HideInInspector] public float jumpVelocity;
    public void Compute() {
        gravity = (2f * jumpHeight) / (timeToApex * timeToApex);
        jumpVelocity = gravity * timeToApex;
    }
}
