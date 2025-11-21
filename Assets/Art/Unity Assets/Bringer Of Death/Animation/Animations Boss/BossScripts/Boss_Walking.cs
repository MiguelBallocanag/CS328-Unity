using UnityEngine;

// MINIMAL VERSION - Only needed if you still want state machine control
// If using BossReaper_SIMPLE_ALL_IN_ONE.cs, you can DELETE this script entirely
// or keep it as a backup that does nothing

public class Boss_Walking : StateMachineBehaviour
{
    // This version does NOTHING - all logic moved to BossReaper
    // Keep it only for animator compatibility, or delete the behavior from your Walking state
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("[Boss_Walking] State entered - but all logic is now in BossReaper script");
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Empty - BossReaper handles everything now
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Empty
    }
}
