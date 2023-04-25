using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimAttackDamage : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerCombat.instance.DoDamage();
    } 
}
