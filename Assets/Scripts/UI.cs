using DS.Data.Save;
using DS.Utilities;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;



public class UI : MonoBehaviour
{
    Controls input;
    [SerializeField]Animator anim;
    public static UI instance;                                                                      //globally accessible reference to this script to remotely invoke it's public methods
    
    [Tooltip("Drop Dialogue Data here to be displayed")] public DSGraphSaveDataSO dialogueCur;               //Dialogue data to be displayed
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
    [SerializeField]            TextMeshProUGUI txtMain, txtSpeaker;                                           //the text that displays the dialogue in UI.   Aaaand the caption of WHO is speaking
    [SerializeField]            TextMeshProUGUI txtChoiceA, txtChoiceB;                                           //the text that displays the dialogue in UI.   Aaaand the caption of WHO is speaking
    [SerializeField]            Slider playerHealthBar, bossHealthBar, bossShieldBar;
    [SerializeField]            Image   playerPortrait;                                             //Displayer of player portrait
    [SerializeField]            Sprite[] playerPortraits = new Sprite[5];                           //Images to display as player looses health
    [SerializeField]            Image XLPortraitLilith, XLPortraitOther;                            //Portraits which display Lilith and others as they tallk
    [SerializeField]            GameObject RevengeList, RevengeListTriggerer;                       //the gorgeous scrollable revenge list

    [SerializeField] private string choiceA_ID = null, choiceB_ID = null;
    [SerializeField] private DSNodeSaveData NodeCurrent;
    [SerializeField] private DSGraphSaveDataSO dataTemp;


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

    //Probably need a version that doesn't lock the movement and/or stops time (actually use pauseWhileRunning) - AV
    protected IEnumerator Typer(DSGraphSaveDataSO _dialogue, bool pauseWhileRunning) //typing the text over time
    {
        PlayerMovement.instance.SetLockMovement();



        dialogueCur = _dialogue;
        txtPageNr = 0;      //page iterator
        Time.timeScale = 0;

        //retrieve START node
        NodeCurrent = null;
        foreach (DSNodeSaveData n in dialogueCur.Nodes)
        {
            if (n.isStartNode) 
            {
                NodeCurrent = n;
                Debug.Log("start node found! " + n.Name);
                break;
            } 
        }
        if (NodeCurrent == null) Debug.LogWarning("No start node found!");

        while(true) //Main dialogue loop - repeat until the next one has no children
        {
            txtSpeaker.text = NodeCurrent.SpeakerName;
            pageText = NodeCurrent.Text;
            SetBigSpriteForDialogue("DialogueSprites/" + NodeCurrent.SpritePath);
            runCoroutine=true;
            Debug.Log(pageText);

            //Choice node special functionality HERE!
            if (NodeCurrent.Choices.Count > 1) 
            {
                Debug.LogWarning("A wild chocie appeared!");
                Debug.Log("A: " + NodeCurrent.Choices[0].Text);
                Debug.Log("B: " + NodeCurrent.Choices[1].Text);

                dataTemp = _dialogue;
                SetDialogueChoices(NodeCurrent.Choices[0].Text, NodeCurrent.Choices[1].Text, NodeCurrent.ChildIDs[0], NodeCurrent.ChildIDs[1]);
                isWaiting = true;
                while (isWaiting == true) yield return new WaitForEndOfFrame();
                Debug.LogWarning("Option setting SHOULD be complete. Emphasis on should/");
                continue;
            }

            //For regular nodes Slowly display the page text
            for (int j = 0; j < (pageText.Length + 1); j++)
            {
                txtMain.text = pageText.Substring(0, j);               //slice the text 
                if (GameManager.instance.cheat_FastForwardDialogue) { GameManager.instance.cheat_FastForwardDialogue= false; goto endOfDialogue; }
                yield return new WaitForSecondsRealtime(typingWait);
                if (!runCoroutine) break;
            }
            if (GameManager.instance.cheat_FastForwardDialogue) { GameManager.instance.cheat_FastForwardDialogue=false; break; }

            //Show full text once done and wait for input to proceed;
            txtMain.text = pageText; isWaiting = true;
            while (isWaiting == true) yield return null;

            //Final node detection - break the loop if the node has no children
            try 
            {
                if (string.IsNullOrEmpty(NodeCurrent.ChildIDs[0])) { Debug.LogWarning("End of dialogue stream reached"); break; }
                NodeCurrent = FindSaveDataID(NodeCurrent.ChildIDs[0], _dialogue);
            }
            catch {Debug.LogWarning("End of dialogue stream reached"); break;  }
        }

        endOfDialogue:
        yield return 1;
        Time.timeScale = 1;
        runCoroutine = false;                                          //Disable once finished
        dialogueCur = null;
        boxTextDisplay.SetActive(false);
        HideSpriteXLLilith();
        HideSpriteXLOther();

        PlayerMovement.instance.SetUnLockMovement();
    }

