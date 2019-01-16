using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SMB_ReloadState : StateMachineBehaviour
{
    public bool finishedAnim = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        StartReload();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        FinishReload();
    }

    public void StartReload()
    {
        finishedAnim = false;
    }

    public void FinishReload()
    {
        finishedAnim = true;
    }
}
