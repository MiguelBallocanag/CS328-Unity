using UnityEngine;

[CreateAssetMenu(menuName="Data/JumpData")]
public class JumpData : ScriptableObject {
    [Header("Heights & Timing")]
    public float jumpHeight = 1.3f;
    public float timeToApex = .5f;

    [Header("Feel")]
    public float earlyCutMult = 2.6f;   // higher = shorter short-hop
    public float fallGravityMult = 1.1f;
    public float fastFallMult = 1.6f;   // when holding Down

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
