using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimChaining : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("T:" + Time.time + "Last timestamp" + PlayerCombat.instance.attackLastTimestamp);
        //Debug.Log("T diff:" + (Time.time- PlayerCombat.instance.attackLastTimestamp).ToString());
        //
    }
}
