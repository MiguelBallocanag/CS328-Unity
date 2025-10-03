public class RunState : GroundedState {
    public override void Enter(PlayerController p) { base.Enter(p); }

    public override void Tick() {
        ApplyGroundMotion();

        if (InputIdle) { pc.SwitchState(new IdleState()); return; }

        if (pc.lastPressedJumpTime > 0f && pc.lastOnGroundTime > 0f) {
            pc.SwitchState(new JumpState()); return;
        }

        if (!pc.IsGrounded) { pc.SwitchState(new FallState()); return; }
    }

    public override void Exit() { }
}
