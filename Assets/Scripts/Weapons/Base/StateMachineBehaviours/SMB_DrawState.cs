using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SMB_DrawState : StateMachineBehaviour
{
    public bool finishedAnim = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        StartDraw();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        FinishDraw();
    }

    public void StartDraw()
    {
        finishedAnim = false;
    }

    public void FinishDraw()
    {
        finishedAnim = true;
    }
}
