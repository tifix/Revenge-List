using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorLockPlayerWhenOnNode : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerMovement.SetUnLockMovement();
    }
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerMovement.SetLockMovement();
    }
}
