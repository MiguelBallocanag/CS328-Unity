using UnityEngine;

[CreateAssetMenu(menuName="Data/DashData")]
public class DashData : ScriptableObject {
    [Header("Feel")]
    public float speed = 7.5f;        // burst speed
    public float time  = 0.06f;      // how long the burst lasts
    public float cooldown = 0.28f;   // time before next dash

    [Header("Rules")]
    public bool allowInAir = true;   // air dash?
    public bool lockYDuringDash = true; // set Y to 0 while dashing

    [Header("Streak")]
    public int streakLimit = 2;
    public float streakWindow = 0.5f;
    public float postStreakLockout = 0.8f;

}
