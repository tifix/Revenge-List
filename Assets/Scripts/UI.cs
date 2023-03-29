﻿using DS.Data.Save;
using DS.Utilities;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

/*
 * Main UI handler
 * 
 * handles dialogue displaying, choices, toggling of UI elements and healthbars
 * 
 */

public class UI : MonoBehaviour
{
    Controls input;
    [SerializeField] Animator anim;
    public static UI instance;                                                                      //globally accessible reference to this script to remotely invoke it's public methods
    [Tooltip("Drop Dialogue Data here to be displayed")] public DSGraphSaveDataSO dialogueCur;      //Dialogue data to be displayed

    //
    [Space, Header("Dialogue Typing settings")]
    //
    public bool runCoroutine;                                                                                   //is the typer coroutine suspended?                                          
    [SerializeField] private bool isWaiting = false;                                                            //is the dialogue paused after a page is completelly written
    [Tooltip("What part of the dialogue is displayed"), Range(0, 99)] public int txtPageNr = 0;                 //Which chunk / page / part of dialogue is currently displayed or being typed out
    private string pageText = "Warning: Unassigned text";
    //[Tooltip("time in s between each letter typed")] public float typingWait = 0.03f;                           //how much time passes between each letter typed
    [Range(0, 2f), SerializeField, Tooltip("disable dialogue skipping at first")] float initSkipLock = 0.1f;    //for how long should the dialogue be unable to be skipped when it is shown 
    bool dialogueSkipLocked = true;                                                                
    //
    [Space, Header("Healthbar settings")]
    //                                            
    [SerializeField] private bool isShowingBossHealth = false;                                      //is the boss healthbar being displayed?
    public bossHealth bossHealth;                                                                   // the data for a boss. SHOULD auto assign when a boss spawns

    //
    [Space, Header("Object references for UI objects")]
    //
    public GameObject boxInteractPrompt; public GameObject boxQTE;                                  //object that displays dialogue and the quick time event parent
    [SerializeField] GameObject boxTextDisplay, boxPause, boxSettings;                   //the pause menu, the settings menu and the prompt to interact with an object
    [SerializeField] GameObject boxWon, boxLost;                                         //VictoryScreen and Death screen respectively
    public           GameObject boxHealthbar, boxBossBar;                                //PLAYER AND BOSS healthbars respectively
    [Space(10)]
    [SerializeField] TextMeshProUGUI txtMain;                                            //the text that displays the dialogue in UI.  
    [SerializeField] TextMeshProUGUI txtSpeaker;                                         //The caption of WHO is speaking
    [SerializeField] TextMeshProUGUI txtChoiceA, txtChoiceB;                             //dialogue choice button texts
    [Space(10)]
    [SerializeField] Slider playerHealthBar;
    [SerializeField] Image playerPortrait;                                               //Displayer of player portrait
    [SerializeField] Sprite[] playerPortraits = new Sprite[5];                           //Images to display as player looses health
    [SerializeField] Slider bossHealthBar, bossShieldBar;
    [SerializeField] Slider settingVolumeMusic, settingVolumeSFX,settingVolume, settingTypeSpeed;
    [SerializeField] Image XLPortraitLilith, XLPortraitOther;                            //Portraits which display Lilith and others as they tallk
    [Space(10)]
    [SerializeField] GameObject RevengeList;                                             //the gorgeous scrollable revenge list
    [SerializeField] GameObject RevengeListTriggerer;                                    //the buton triggering the revenge lsit display
    [SerializeField] GameObject OutroCinematicObject;                                    //displays the outro pretties!
    [SerializeField] DSGraphSaveDataSO OutroDialogue1, OutroDialogue2;                   //dialogue displayed in the outro sequence
    [SerializeField] AudioMixer AudioMixer;                                              //audio mixer, volume of which we're changing

    //
    //Node typing data retrieval
    //
    private string choiceA_ID = null, choiceB_ID = null;
    private DSNodeSaveData NodeCurrent;
    private DSGraphSaveDataSO dataTemp;

