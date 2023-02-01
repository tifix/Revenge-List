using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inter_TextTrigger : Interactible
{
    [Tooltip("this should be shown when entered")] public Dialogue dialogue;
    // Start is called before the first frame update
    protected override void Interaction()  {DialogueMASTERCLASS.instance.Show(dialogue); base.Interaction(); }
    

}
