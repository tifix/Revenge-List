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
    [SerializeField] private bool isShowingBossHealth = false;                                      //is the boss healthbar being displayed?
    public bossHealth bossHealth;                                                                   // the data for a boss. SHOULD auto assign when a boss spawns
    public bool runCoroutine;                                                                      //is the typer coroutine suspended?
    private string pageText = "Warning: Unassigned text";
    [Tooltip("time in s between each letter typed")] public float typingWait = 0.03f;               //how much time passes between each letter typed
    [Space, Header("Object references")]
    public GameObject boxInteractPrompt; public GameObject boxQTE;                                                    //object that displays dialogue and the quick time event parent
    [SerializeField]            GameObject boxTextDisplay, boxPause, boxSettings;                   //the pause menu, the settings menu and the prompt to interact with an object
    [SerializeField]            GameObject boxWon, boxLost;                                         //VictoryScreen and Death screen respectively
    [SerializeField]            GameObject boxHealthbar, boxBossBar;                                 //PLAYER AND BOSS healthbars respectively
    [SerializeField]            Text txtMain, txtSpeaker;                                           //the text that displays the dialogue in UI.   Aaaand the caption of WHO is speaking
    [SerializeField]            Slider playerHealthBar, bossHealthBar, bossShieldBar;
    [SerializeField]            Image   playerPortrait;                                             //Displayer of player portrait
    [SerializeField]            Sprite[] playerPortraits = new Sprite[5];                           //Images to display as player looses health
    [SerializeField]            Image XLPortraitLilith, XLPortraitOther;                           //Images to display as player looses health

    


    public void Awake()                             //Ensuring single instance of the script
    {
        if (instance == null) instance = this;
        else Destroy(this);

        

        input = new Controls();                         //Initialising inputs
        input.Menu.Pause.performed += InputPause;
        input.Menu.Pause.Enable();
        input.Menu.Confirm.performed += ForwardDialogue;
        input.Menu.Confirm.Enable();
    }
    public void Start()
    {
        playerHealthBar.maxValue = PlayerCombat.instance.GetMaxHealth();    //Scaling healthbar automatically

        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        if (boss!=null && boss.TryGetComponent<bossHealth>(out bossHealth healthData)) BossInitialiseHealthBar(healthData);
    }

    public void OnDestroy()
    {
        input.Menu.Confirm.performed -= ForwardDialogue;
        input.Menu.Pause.performed -= InputPause;
        GameManager.instance.SetPause(false);                  //ensuring the game is not locked in a permanent pause state upon exiting while paused
    }


    public void Update()
    {
        UpdateHealthDisplays();
    }
    #region dialogue-related

    public void ForwardDialogue(InputAction.CallbackContext obj)        //advancing the dialogue - next page if finished, fast forwarding otherwise.
    {
        if (dialogueCur != null)
        {
            if (isWaiting) { txtPageNr++; isWaiting = false; }
            else runCoroutine = false;
        }
    }

    protected IEnumerator Typer(Dialogue _dialogue, bool pauseWhileRunning) //typing the text over time
    {
        //Debug.Log("Starting Display of "+_dialogue.textBody[0]);
        
         PlayerMovement.SetLockMovement();
        
        dialogueCur = _dialogue;
        txtPageNr = 0;      //page iterator
        Time.timeScale = 0;

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
                yield return new WaitForSecondsRealtime(typingWait);
                if (!runCoroutine) break;
            }
            
            txtMain.text = pageText;                                   //display the text
            isWaiting = true;

            while (isWaiting == true) yield return null;               //hold until player progresses the text with SPACE
            
        }
        yield return 1;
        Time.timeScale = 1;
        runCoroutine = false;                                          //Disable once finished
        dialogueCur = null;
        boxTextDisplay.SetActive(false);

        PlayerMovement.SetUnLockMovement();
    }
    public void Show(Dialogue _dialogue, bool pauseWhileRunning)                            //Call this with a dialogue structure to display it!
    {
        boxTextDisplay.SetActive(true);

        Debug.Log("initialising text");
        if (!runCoroutine)
        {
            StartCoroutine(Typer(_dialogue,pauseWhileRunning));
            runCoroutine = true;
        }
    }
    public void Show(Dialogue _dialogue) => Show(_dialogue, false); //by default - pause world time while showing dialogue

    #endregion

    public void UpdateHealthDisplays()
    {
        //Debug.Log(PlayerCombat.instance.GetHealth());
        playerHealthBar.value = PlayerCombat.instance.GetHealth();
        SetPlayerPortrait(PlayerCombat.instance.GetHealth(), PlayerCombat.instance.GetMaxHealth()); //setting the player portrait from the base of avalible health-reflecting portraits

        if (bossHealth != null)
        {
            bossShieldBar.maxValue = bossHealth.GetMaxHealth();
            bossShieldBar.value = bossHealth.GetHealth();
        } 
    }
    public void AfterQTE_UI(int count)
    {

        if (bossHealth.coreHealth < 1)  //Hide healthbars upon boss death
        {
            Debug.LogWarning("Boss defeated!");
            bossHealth.gameObject.SetActive(false);
            boxBossBar.SetActive(false);
        }
                                        //Refill shieldbar and main health
        bossHealth.isCoreExposed = false;
        bossHealth.SetHealth(bossHealth.GetMaxHealth());
        bossShieldBar.value = bossShieldBar.maxValue;
        bossHealthBar.value = bossHealth.coreHealth;
    }


    public void BossInitialiseHealthBar(bossHealth data)                        //initialises the boss healthbar. 
    {
            bossHealth = data;
            bossHealthBar.maxValue = data.GetMaxHealth();
            bossHealthBar.value = bossHealthBar.maxValue;
            boxBossBar.SetActive(true);
        
    }

    public void SetPlayerPortrait(float health, float maxHealth) 
    {
        float hp_normalised = health / maxHealth;
        int whichPortrait =(int)Mathf.Round( Mathf.Lerp(0, playerPortraits.Length-1, hp_normalised));  //from the list of portraits, grab  
        playerPortrait.sprite = playerPortraits[whichPortrait];
    }       //Changes the player portrait shown next to the healthbar

    protected IEnumerator WaitAndLoadMenu(float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        BackToMenu();

    }


   


    #region buttons and element toggles
    public void TogglePauseMenu() { boxPause.SetActive(!boxPause.activeInHierarchy); GameManager.instance.TogglePause(); }     //Toggle pause menu
    public void BackToMenu() { GameManager.LoadMenu(); }
    public void InputPause(InputAction.CallbackContext obj) => TogglePauseMenu();
    public void ToggleSettings() { boxSettings.SetActive(!boxSettings.activeInHierarchy); }                                 //Toggle settings menu
    public void ToggleHealthbar() { boxHealthbar.SetActive(!boxHealthbar.activeInHierarchy); }                              //Toggle the player healthbar display
    public void ToggleQTEScreen()                                                                                           //Toggle QTE screen and freeze player movement
    {
        if(!boxQTE.activeInHierarchy) PlayerMovement.SetLockMovement();
        else { PlayerMovement.SetUnLockMovement(); }

        boxQTE.SetActive(!boxQTE.activeInHierarchy); 
    }
    public void EnableLostScreen()
    {
        boxLost.SetActive(true);
        StartCoroutine(WaitAndLoadMenu(GameManager.instance.DeathReloadTime));
    }
    public void QuitToWindows() { Application.Quit(); }

    public void SetSpriteXLLilith(Sprite s) { XLPortraitLilith.gameObject.SetActive(true); XLPortraitLilith.sprite = s; }
    public void SetSpriteXLOther(Sprite s) { XLPortraitOther.gameObject.SetActive(true); XLPortraitOther.sprite = s; }
    public void HideSpriteXLLilith() { XLPortraitLilith.gameObject.SetActive(false); }
    public void HideSpriteXLOther() { XLPortraitOther.gameObject.SetActive(false); }

    #endregion
}
