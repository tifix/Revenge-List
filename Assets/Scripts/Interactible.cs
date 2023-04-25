/*
 * Buttons, doors. Upon entering triger collider invokes uponInteraction event.
 * Can show prompts, be auto-use or single use
 * Single use can either destroy object or disable just the script
 */
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Interactible : MonoBehaviour
{
    protected Controls input;
    [SerializeField] bool isAutoUse = true, isInRange = false;
    [SerializeField, Tooltip("if true, the script is destroyed upon use")]  protected bool isSingleUse = false;
    [SerializeField, Tooltip("if true, the script is disabled upon use")]   protected bool isDisabledOnUse = false;
    public UnityEvent uponInteractionDo = new UnityEvent();
    public Canvas PromptCanvas;

    //Binding controls
    public void Awake()
    {
        input = new Controls();
        input.Ground.Interact.Enable();
    }

    //initialising interaction prompt (if using) and AutoInteracting if player is immediatelly in range
    private void OnEnable()
    {
        InitialisePrompt();
        if (isInRange && isAutoUse) Interaction();
    }

    //if it's PLAYER tag entering - interaction behaviours
    void OnTriggerEnter(Collider other)
    {
        input.Ground.Interact.performed += DoInteraction;

        if(other.CompareTag("Player"))
        {
            isInRange = true;
            if(!isAutoUse) UI.instance.boxInteractPrompt.SetActive(true);
            if(PromptCanvas!=null) PromptCanvas.gameObject.SetActive(true);
            if (isAutoUse) Interaction(); 
        }
    }
    //Unbininding interaction and cleaning up the prompt
    private void OnTriggerExit(Collider other)
    {
        isInRange = false;
        if (!isAutoUse) UI.instance.boxInteractPrompt.SetActive(false);
        if (PromptCanvas != null) PromptCanvas.gameObject.SetActive(false);
        input.Ground.Interact.performed -= DoInteraction;
    }
    public void DoInteraction(InputAction.CallbackContext obj) {if(isInRange) Interaction(); }  //Binding the input action to the interaction event.

    //MAIN handling of the interaction
    protected virtual void Interaction()    
    {
        uponInteractionDo.Invoke();
        if (isSingleUse) { UI.instance.boxInteractPrompt.SetActive(false); input.Ground.Interact.performed -= DoInteraction; Destroy(this); }
        if (isDisabledOnUse) { UI.instance.boxInteractPrompt.SetActive(false); gameObject.SetActive(false); }
    }

    //If the interactible has a world-space prompt, auto-assign it (called OnEnable)
    protected virtual void InitialisePrompt() 
    {
        try { PromptCanvas = GetComponentInChildren<Canvas>(); } catch { }
        if (PromptCanvas != null) 
        {
            Debug.Log("Interactible canvas found, funky worldspace prompt added");
            GetComponentInChildren<Canvas>().worldCamera = Camera.main;
        }
    }
}
