using UnityEngine;

public sealed class Anim {
    private readonly Animator a;

    static readonly int P_Speed    = Animator.StringToHash("Speed");
    static readonly int P_Grounded = Animator.StringToHash("Grounded");
    static readonly int P_YVel     = Animator.StringToHash("YVel");

    static readonly int P_Jump     = Animator.StringToHash("JumpTrig");
    static readonly int P_Dash     = Animator.StringToHash("DashTrig");
    static readonly int P_Attack   = Animator.StringToHash("AttackTrig");

    static readonly int S_Attack   = Animator.StringToHash("Attack");

    public Anim(Animator animator) { a = animator; }

    public void SetGrounded(bool v) { if (a) a.SetBool(P_Grounded, v); }
    public void SetSpeed(float v)   { if (a) a.SetFloat(P_Speed, v); }
    public void SetYVel(float v)    { if (a) a.SetFloat(P_YVel, v); }

    public void Jump()      { if (a) a.SetTrigger(P_Jump); }
    public void Dash()      { if (a) a.SetTrigger(P_Dash); }
    public void AttackTrigger() { if (a) a.SetTrigger(P_Attack); }

    public bool IsInAttack() {
        if (!a) return false;
        var st = a.GetCurrentAnimatorStateInfo(0);
        return st.shortNameHash == S_Attack;
    }

    public void PlayAttackFromStart() {
        if (a) a.Play(S_Attack, 0, 0f);
    }
}