    #region MonoBehaviour simple methods
    public void Awake()                             //Ensuring single instance of the script
    {
        if (instance == null) instance = this;
        else Destroy(this);

        input = new Controls();                         //Initialising inputs
        input.Menu.Pause.performed += InputPause;
        input.Menu.Pause.Enable();
        input.Menu.Confirm.performed += DialogueAdvance;
        input.Menu.Confirm.Enable();

    }
    public void Start()
    {
        playerHealthBar.maxValue = PlayerCombat.instance.GetMaxHealth();    //Scaling healthbar automatically

        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        if (boss != null && boss.TryGetComponent<bossHealth>(out bossHealth healthData)) InitialiseHealthBoss(healthData);
        ImportSettingsFromMenu();   //imports volume and typing speed settings from menu settings.
    }

    public void OnDestroy()
    {
        input.Menu.Confirm.performed -= DialogueAdvance;
        input.Menu.Pause.performed -= InputPause;
        GameManager.instance.SetPause(false);                  //ensuring the game is not locked in a permanent pause state upon exiting while paused
    }


    public void Update()
    {
        UpdateHealthDisplays();
    }

    #endregion

    #region dialogue-related

    public void DialogueAdvance(InputAction.CallbackContext obj)        //advancing the dialogue - next page if finished, fast forwarding otherwise.
    {
        if (dialogueCur != null)
        {
            if (isWaiting) { txtPageNr++; isWaiting = false; }
            else if (!dialogueSkipLocked) runCoroutine = false;
            //Debug.Log(Time.time+" > "+ (dilogueStartTimestamp+ initSkipLock).ToString());
            //if (Time.time > dilogueStartTimestamp + initSkipLock)    // after a small initial delay, enable fast forwarding dialoge
            //if(Time.time > dilogueStartTimestamp+initSkipLock) 
        }
    }

    private void ImportSettingsFromMenu() 
    {
        try
        {
            if (Settings.instance != null) 
            {
                settingTypeSpeed.value = Settings.instance.typingWait;
                settingVolume.value = Settings.instance.volume;
                settingVolumeMusic.value = Settings.instance.volumeMusic;
                settingVolumeSFX.value = Settings.instance.volumeSFX;
            }
           
        }
        catch
        {

            Debug.LogWarning("Error while importing settings from menu scene. Proceeding with defaults");
        }
    
    }

    protected IEnumerator DialogueTyper(DSGraphSaveDataSO _dialogue, bool pauseWhileRunning) //typing the text over time
    {
        NodeCurrent = DialogueInitialise(_dialogue, pauseWhileRunning);

        //Main dialogue loop - repeat until the next one has no children
        while (true)
        {
            txtSpeaker.text = NodeCurrent.SpeakerName;
            pageText = NodeCurrent.Text;
            SetBigSpriteForDialogue("DialogueSprites/" + NodeCurrent.SpritePath);
            runCoroutine = true;

            //Choice node special functionality HERE!
            if (NodeCurrent.Choices.Count > 1)
            {
                dataTemp = _dialogue;
                SetDialogueChoices(NodeCurrent.Choices[0].Text, NodeCurrent.Choices[1].Text, NodeCurrent.ChildIDs[0], NodeCurrent.ChildIDs[1]);
                isWaiting = true;
                while (isWaiting == true) yield return new WaitForEndOfFrame();
                continue;
            }

            //For regular nodes Slowly display the page text
            for (int j = 0; j < (pageText.Length + 1); j++)
            {
                txtMain.text = pageText.Substring(0, j);               //slice the text 
                if (GameManager.instance.cheat_FastForwardDialogue) {  goto endOfDialogue; }
                yield return new WaitForSecondsRealtime(Settings.instance.typingWait);
                if (!runCoroutine) break;
            }
            if (GameManager.instance.cheat_FastForwardDialogue) {  break; }

            //Show full text once done and wait for input to proceed;
            txtMain.text = pageText; isWaiting = true;
            while (isWaiting == true) yield return null;

            //Final node detection - break the loop if the node has no children
            try
            {
                if (string.IsNullOrEmpty(NodeCurrent.ChildIDs[0])) { Debug.LogWarning("End of dialogue stream reached"); break; }
                NodeCurrent = FindSaveDataID(NodeCurrent.ChildIDs[0], _dialogue);
            }
            catch { Debug.LogWarning("End of dialogue stream reached"); break; }
        }

        endOfDialogue:
        yield return 1;

        //Disable once finished
        DialogueCleanup(pauseWhileRunning);
    }

