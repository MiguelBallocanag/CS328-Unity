using UnityEngine;

public class JumpState : AirState
{
    public override void Enter(PlayerController p)
    {
        base.Enter(p);
        p.Anim_Jump();
        DoJump();
    }

    public override void Tick()
    {
        if ((pc.attackPressed || pc.AttackBuffered) && pc.lightAttack != null) {
            pc.ConsumeAttackBuffer();
            pc.SwitchState(new AttackLightState(pc.lightAttack));
            return;
        }

        if (pc.dashPressed && pc.CanDashNow) { pc.SwitchState(new DashState()); return; }


        ApplyAirMotion();
        ApplyBaseGravity();
        ApplyEarlyCutIfNeeded();
        ApplyHeavierFall();

        if (pc.rb.linearVelocity.y > pc.JumpVelocity)
            pc.rb.linearVelocity = new Vector2(pc.rb.linearVelocity.x, pc.JumpVelocity);

        if (!pc.IsGrounded && pc.lastPressedJumpTime > 0f && pc.airJumpsLeft > 0)
        {
            pc.airJumpsLeft--;
            DoJump();
        }

        if (Falling) pc.SwitchState(new FallState());
    }
}
