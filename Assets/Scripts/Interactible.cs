using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Interactible : MonoBehaviour
{
    protected Controls input;
    [SerializeField] bool isAutoUse = true, isInRange = false;
    [SerializeField, Tooltip("if true, the script is destroyed upon use")] protected bool isSingleUse = false;
    [SerializeField, Tooltip("if true, the script is disabled upon use")] protected bool isDisabledOnUse = false;
    public UnityEvent uponInteractionDo = new UnityEvent();

    public void Awake()
    {
        input = new Controls();
        input.Ground.Interact.Enable();
        //input.Ground.Interact.performed +=DoInteraction;
    }

    

    void OnTriggerEnter(Collider other)
    {
        //if(other.gameObject)
        //input.Ground.Interact.Enable();
        input.Ground.Interact.performed += DoInteraction;

        isInRange = true;
        if(!isAutoUse) UI.instance.boxInteractPrompt.SetActive(true);
        if (isAutoUse) { Interaction(); }  //Forbidden magiks resonate deep within
    }
    

    private void OnTriggerExit(Collider other)
    {
        isInRange = false;
        if (!isAutoUse) UI.instance.boxInteractPrompt.SetActive(false);
        //input.Ground.Interact.ChangeBinding(0).Erase();//
        input.Ground.Interact.performed -= DoInteraction;
    }
    public void DoInteraction(InputAction.CallbackContext obj) {if(isInRange) Interaction(); }
     

    protected virtual void Interaction()
    {
        uponInteractionDo.Invoke();
        if (isSingleUse) { UI.instance.boxInteractPrompt.SetActive(false); input.Ground.Interact.performed -= DoInteraction; Destroy(this); }
        if (isDisabledOnUse) { UI.instance.boxInteractPrompt.SetActive(false); gameObject.SetActive(false); }
    }


}
