using UnityEngine;
using UnityEngine.InputSystem;

public class JumpState : AirState
{
    public override void Enter(PlayerController p)
    {
        base.Enter(p);

        //If we jumped from ground, reset air jumps; if we were already airborne
        // (e.g., buffered coyote), we still consider this the first jump.
        if (p.lastOnGroundTime > 0f) p.airJumpsLeft = 1; // double jump count
        DoJump();
    }

    public override void Tick()
    {
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
