using UnityEngine;

public abstract class AirState : BaseState
{
    protected bool Falling => pc.rb.linearVelocity.y <= 0f;

    protected void DoJump()
    {
        pc.lastPressedJumpTime = 0f;
        pc.lastOnGroundTime = 0f;
        pc.rb.linearVelocity = new Vector2(pc.rb.linearVelocity.x, pc.JumpVelocity);
    }

    protected void ApplyBaseGravity()
    {
        float g = pc.Gravity;

        // Apex hangtime: reduce gravity near apex for precision
        if (pc.enableApexHang && pc.rb.linearVelocity.y > 0f &&
            Mathf.Abs(pc.rb.linearVelocity.y) <= pc.apexVyBand)
        {
            g *= pc.apexHangMult; // < 1 slows rise near apex
        }

        pc.rb.linearVelocity += Vector2.up * (-g * Time.fixedDeltaTime);
    }

    protected void ApplyEarlyCutIfNeeded()
    {
        if (pc.rb.linearVelocity.y > 0f)
        {
            if (!pc.jumpHeld)
            {
                // short hop gravity (Celeste / SMB jump cutoff)
                pc.rb.linearVelocity += Vector2.up * (-pc.Gravity * pc.jumpData.earlyCutMult * Time.fixedDeltaTime);
            }
        }
    }

    protected void ApplyHeavierFall()
    {
        if (pc.rb.linearVelocity.y < 0f)
        {
            float mult = pc.jumpData.fallGravityMult;
            if (pc.moveInput.y < -0.5f) mult *= pc.jumpData.fastFallMult;
            pc.rb.linearVelocity += Vector2.up * (-pc.Gravity * (mult - 1f) * Time.fixedDeltaTime);
        }
    }

    protected void ApplyAirMotion()
    {
        float x = pc.moveInput.x;
        float vx = pc.rb.linearVelocity.x;

        // base caps & accel
        float cap = Mathf.Max(pc.runSpeed, pc.maxAirSpeed);
        float target = x * cap;

        float accel;
        if (Mathf.Abs(x) > 0.01f)
        {
            bool reversing = Mathf.Sign(x) != Mathf.Sign(vx) && Mathf.Abs(vx) > 0.1f;
            accel = reversing ? pc.airTurnAccel : pc.airAccel;
        }
        else
        {
            accel = pc.airDecel;
            target = 0f;
        }

        // Apex steer: boost control and a slight cap bump near apex
        if (pc.enableApexSteer && Mathf.Abs(pc.rb.linearVelocity.y) <= pc.apexSteerVyBand)
        {
            accel *= pc.apexAccelBoost;
            cap *= pc.apexMaxAirSpeedBoost;
            target = x * cap;
        }

        float newVx = Mathf.MoveTowards(vx, target, accel * Time.fixedDeltaTime);
        newVx = Mathf.Clamp(newVx, -cap, cap);

        pc.rb.linearVelocity = new Vector2(newVx, pc.rb.linearVelocity.y);
    }
}
