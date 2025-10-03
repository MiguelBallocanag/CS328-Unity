using UnityEngine;

public abstract class GroundedState : BaseState {
    protected bool InputIdle => Mathf.Abs(pc.moveInput.x) < 0.1f;

    protected void ApplyGroundMotion() {
        float x = pc.moveInput.x;
        float target = x * pc.runSpeed;
        float accel = (Mathf.Abs(x) > 0.01f ? pc.groundAccel : pc.groundDecel) * Time.deltaTime;
        float vx = Mathf.MoveTowards(pc.rb.linearVelocity.x, target, accel);
        pc.rb.linearVelocity = new Vector2(vx, pc.rb.linearVelocity.y);
    }
}

