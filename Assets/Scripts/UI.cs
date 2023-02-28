using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;



public class UI : MonoBehaviour
{
    Controls input;
    public static UI instance;                                                                      //globally accessible reference to this script to remotely invoke it's public methods
    
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
    [SerializeField]            Image XLPortraitLilith, XLPortraitOther;                            //Portraits which display Lilith and others as they tallk
    [SerializeField]            GameObject RevengeList, RevengeListTriggerer;                       //the gorgeous scrollable revenge list

    


    public void Awake()                             //Ensuring single instance of the script
    {
        if (instance == null) instance = this;
        else Destroy(this);

        

        input = new Controls();                         //Initialising inputs
        input.Menu.Pause.performed += InputPause;
        input.Menu.Pause.Enable();
    }
    public void Start()
    {
        playerHealthBar.maxValue = PlayerCombat.instance.GetMaxHealth();    //Scaling healthbar automatically

        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        if (boss!=null && boss.TryGetComponent<bossHealth>(out bossHealth healthData)) BossInitialiseHealthBar(healthData);
    }

    public void OnDestroy()
    {
        input.Menu.Pause.performed -= InputPause;
        GameManager.instance.SetPause(false);                  //ensuring the game is not locked in a permanent pause state upon exiting while paused
    }


    public void Update()
    {
        UpdateHealthDisplays();
    }

    //Some values seem unnecessary to update each frame ie. the max values
    //Might be useful to call player and boss separately only when taking damage - AV
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
            GameManager.instance.CamFollowPlayer();
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
            bossShieldBar.maxValue = data.GetMaxHealth();
            bossHealthBar.maxValue = data.coreHealth;
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


   public Sprite SetBigSpriteForDialogue(string fileName) 
   {
        Sprite s = Resources.Load<Sprite>(fileName) as Sprite;
        if (s != null)
        {
            if (fileName.Contains("Lil"))
            {
                Debug.Log("Sprite identified as Lilith");
                SetSpriteXLLilith(s);
                ShowSpriteXLLilith();
            }
            else
            {
                Debug.Log("Sprite identified as Other");
                SetSpriteXLOther(s);
                ShowSpriteXLOther();
            }

        }
        if (s == null) { Debug.LogWarning("Could not find a sprite named: "+fileName+ " in the Resorces/DialogueSprites folder"); HideSpriteXLLilith(); HideSpriteXLOther(); } //Hide both if name is invalid

        return null;
   }



    #region buttons and element toggles
    public void TogglePauseMenu() { boxPause.SetActive(!boxPause.activeInHierarchy); GameManager.instance.TogglePause(); }     //Toggle pause menu
    public void BackToMenu() { GameManager.LoadMenu(); }
    public void InputPause(InputAction.CallbackContext obj) => TogglePauseMenu();
    public void ToggleSettings() { boxSettings.SetActive(!boxSettings.activeInHierarchy); }                                 //Toggle settings menu
    public void ToggleHealthbar() { boxHealthbar.SetActive(!boxHealthbar.activeInHierarchy); }                              //Toggle the player healthbar display
    public void ToggleQTEScreen()                                                                                           //Toggle QTE screen and freeze player movement
    {
        if(!boxQTE.activeSelf) PlayerMovement.instance.SetLockMovement();
        else { PlayerMovement.instance.SetUnLockMovement(); }
        
        boxQTE.SetActive(!boxQTE.activeInHierarchy); 
    }
    public void EnableLostScreen()
    {
        boxLost.SetActive(true);
        StartCoroutine(WaitAndLoadMenu(GameManager.instance.DeathReloadTime));
    }

    public void ToggleRevengeList() => RevengeList.SetActive(!RevengeList.activeInHierarchy);

    public void QuitToWindows() { Application.Quit(); }

    public void SetSpriteXLLilith(Sprite s) { XLPortraitLilith.gameObject.SetActive(true); XLPortraitLilith.sprite = s; }
    public void SetSpriteXLOther(Sprite s) { XLPortraitOther.gameObject.SetActive(true); XLPortraitOther.sprite = s; }
    public void HideSpriteXLLilith() { XLPortraitLilith.gameObject.SetActive(false); }
    public void HideSpriteXLOther() { XLPortraitOther.gameObject.SetActive(false); }
    public void ShowSpriteXLLilith() { XLPortraitLilith.gameObject.SetActive(true); }
    public void ShowSpriteXLOther() { XLPortraitOther.gameObject.SetActive(true); }


    #endregion
}
