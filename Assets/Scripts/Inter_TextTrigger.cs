using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class Inter_TextTrigger : Interactible
{
    private bool isRunning = false;
    public bool isPausedWhileShowingA= true;
    [Tooltip("this should be shown when entered")] public Dialogue preUseDialogue;
    [Range(0, 2f)] public float waitAfterPreDialogue = 0;
    public bool isPausedWhileShowingB = false;
    [Tooltip("this should be shown when entered")] public Dialogue postUsedialogue;
    [Range(0, 2f)] public float waitAfterInteract = 0;
    // Start is called before the first frame update

    protected override void Interaction()  
    {
        if(!isRunning)StartCoroutine(executionWait());   
       // base.Interaction();
    }
    public IEnumerator executionWait() 
    {
        isRunning = true;

        //Show the pre dialogue and wait for it to finish before proceeding
        if (preUseDialogue != null) UI.instance.Show(preUseDialogue, isPausedWhileShowingA);

        yield return new WaitForSeconds(waitAfterPreDialogue);
        while (UI.instance.runCoroutine == true) yield return new WaitForEndOfFrame();

        yield return null;
        Debug.Log("interactionEvent now!");
        //Actual interaction happens here
        uponInteractionDo.Invoke();
        yield return null;

        //possible after interaction dialogue. Need to detect when the interaction thing is finished
        /*
        yield return new WaitForSeconds(waitAfterInteract);
        if (postUsedialogue != null) UI.instance.Show(postUsedialogue, isPausedWhileShowingB);
        */

        //post use clean-up
        if (isSingleUse) { Debug.Log("X"); UI.instance.boxInteractPrompt.SetActive(false); input.Ground.Interact.performed -= DoInteraction; Destroy(this); }
        if (isDisabledOnUse) { Debug.Log("disabling trigger "+name); UI.instance.boxInteractPrompt.SetActive(false); gameObject.SetActive(false); }
        isRunning = false;
    }


}
