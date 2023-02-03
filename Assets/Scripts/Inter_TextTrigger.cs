using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inter_TextTrigger : Interactible
{
    [Tooltip("this should be shown when entered")] public Dialogue dialogue;
    // Start is called before the first frame update
    protected override void Interaction()  
    {
        Debug.Log("priming " + dialogue.textBody[0]);
        UI.instance.Show(dialogue);
        Debug.Log("x");
        base.Interaction(); 
    }
    

}
