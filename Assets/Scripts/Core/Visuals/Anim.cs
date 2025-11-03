using UnityEngine;

/// <summary>
/// Centralized animator wrapper (Facade) for the Player.
/// Keeps Animator parameter hashes + trigger calls in one place.
/// </summary>
public sealed class Anim
{
    private readonly Animator a;

    // Precompute Animator parameter hashes
    static readonly int P_Speed = Animator.StringToHash("Speed");
    static readonly int P_Grounded = Animator.StringToHash("Grounded");
    static readonly int P_YVel = Animator.StringToHash("YVel");
    static readonly int P_Jump = Animator.StringToHash("JumpTrig");
    static readonly int P_Dash = Animator.StringToHash("DashTrig");
    static readonly int P_Attack = Animator.StringToHash("AttackTrig");


    public Anim(Animator animator)
    {
        a = animator;
    }

    // --- Parameter Setters ---
    public void SetGrounded(bool v) { if (a) a.SetBool(P_Grounded, v); }
    public void SetSpeed(float v) { if (a) a.SetFloat(P_Speed, v); }
    public void SetYVel(float v) { if (a) a.SetFloat(P_YVel, v); }

    // --- One-off triggers ---
    public void Jump() { if (a) a.SetTrigger(P_Jump); }
    public void Dash() { if (a) a.SetTrigger(P_Dash); }
    
    public void Attack() { if (a) a.SetTrigger(P_Attack); }

}