    private DSNodeSaveData GetChildNodeData() 
    {
        DSNodeSaveData t = null;
        //NodeCu

        return t;
    }


    public void Show(DSGraphSaveDataSO _dialogue, bool pauseWhileRunning)                            //Call this with a dialogue structure to display it!
    {
        boxTextDisplay.SetActive(true);




        Debug.Log("initialising text");
        if (!runCoroutine)
        {
            StartCoroutine(Typer(_dialogue,pauseWhileRunning));
            runCoroutine = true;
        }
    }
    public void Show(DSGraphSaveDataSO _dialogue) => Show(_dialogue, false); //by default - pause world time while showing dialogue

    #endregion

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
            bossHealth.gameObject.SetActive(false);
            boxBossBar.SetActive(false);
            PlayerMovement.instance.ReleaseBind();
            PlayerMovement.instance.SetUnLockMovement();
            GameManager.instance.CamFollowPlayer();
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


    public static DSNodeSaveData FindSaveDataID(string ID, DSGraphSaveDataSO graph)
    {
        if (ID == null || ID == "") return null;

        foreach (DSNodeSaveData node in graph.Nodes)
        {
            if (ID == node.ID)
            {
                return node;
            }
        }
        return null;
    }



    public void SetDialogueChoicesTest(string text) => SetDialogueChoices(text, text, "DebugA","DebugB");
    public void SetDialogueChoices(string textA, string textB, string AAddress, string BAddress) 
    {

        Debug.Log("Dialogue Choices set to");
        Debug.Log(textA+ " : " + textB);
        txtChoiceA.text = textA;    txtChoiceB.text = textB;
        choiceA_ID = AAddress;      choiceB_ID = BAddress;

        //enabling the choice displays once set
        if (!boxTextDisplay.gameObject.activeInHierarchy) boxTextDisplay.gameObject.SetActive(true);
        txtChoiceA.transform.parent.parent.gameObject.SetActive(true);
    }
    public void SetDialogueChoiceHidden()=> txtChoiceA.transform.parent.parent.gameObject.SetActive(false);

    public void OnDialogueChoiceA() => OnDialogueChoice(true);
    public void OnDialogueChoiceB() => OnDialogueChoice(false);
    public void OnDialogueChoice(bool isA) 
    {
        if(isA) NodeCurrent=FindSaveDataID(choiceA_ID, dataTemp);
        else NodeCurrent =FindSaveDataID(choiceB_ID, dataTemp);

        SetDialogueChoiceHidden();
        isWaiting = false;
    }

    public void FadeOut()
    {
        anim.SetTrigger("Out");
    }

    public void FadeIn()
    {
        anim.SetTrigger("In");
    }

    #region buttons and element toggles
    public void TogglePauseMenu() { boxPause.SetActive(!boxPause.activeInHierarchy); GameManager.instance.TogglePause(); }     //Toggle pause menu
    public void BackToMenu() 
    {
        AudioManager.instance.Play("MenuClick");
        GameManager.LoadMenu(); 
    }
    public void InputPause(InputAction.CallbackContext obj) => TogglePauseMenu();
    public void ToggleSettings()                                                                                            //Toggle settings menu
    {
        AudioManager.instance.Play("MenuClick");
        boxSettings.SetActive(!boxSettings.activeInHierarchy); 
    }                                 
    public void ToggleHealthbar() { boxHealthbar.SetActive(!boxHealthbar.activeInHierarchy); }                              //Toggle the player healthbar display
    public void ToggleQTEScreen()                                                                                           //Toggle QTE screen and freeze player movement
    {
        if(boxQTE.activeSelf) PlayerMovement.instance.SetLockMovement();
        else { PlayerMovement.instance.SetUnLockMovement(); }

        boxQTE.SetActive(!boxQTE.activeInHierarchy); 
    }
    public void EnableLostScreen()
    {
        boxLost.SetActive(true);
        StartCoroutine(WaitAndLoadMenu(GameManager.instance.DeathReloadTime));
    }

    public void ToggleRevengeList() => RevengeList.SetActive(!RevengeList.activeInHierarchy);

    public void QuitToWindows() 
    {
        AudioManager.instance.Play("MenuClick");
        Application.Quit(); 
    }

    public void SetSpriteXLLilith(Sprite s) { XLPortraitLilith.gameObject.SetActive(true); XLPortraitLilith.sprite = s; }
    public void SetSpriteXLOther(Sprite s) { XLPortraitOther.gameObject.SetActive(true); XLPortraitOther.sprite = s; }
    public void HideSpriteXLLilith() { XLPortraitLilith.gameObject.SetActive(false); }
    public void HideSpriteXLOther() { XLPortraitOther.gameObject.SetActive(false); }
    public void ShowSpriteXLLilith() { XLPortraitLilith.gameObject.SetActive(true); }
    public void ShowSpriteXLOther() { XLPortraitOther.gameObject.SetActive(true); }


    public void LoadDialogueFromText() 
    {
    
    }

    #endregion
}