    public DSNodeSaveData DialogueInitialise(DSGraphSaveDataSO _dialogue, bool pauseWhileRunning) //Find the start node values
    {
        if (pauseWhileRunning) PlayerMovement.instance.SetLockMovement();
        dialogueCur = _dialogue;
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

        return NodeCurrent;
    }
    public void DialogueCleanup(bool pauseWhileRunning) //Hide dialogue bits and resert values once done
    {
        Time.timeScale = 1;
        runCoroutine = false;
        dialogueCur = null;
        boxTextDisplay.SetActive(false);
        HideSpriteXLLilith();
        HideSpriteXLOther();
        RevengeListTriggerer.SetActive(true);

        if (pauseWhileRunning) PlayerMovement.instance.SetUnLockMovement();
    }

    public void DialogueShow(DSGraphSaveDataSO _dialogue, bool pauseWhileRunning)                    //Call this with a dialogue structure to display it!
    {
        StartCoroutine(DialogueSkipLock()); //locks the ability to skip for REALTIME duration
        boxTextDisplay.SetActive(true);
        RevengeListTriggerer.SetActive(false);
        PlayerCombat.instance.gameObject.GetComponent<Animator>().SetBool("isAttacking", false);    //Interupting attack combos- M
        Debug.Log("initialising text");
        if (!runCoroutine)
        {
            StartCoroutine(DialogueTyper(_dialogue, pauseWhileRunning));
            runCoroutine = true;
        }
    }
    public void DialogueShow(DSGraphSaveDataSO _dialogue) => DialogueShow(_dialogue, false); //by default - pause world time while showing dialogue
    public static void Dialogue(DSGraphSaveDataSO _dialogue) => instance.DialogueShow(_dialogue, false); //by default - pause world time while showing dialogue. Shorthand for those who can't type or wanna grab from weird places

    IEnumerator DialogueSkipLock() //as requested by R*, dialogue is not skippable for the first moment after showing up. Coroutine needed as realtime is paused
    {
        dialogueSkipLocked = true;
        yield return new WaitForSecondsRealtime(initSkipLock);
        dialogueSkipLocked = false;
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
        if (s == null) { Debug.LogWarning("Could not find a sprite named: " + fileName + " in the Resorces/DialogueSprites folder"); HideSpriteXLLilith(); HideSpriteXLOther(); } //Hide both if name is invalid

        return null;
    }               //Load and set an appropriate sprite for Lilith and or other
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

    #endregion


    #region Healthbar related
    public void InitialiseHealthBoss(bossHealth data)                        //initialises the boss healthbar. Happens once - when boss is enabled
    {
        bossHealth = data;
        bossShieldBar.maxValue = data.GetMaxHealth();
        bossHealthBar.maxValue = data.coreHealth;
        bossHealthBar.value = bossHealthBar.maxValue;
        boxBossBar.SetActive(true);
    }
    public void UpdateHealthDisplays()                                       //On Update() refreshes player health and boss health
    {
        UpdateHealthPlayer();
        UpdateHealthBoss();
    }
    public void UpdateHealthPlayer()
    {
        playerHealthBar.value = PlayerCombat.instance.GetHealth();
        SetPlayerPortrait(PlayerCombat.instance.GetHealth(), PlayerCombat.instance.GetMaxHealth()); //setting the player portrait from the base of avalible health-reflecting portraits

    }
    public void UpdateHealthBoss()
    {
        if (bossHealth != null)
        {
            bossShieldBar.maxValue = bossHealth.GetMaxHealth();
            bossShieldBar.value = bossHealth.GetHealth();
        }
    }
    public void CleanupHealthBoss(bool isBossKilled)                                //After the QTE - if the boss is dead hide the healthbars, deal damage otherwise 
    {
        //Hide healthbars upon boss death
        if (isBossKilled)
        {
            //Debug.LogWarning("Boss defeated!");
            bossHealth.gameObject.SetActive(false);
            boxBossBar.SetActive(false);
        }
        else 
        {
            //Refill shieldbar and main health
            bossHealth.isCoreExposed = false;
            bossHealth.SetHealth(bossHealth.GetMaxHealth());
            bossShieldBar.value = bossShieldBar.maxValue;
            bossHealthBar.value = bossHealth.coreHealth;
        }
    }


