public class RunState : GroundedState {
    public override void Enter(PlayerController p) { base.Enter(p); }

    public override void Tick()
    {
        if (pc.dashPressed && pc.CanDashNow) { pc.SwitchState(new DashState()); return; }
        ApplyGroundMotion();

        if (InputIdle) { pc.SwitchState(new IdleState()); return; }

        // Buffered + coyote jump → Jump (REPLACED)
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
