using UnityEngine;
using UnityEngine.InputSystem;

public abstract class AirState : BaseState
{
    protected bool Falling => pc.rb.linearVelocity.y <= 0f;

    protected void DoJump()
    {
        pc.lastPressedJumpTime = 0f;
        pc.lastOnGroundTime = 0f;

        pc.rb.linearVelocity = new Vector2(pc.rb.linearVelocity.x, pc.JumpVelocity);

    }
    protected void ApplyEarlyCutIfNeeded()
    {
        bool jumpHeld = (Keyboard.current?.spaceKey.isPressed ?? false) || (Gamepad.current?.aButton.isPressed ?? false);

        if (!jumpHeld && pc.rb.linearVelocity.y > 0f)
        {
            pc.rb.linearVelocity += Vector2.up * (-pc.Gravity * pc.jumpData.earlyCutMult * Time.deltaTime);

        }
    }

    protected void ApplyHeavierFall()
    {
        if (pc.rb.linearVelocity.y < 0f)
        {
            float mult = pc.jumpData.fallGravityMult;
            if (pc.moveInput.y < -0.5f) mult *= pc.jumpData.fastFallMult;//fast fall when holding down
            pc.rb.linearVelocity += Vector2.up * (-pc.Gravity * (mult - 1f) * Time.deltaTime);
        }
    }

    protected void ApplyAirMotion()
    {
        float x = pc.moveInput.x;
        float vx = pc.rb.linearVelocity.x;
        float target = x * Mathf.Max(pc.runSpeed, pc.maxAirSpeed); // pick your cap

        float accel;
        if (Mathf.Abs(x) > 0.01f)
        {
            // If trying to reverse direction mid-air, give a stronger accel
            bool reversing = Mathf.Sign(x) != Mathf.Sign(vx) && Mathf.Abs(vx) > 0.1f;
            accel = reversing ? pc.airTurnAccel : pc.airAccel;
        }
        else
        {
            accel = pc.airDecel;
            target = 0f; // drift toward 0 when no input
        }

        float newVx = Mathf.MoveTowards(vx, target, accel * Time.deltaTime);

        // Optional horizontal speed cap
        newVx = Mathf.Clamp(newVx, -pc.maxAirSpeed, pc.maxAirSpeed);

        pc.rb.linearVelocity = new Vector2(newVx, pc.rb.linearVelocity.y);
    }


}
