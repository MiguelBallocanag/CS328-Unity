// RunState.cs
using UnityEngine;
public class RunState : GroundedState {
    public override void Enter(PlayerController p){ pc = p; }
    public override void Tick() {
        float x = pc.moveInput.x;
        float target = x * pc.runSpeed;
        float vx = Mathf.MoveTowards(pc.rb.linearVelocity.x, target, pc.groundAccel * Time.deltaTime);
        pc.rb.linearVelocity = new Vector2(vx, pc.rb.linearVelocity.y);

        if (Mathf.Abs(x) < 0.1f) pc.SwitchState(new IdleState());
    }
    public override void Exit() { }
}
