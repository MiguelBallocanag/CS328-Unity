using NUnit.Framework.Interfaces;
using Unity.VisualScripting;
using UnityEngine;

public class IdleState : GroundedState
{
    public override void Enter(PlayerController p)
    {
        pc = p;
    }

    public override void Tick()
    {
        if (Mathf.Abs(pc.moveInput.x) > 0.1f)
        {
            pc.SwitchState(new RunState());

            var v = pc.rb.linearVelocity;
            pc.rb.linearVelocity = new Vector2(Mathf.MoveTowards(v.x, 0f, pc.groundDecel * Time.deltaTime), v.y);
        }
    }

    public override void Exit(){}
}