﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SMB_Evade : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.GetComponent<Enemy>().EvadeEnd();
    }
}
