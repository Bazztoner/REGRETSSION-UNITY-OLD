using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SMB_ShootState :StateMachineBehaviour
{
    public bool finishedAnim = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        StartShooting();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        FinishShooting();
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        FinishShooting();
    }

    public void StartShooting()
    {
        finishedAnim = false;
    }

    public void FinishShooting()
    {
        finishedAnim = true;
    }
}

