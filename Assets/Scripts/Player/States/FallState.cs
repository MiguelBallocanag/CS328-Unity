using UnityEngine;

public class FallState : AirState
{
    public override void Enter(PlayerController p)
    {
        base.Enter(p);
    }

    public override void Tick()
    {
        if ((pc.attackPressed || pc.AttackBuffered) && pc.lightAttack != null) {
            pc.ConsumeAttackBuffer();
            pc.SwitchState(new AttackLightState(pc.lightAttack));
            return;
        }

        if (pc.dashPressed && pc.CanDashNow)
        {
            pc.SwitchState(new DashState());
            return;
        }

        ApplyAirMotion();
        ApplyBaseGravity();
        ApplyHeavierFall();

        if (pc.rb.linearVelocity.y > pc.JumpVelocity)
            pc.rb.linearVelocity = new Vector2(pc.rb.linearVelocity.x, pc.JumpVelocity);

        if (!pc.IsGrounded && pc.lastPressedJumpTime > 0f && pc.airJumpsLeft > 0)
        {
            pc.airJumpsLeft--;
            pc.SwitchState(new JumpState());
            return;
        }

        if (pc.IsGrounded)
        {
            if (Mathf.Abs(pc.moveInput.x) < 0.1f) pc.SwitchState(new IdleState());
            else pc.SwitchState(new RunState());
        }
    }
}