    public void SetPlayerPortrait(float health, float maxHealth)            //Changes the player portrait shown next to the healthbar
    {
        float hp_normalised = health / maxHealth;
        int whichPortrait = (int)Mathf.Round(Mathf.Lerp(0, playerPortraits.Length - 1, hp_normalised));  //from the list of portraits, grab an apropriate one
        playerPortrait.sprite = playerPortraits[whichPortrait];
    }

    #endregion

    #region Dialogue Choices

    public void SetDialogueChoicesTest(string text) => SetDialogueChoices(text, text, "DebugA", "DebugB");
    public void SetDialogueChoices(string textA, string textB, string AAddress, string BAddress)
    {

        Debug.Log("Dialogue Choices set to");
        Debug.Log(textA + " : " + textB);
        txtChoiceA.text = textA; txtChoiceB.text = textB;
        choiceA_ID = AAddress; choiceB_ID = BAddress;

        //enabling the choice displays once set
        if (!boxTextDisplay.gameObject.activeInHierarchy) boxTextDisplay.gameObject.SetActive(true);
        txtChoiceA.transform.parent.parent.gameObject.SetActive(true);
    }
    public void SetDialogueChoiceHidden() => txtChoiceA.transform.parent.parent.gameObject.SetActive(false);

    public void OnDialogueChoiceA() => OnDialogueChoice(true);
    public void OnDialogueChoiceB() => OnDialogueChoice(false);
    public void OnDialogueChoice(bool isA)
    {
        if (isA) NodeCurrent = FindSaveDataID(choiceA_ID, dataTemp);
        else NodeCurrent = FindSaveDataID(choiceB_ID, dataTemp);

        SetDialogueChoiceHidden();
        isWaiting = false;
    }



    #endregion

    public void FadeOut()
    {
        anim.SetTrigger("Out");
    }
    public void FadeIn()
    {
        anim.SetTrigger("In");
    }
    public void CutToBlack()
    {
        anim.SetTrigger("Cut");
    }


    public void QuitToWindows() 
    {
        AudioManager.instance.PlaySFX("MenuClick");
        Application.Quit(); 
    }
    public IEnumerator OutroSequenceWithTimings() 
    {
        ToggleHealthbar(false);
        OutroCinematicObject.SetActive(true);
        FadeIn();
        yield return new WaitForSeconds(1);
        if (OutroDialogue1 != null) DialogueShow(OutroDialogue1, true); else Debug.LogWarning("OutroDialogue with backstory not assigned in UI!");

        while(dialogueCur!=null) yield return new WaitForEndOfFrame();   //waiting for the dialogue to finish, before proceeding
        Debug.Log("Backstory speech finished");
        CutToBlack(); //cut to black either too fast or glitched. TEST - Milla
        //play stab sfx wiat till sfx finished
        //play scream sfx
        //once the scream done finished, show dialgoue
        if (OutroDialogue2 != null) DialogueShow(OutroDialogue2, true); else Debug.LogWarning("outro-most dialogue not assigned in UI!");

        while (dialogueCur!=null) yield return new WaitForEndOfFrame();   //waiting for the dialogue to finish, before proceeding
        Debug.Log("Karl finished talking about death and taxes, game finished!");

        //this is where the scrolling credits will go!

        GameManager.LoadMenu();
    }


