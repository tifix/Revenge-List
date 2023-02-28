using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimLockMovement : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerMovement.instance.UnPauseMovement();
    }
}
