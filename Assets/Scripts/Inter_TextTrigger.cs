using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DS.Data.Save;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using System.IO;
using UnityEditor;
using UnityEngine.Assertions;

public class Inter_TextTrigger : Interactible
{
    private bool isRunning = false;
    public bool isPausedWhileShowingA= true;
    [Tooltip("text shown upon interaction")] public DSGraphSaveDataSO preUseDialogue;
    [Tooltip("delay between showing dialogue and executing the event"),Range(0, 2f)] public float waitAfterPreDialogue = 0;
    [Tooltip("delay between the event and script cleanup"),Range(0, 2f)] public float waitAfterInteract = 0;

    protected override void Interaction()  
    {
        if(isSingleUse) UI.instance.boxInteractPrompt.SetActive(false);
        if (!isRunning)StartCoroutine(executionWait());   
    }
    public IEnumerator executionWait() 
    {
        isRunning = true;

        //Show the pre dialogue and wait for it to finish before proceeding
        if (preUseDialogue != null) UI.instance.DialogueShow(preUseDialogue, isPausedWhileShowingA);

        yield return new WaitForSeconds(waitAfterPreDialogue);
        while (UI.instance.runCoroutine == true) yield return new WaitForEndOfFrame();

        yield return null;
        Debug.Log("interactionEvent now!");
        //Actual interaction happens here
        uponInteractionDo.Invoke();
        yield return null;

        //post use clean-up
        if (isSingleUse) { Debug.Log("X"); UI.instance.boxInteractPrompt.SetActive(false); input.Ground.Interact.performed -= DoInteraction; Destroy(this); }
        if (isDisabledOnUse) { Debug.Log("disabling trigger "+name); UI.instance.boxInteractPrompt.SetActive(false); gameObject.SetActive(false); }
        isRunning = false;
    }



} 
