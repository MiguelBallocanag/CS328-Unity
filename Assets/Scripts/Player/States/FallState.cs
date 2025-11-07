using UnityEngine;

public class FallState : AirState
{
    public override void Enter(PlayerController p)
    {
        base.Enter(p);
    }

    public override void Tick()
    {
        // Platform Snap Assist: only while falling and not moving too fast downward
        if (pc.enablePlatformSnap && pc.rb.linearVelocity.y <= 0f && pc.rb.linearVelocity.y >= pc.snapVYThreshold)
        {
            var hit = Physics2D.Raycast(pc.groundCheck.position, Vector2.down, pc.snapProbe, pc.groundMask);
            if (hit.collider && Vector2.Angle(hit.normal, Vector2.up) <= pc.snapMaxSlope)
            {
                // Snap onto platform top (preserve horizontal)
                float desiredY = hit.point.y + pc.groundCheckRadius;
                pc.rb.position = new Vector2(pc.rb.position.x, desiredY);
                pc.rb.linearVelocity = new Vector2(pc.rb.linearVelocity.x, 0f);

                // Land now
                pc.StartLandingGrace();
                if (Mathf.Abs(pc.moveInput.x) < 0.1f) pc.SwitchState(new IdleState());
                else pc.SwitchState(new RunState());
                return;
            }
        }

        // Allow attack/dash
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

        // Cap rise (shouldn't happen in fall, but keep symmetric)
        if (pc.rb.linearVelocity.y > pc.JumpVelocity)
            pc.rb.linearVelocity = new Vector2(pc.rb.linearVelocity.x, pc.JumpVelocity);

        // Buffered air jump
        if (!pc.IsGrounded && pc.lastPressedJumpTime > 0f && pc.airJumpsLeft > 0)
        {
            pc.airJumpsLeft--;
            pc.SwitchState(new JumpState());
            return;
        }

        // Landed normally
        if (pc.IsGrounded)
        {
            pc.StartLandingGrace();
            if (Mathf.Abs(pc.moveInput.x) < 0.1f) pc.SwitchState(new IdleState());
            else pc.SwitchState(new RunState());
        }
    }
}
