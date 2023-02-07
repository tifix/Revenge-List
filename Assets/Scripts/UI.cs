using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;



public class UI : MonoBehaviour
{
    Controls input;
    public static UI instance;                                                                      //globally accessible reference to this script to remotely invoke it's public methods
    
    [Tooltip("Drop Dialogue Data here to be displayed")] public Dialogue dialogueCur;               //Dialogue data to be displayed
    [Space, Header("Typing settings")]
    [Tooltip("What part of the dialogue is displayed"), Range(0, 99)] public int txtPageNr = 0;     //Which chunk / page / part of dialogue is currently displayed or being typed out
    [SerializeField] private bool isWaiting = false;                                                //is the dialogue paused after a page is completelly written
    private bool runCoroutine;                                                                      //is the typer coroutine suspended?
    private string pageText = "Warning: Unassigned text";
    [Tooltip("time in s between each letter typed")] public float typingWait = 0.03f;               //how much time passes between each letter typed
    [Space,Header("Object references")]
    public GameObject boxInteractPrompt, boxQTE;                                                    //object that displays dialogue and the quick time event parent
    [SerializeField]            private GameObject boxTextDisplay, boxPause, boxSettings;           //the pause menu, the settings menu and the prompt to interact with an object
    [SerializeField]            Text txtMain, txtSpeaker;                                           //the text that displays the dialogue in UI.   Aaaand the caption of WHO is speaking
    


    public void Awake()                             //Ensuring single instance of the script
    {
        if (instance == null) instance = this;
        else Destroy(this);

        input = new Controls();

        input.Menu.Pause.performed += InputPause;
        input.Menu.Pause.Enable();
        input.Menu.Confirm.performed += ForwardDialogue;
        input.Menu.Confirm.Enable();
    }
    public void OnDestroy()
    {
        input.Menu.Confirm.performed -= ForwardDialogue;
        input.Menu.Pause.performed -= InputPause;
        GameData.instance.SetPause(false);                  //ensuring the game is not locked in a permanent pause state upon exiting while paused
    }


    public void Update()
    {
      //if(dialogueCur!=null) DialogueStoppingLogic();
    }


    public void ForwardDialogue(InputAction.CallbackContext obj)        //advancing the dialogue - next page if finished, fast forwarding otherwise.
    {
        
        if (dialogueCur != null)
        {
            if (isWaiting) { txtPageNr++; isWaiting = false; }
            else
            {
                runCoroutine = false;
            }
        }
    }


    protected IEnumerator Typer(Dialogue _dialogue) //typing the text over time
    {
        //Debug.Log("Starting Display of "+_dialogue.textBody[0]);
        
         PlayerMovement.SetLockMovement();
        
        dialogueCur = _dialogue;
        txtPageNr = 0;      //page iterator

        while (txtPageNr < _dialogue.textBody.Count)
        {
            runCoroutine = true;
            //speaker set
            if (_dialogue.textSpeaker.Count > 1) txtSpeaker.text = _dialogue.textSpeaker[txtPageNr];      //if dialogue switches between speakers - updates speaker. If just one, doesn't bother checking.
            else txtSpeaker.text = _dialogue.textSpeaker[0];

            //get text data
            pageText = _dialogue.textBody[txtPageNr];


            //Slowly display the page text
            for (int i = 0; i < (pageText.Length + 1); i++)
            {
                txtMain.text = pageText.Substring(0, i);               //slice the text 
                yield return new WaitForSeconds(typingWait);
                if (!runCoroutine) break;
            }
            
            txtMain.text = pageText;                                   //display the text
            isWaiting = true;

            while (isWaiting == true) yield return null;               //hold until player progresses the text
            
        }
        runCoroutine = false;
        dialogueCur = null;
        boxTextDisplay.SetActive(false);

        PlayerMovement.SetUnLockMovement();
    }
    public void Show(Dialogue _dialogue)                            //Call this with a dialogue structure to display it!
    {
        boxTextDisplay.SetActive(true);

        Debug.Log("initialising text");
        if (!runCoroutine)
        {
            StartCoroutine(Typer(_dialogue));
            runCoroutine = true;
        }
    }


    #region button Stuffs
    public void TogglePauseMenu() { boxPause.SetActive(!boxPause.activeInHierarchy); GameData.instance.TogglePause(); }  //Toggle pause menu
    public void BackToMenu() { GameData.instance.LoadMenu(); }
    public void InputPause(InputAction.CallbackContext obj) => TogglePauseMenu();
    public void ToggleSettings() { boxSettings.SetActive(!boxSettings.activeInHierarchy); }                                                        //Toggle settings menu
    public void ToggleQTEScreen() //Toggle QTE screen and freeze player movement
    {
        if(!boxQTE.activeInHierarchy) PlayerMovement.SetLockMovement();
        else { PlayerMovement.SetUnLockMovement(); }

        boxQTE.SetActive(!boxQTE.activeInHierarchy); 
    }                                                        
    public void QuitToWindows() { Application.Quit(); }


    #endregion
}
