using UnityEngine;

[CreateAssetMenu(menuName = "Data/AttackData/Light")]
public class AttackData : ScriptableObject {
    [Header("Timing (seconds)")]
    public float startup = 0.06f;   // wind-up (not hitting)
    public float active  = 0.08f;   // can hit
    public float recovery= 0.12f;   // end lag

    [Header("Damage/Force")]
    public int damage = 10;
    public float hitstun = 0.12f;
    public Vector2 knockback = new Vector2(6f, 3f); // units/sec impulse

    [Header("Hitbox")]
    public Vector2 boxSize = new Vector2(1.1f, 0.6f);
    public Vector2 localOffset = new Vector2(0.8f, 0.1f); // from the player's center
    public LayerMask targets; // Enemy layer(s)
    public int maxHitsPerSwing = 1; // prevent multi-hits per target
}
