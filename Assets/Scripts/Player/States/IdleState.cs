using UnityEngine;

public class IdleState : GroundedState
{
    public override void Enter(PlayerController p) { base.Enter(p); }

    public override void Tick()
    {
        float vx = pc.rb.linearVelocity.x;
        if (Mathf.Abs(vx) > pc.stopThreshold)
            pc.rb.linearVelocity = new Vector2(Mathf.MoveTowards(vx, 0f, pc.idleBrake * Time.deltaTime), pc.rb.linearVelocity.y);
        else
            pc.rb.linearVelocity = new Vector2(0f, pc.rb.linearVelocity.y);

        if (pc.dashPressed && pc.CanDashNow) { pc.SwitchState(new DashState()); return; }

        if (!InputIdle) { pc.SwitchState(new RunState()); return; }

        if (pc.ConsumeJumpBufferIfAvailable())
        {
            pc.SwitchState(new JumpState());
            return;
        }

        if (!pc.IsGrounded) { pc.SwitchState(new FallState()); return; }

        if (pc.attackPressed && pc.lightAttack != null)
        {
            pc.SwitchState(new AttackLightState(pc.lightAttack));
            return;
        }
    }

    public override void Exit() { }
}
