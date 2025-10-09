using UnityEngine;

[CreateAssetMenu(menuName="Data/DashData")]
public class DashData : ScriptableObject {
    [Header("Feel")]
    public float speed = 20f;        // burst speed
    public float time  = 0.11f;      // how long the burst lasts
    public float cooldown = 0.14f;   // time before next dash

    [Header("Rules")]
    public bool allowInAir = true;   // air dash?
    public bool lockYDuringDash = true; // set Y to 0 while dashing
}
