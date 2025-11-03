public class JumpState : AirState
{
    public override void Enter(PlayerController p)
    {
        base.Enter(p);

        p.Anim_Jump();
        DoJump();
    }

    public override void Tick()
    {
        if (pc.dashPressed && pc.CanDashNow)
        {
            pc.SwitchState(new DashState());
            return;
        }
        
        ApplyAirMotion();
        ApplyEarlyCutIfNeeded();
        ApplyHeavierFall();

        //Double jump: only while airborne, buffered press still valid
        if (!pc.IsGrounded && pc.lastPressedJumpTime > 0f && pc.airJumpsLeft > 0)
        {
            pc.airJumpsLeft--;
            DoJump();
        }

        if (Falling) pc.SwitchState(new FallState());
    }
}
