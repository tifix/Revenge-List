using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class Inter_TextTrigger : Interactible
{
    public bool isGamePausedWhileShowing = true;
    [Range(0, 2f)] public float waitAfterPreDialogue = 0, waitAfterInteract = 0;
    [Tooltip("this should be shown when entered")] public Dialogue preUseDialogue;
    [Tooltip("this should be shown when entered")] public Dialogue postUsedialogue;
    // Start is called before the first frame update

    protected override void Interaction()  
    {
        StartCoroutine(executionWait());

        
       // base.Interaction();



    }
    public IEnumerator executionWait() 
    {
        //Show the pre dialogue and wait for it to finish before proceeding
        if (preUseDialogue != null) UI.instance.Show(preUseDialogue, isGamePausedWhileShowing);

        yield return new WaitForSecondsRealtime(waitAfterPreDialogue);
        while (UI.instance.runCoroutine == true) yield return new WaitForEndOfFrame();
        
        //Actual interaction happens here
        uponInteractionDo.Invoke();

        //After interaction stuffs
        yield return new WaitForSecondsRealtime(waitAfterInteract);
        if (postUsedialogue != null) UI.instance.Show(postUsedialogue, isGamePausedWhileShowing);

        //post use clean-up
        if (isSingleUse) { Debug.Log("X"); UI.instance.boxInteractPrompt.SetActive(false); input.Ground.Interact.performed -= DoInteraction; Destroy(this); }
        if (isDisabledOnUse) { UI.instance.boxInteractPrompt.SetActive(false); gameObject.SetActive(false); }
    }


}
