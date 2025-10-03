using UnityEngine;

public class IdleState : GroundedState
{
    public override void Enter(PlayerController p) { base.Enter(p); }

    public override void Tick()
    {
        // Move → Run
        if (!InputIdle) { pc.SwitchState(new RunState()); return; }

        // Buffered + coyote jump → Jump
        if (pc.lastPressedJumpTime > 0f && pc.lastOnGroundTime > 0f)
        {
            pc.SwitchState(new JumpState());
            return;
        }

        // Walked off a ledge → Fall
        if (!pc.IsGrounded) { pc.SwitchState(new FallState()); return; }
    }

    public override void Exit() { }
}