    #region buttons and element toggles
    public void TogglePauseMenu() //Toggle pause menu
    {
        boxPause.SetActive(!boxPause.activeInHierarchy);
        PlayerCombat.instance.gameObject.GetComponent<Animator>().SetBool("isAttacking", false);    //Interupting attack combos- M

        //Pausing game inputs and game time
        if (boxPause.activeInHierarchy) PlayerMovement.instance.SetLockMovement();
        else { PlayerMovement.instance.SetUnLockMovement(); }
        GameManager.instance.SetPause(boxPause.activeInHierarchy);

        //Hiding other UI elements while paused
        if (boxSettings.activeInHierarchy) boxSettings.SetActive(false);
        if (RevengeList.activeInHierarchy) RevengeList.SetActive(false);
    }     
    public void BackToMenu() 
    {
        AudioManager.instance.PlaySFX("MenuClick");
        GameManager.LoadMenu();
    }

    public void InputPause(InputAction.CallbackContext obj) => TogglePauseMenu();
    public void ToggleSettings()                                                                                            //Toggle settings menu
    {
        AudioManager.instance.PlaySFX("MenuClick");
        boxSettings.SetActive(!boxSettings.activeInHierarchy); 
    }                                 
    public void ToggleHealthbar() { boxHealthbar.SetActive(!boxHealthbar.activeInHierarchy); }                              //Toggle the player healthbar display
    public void ToggleHealthbar(bool state) { boxHealthbar.SetActive(state); }                              //Toggle the player healthbar display
    public void ToggleQTEScreen()                                                                                           //Toggle QTE screen and freeze player movement
    {
        if(boxQTE.activeSelf) PlayerMovement.instance.SetLockMovement();
        else { PlayerMovement.instance.SetUnLockMovement(); }

        boxQTE.SetActive(!boxQTE.activeInHierarchy); 
    }
    public void EnableLostScreen()
    {
        boxLost.SetActive(true);
        Invoke("BackToMenu", GameManager.instance.DeathReloadTime);
    }

    public void ToggleRevengeList()     //Added pausing when Revenge list shown
    {
        PlayerCombat.instance.gameObject.GetComponent<Animator>().SetBool("isAttacking", false);    //Interupting attack combos- M
        RevengeList.SetActive(!RevengeList.activeInHierarchy);
        GameManager.instance.SetPause(RevengeList.activeInHierarchy);
    }
    public void SetSpriteXLLilith(Sprite s) { XLPortraitLilith.gameObject.SetActive(true); XLPortraitLilith.sprite = s; }
    public void SetSpriteXLOther(Sprite s) { XLPortraitOther.gameObject.SetActive(true); XLPortraitOther.sprite = s; }
    public void HideSpriteXLLilith() { XLPortraitLilith.gameObject.SetActive(false); }
    public void HideSpriteXLOther() { XLPortraitOther.gameObject.SetActive(false); }
    public void ShowSpriteXLLilith() { XLPortraitLilith.gameObject.SetActive(true); }
    public void ShowSpriteXLOther() { XLPortraitOther.gameObject.SetActive(true); }

    public void SetTypingSpeed(float typeRate) => Settings.instance.typingWait = Mathf.Lerp(0.04f,0.01f, typeRate); //left to slow, right to fast
    public void SetVolume(float value) { float t = Mathf.Lerp(-80, 0, value); Debug.Log(t); AudioMixer.SetFloat("masterVolume", t); Settings.instance.volume = t; }  //left to mute, right to loud
    public void SetVolumeMusic(float value) { float t = Mathf.Lerp(-80, 0, value); Debug.Log(t); AudioMixer.SetFloat("musicVolume", t); Settings.instance.volumeMusic = t; }  //left to mute, right to loud
    public void SetVolumeSFX(float value) { float t = Mathf.Lerp(-80, 0, value); Debug.Log(t); AudioMixer.SetFloat("sfxVolume", t); Settings.instance.volumeSFX = t; }  //left to mute, right to loud

    public void PlayOutroSequence() => StartCoroutine("OutroSequenceWithTimings");
    
    


    #endregion
}
