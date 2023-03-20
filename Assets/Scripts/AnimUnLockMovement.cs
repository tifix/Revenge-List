using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimUnLockMovement : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        try
        {
            PlayerMovement.instance.UnPauseMovement();
        }
        catch { Debug.Log("player movement instance not set yet"); }
    }
}
