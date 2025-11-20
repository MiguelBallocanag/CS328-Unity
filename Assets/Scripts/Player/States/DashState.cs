using UnityEngine;

public class DashState : BaseState {
    float _t;
    float _originalGrav;
    Vector2 _dir;

    public override void Enter(PlayerController p)
    {
        base.Enter(p);


        // decide dash direction
        if (pc.IsGrounded)
        {
            float x = pc.moveInput.x;
            if (Mathf.Abs(x) < 0.1f) x = pc.rb.linearVelocity.x >= 0 ? 1f : -1f; // face/vel fallback
            _dir = new Vector2(Mathf.Sign(x), 0f);
        }
        else
        {
            // air: use stick if any, else face/vel
            Vector2 inDir = pc.moveInput.sqrMagnitude > 0.01f ? pc.moveInput.normalized

                           : new Vector2(Mathf.Sign(pc.rb.linearVelocity.x == 0 ? 1 : pc.rb.linearVelocity.x), 0f);
            // If you want air dash to be mostly horizontal, clamp Y a bit:
            // inDir.y = Mathf.Clamp(inDir.y, -0.6f, 0.6f);
            _dir = inDir;
        }

        _t = pc.dashData.time;

        _originalGrav = pc.rb.gravityScale;
        if (pc.dashData.lockYDuringDash) pc.rb.gravityScale = 0f;

        // set burst velocity
        Vector2 v = _dir * pc.dashData.speed;
        if (pc.dashData.lockYDuringDash) v.y = 0f;
        pc.rb.linearVelocity = v;

        // start cooldown
        pc.jumpPressed = false; // optional: prevent accidental jump during dash frame
        pc.lastPressedJumpTime = 0f;

        pc.ConsumeAirDashIfNeeded();
        pc.StartDashCooldown();
        pc.RegisterDashForStreak();
        p.Anim_Dash();
        pc.dashPressed = false;
    }

    public override void Tick() {
        // FIXED: Use fixedDeltaTime for frame-rate independent physics
        _t -= Time.fixedDeltaTime;

        // keep dash velocity stable while active (prevents air drag shaping it)
        if (_t > 0f) {
            Vector2 v = _dir * pc.dashData.speed;
            if (pc.dashData.lockYDuringDash) v.y = 0f;
            pc.rb.linearVelocity = new Vector2(v.x, pc.dashData.lockYDuringDash ? v.y : pc.rb.linearVelocity.y);
            return;
        }

        // dash ended -> go to appropriate state
        if (pc.IsGrounded) {
            if (Mathf.Abs(pc.moveInput.x) < 0.1f) pc.SwitchState(new IdleState());
            else pc.SwitchState(new RunState());
        } else {
            // choose JumpState vs FallState by vertical sign
            if (pc.rb.linearVelocity.y > 0f) pc.SwitchState(new JumpState());
            else pc.SwitchState(new FallState());
        }
    }

    public override void Exit() {
        pc.rb.gravityScale = _originalGrav;

        var v = pc.rb.linearVelocity;
                      
        v.x *= 0.15f;                 
  
        pc.rb.linearVelocity = v;
        pc.justDashed = true;
    }
}
