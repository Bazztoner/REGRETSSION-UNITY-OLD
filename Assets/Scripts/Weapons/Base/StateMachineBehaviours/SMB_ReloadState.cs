using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SMB_ReloadState : StateMachineBehaviour
{
    public bool finishedAnim = false;
    public bool reloadCancelled = false;

    public void CancelReload()
    {
        reloadCancelled = true;
    }

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
        reloadCancelled = false;
        finishedAnim = false;
    }

    public void FinishReload()
    {
        finishedAnim = true;
    }
}
