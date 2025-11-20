using UnityEngine;

public class JumpState : AirState
{
    float _riseTime;

    public override void Enter(PlayerController p)
    {
        base.Enter(p);
        _riseTime = 0f;
        p.Anim_Jump();
        DoJump();
    }

    public override void Tick()
    {
        _riseTime += Time.deltaTime;

        // allow attack during jump
        if ((pc.attackPressed || pc.AttackBuffered) && pc.lightAttack != null) {
            pc.ConsumeAttackBuffer();
            pc.SwitchState(new AttackLightState(pc.lightAttack));
            return;
        }

        // allow dash during jump
        if (pc.dashPressed && pc.CanDashNow) {
            pc.SwitchState(new DashState());
            return;
        }

        // standard aerial movement
        ApplyAirMotion();
        
        // Apply gravity with early cut for variable jump height
        ApplyBaseGravity();
        ApplyEarlyCutIfNeeded();

        // Prevent velocity from exceeding initial jump velocity (fixes off-screen launch bug)
        // This caps upward velocity to the jump velocity, preventing physics errors
        if (pc.rb.linearVelocity.y > pc.JumpVelocity)
        {
            pc.rb.linearVelocity = new Vector2(pc.rb.linearVelocity.x, pc.JumpVelocity);
        }

        // heavier fall / fast fall when descending
        ApplyHeavierFall();

        // buffered air jump
        if (!pc.IsGrounded && pc.lastPressedJumpTime > 0f && pc.airJumpsLeft > 0)
        {
            pc.airJumpsLeft--;
            DoJump();
        }

        // transition to fall once velocity <= 0 or gravity overtakes rise
        if (Falling)
            pc.SwitchState(new FallState());
    }
}