using UnityEngine;

public class FallState : AirState
{
    public override void Enter(PlayerController p)
    {
        base.Enter(p);
        // If we just walked off a ledge, grant air jumps
        
    }

    public override void Tick()
    {
        if (pc.dashPressed && pc.CanDashNow)
        {
            pc.SwitchState(new DashState());
            return;
        }
        ApplyAirMotion();
        ApplyHeavierFall();

        // Air jump from fall (double jump)
        if (!pc.IsGrounded && pc.lastPressedJumpTime > 0f && pc.airJumpsLeft > 0)
        {
            pc.airJumpsLeft--;
            pc.SwitchState(new JumpState()); // JumpState.DoJump() runs in Enter
            return;
        }

        // Landed → go to Idle/Run based on input
        if (pc.IsGrounded)
        {
            if (Mathf.Abs(pc.moveInput.x) < 0.1f) pc.SwitchState(new IdleState());
            else pc.SwitchState(new RunState());
        }
        if (pc.attackPressed && pc.lightAttack != null) {
        pc.SwitchState(new AttackLightState(pc.lightAttack));
        return;
}

    }
}
