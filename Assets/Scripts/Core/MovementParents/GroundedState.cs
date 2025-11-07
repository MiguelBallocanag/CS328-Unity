using UnityEngine;

public abstract class GroundedState : BaseState {
    protected bool InputIdle => Mathf.Abs(pc.moveInput.x) < 0.1f;

    protected void ApplyGroundMotion()
    {
        float x = pc.moveInput.x;
        float vx = pc.rb.linearVelocity.x;

        if (Mathf.Abs(x) < 0.01f)
        {
            // Extra brake during landing grace
            float brake = pc.idleBrake + (pc.InLandingGrace ? pc.graceFrictionBrake : 0f);

            if (Mathf.Abs(vx) <= pc.stopThreshold)
            {
                vx = 0f;
            }
            else
            {
                vx = Mathf.MoveTowards(vx, 0f, brake * Time.fixedDeltaTime);
            }
            pc.rb.linearVelocity = new Vector2(vx, pc.rb.linearVelocity.y);
            return;
        }

        float target = x * pc.runSpeed;
        float accel  = pc.groundAccel * Time.fixedDeltaTime;
        vx = Mathf.MoveTowards(vx, target, accel);
        pc.rb.linearVelocity = new Vector2(vx, pc.rb.linearVelocity.y);
    }
}
