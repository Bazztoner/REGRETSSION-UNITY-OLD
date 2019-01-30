using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SMB_HolsterState : StateMachineBehaviour
{
    public bool finishedAnim = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        StartHolster();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        FinishHolster();
    }

    public void StartHolster()
    {
        finishedAnim = false;
    }

    public void FinishHolster()
    {
        finishedAnim = true;
    }
}
