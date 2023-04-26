using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using UnityEngine.UI;
//using TMPro;

public class GameManager : MonoBehaviour
{
    public float testScore = 0;
    public static GameManager instance;
    [SerializeField] private bool isGamePaused = false;
    [SerializeField] private bool isGameWon = false;
    [SerializeField] private bool isGameLost = false;
    [SerializeField] public float DeathReloadTime;                                      //time in seconds(realtime) before menu reloads on game over.
    
    CinemachineVirtualCamera cam;
    CinemachineBasicMultiChannelPerlin perlin;
    CinemachineFramingTransposer frame;
    float deadZone = 0.2f;
    bool isCurrentlyShaking = false;    //is the camera shaking at the moment - used for removing multiple shakes at once

    [Header("Cheats")]
    [Tooltip("adjust game speed to analyse details... in detail"), Range(0, 2f)] public float cheat_TimeScale = 1;
    [Tooltip("enable to skip every dialogue encountered")] public bool cheat_FastForwardDialogue = false;
    [Tooltip("enable to stop taking damage")] public bool cheat_GodMode = false;
    [Tooltip("enable to warp to Karl arena")] public bool cheat_WarpToKarl = false;
    [Tooltip("enable to warp to Microwave arena")] [SerializeField] bool cheat_WarpToMicrowave = false;
    [Tooltip("enable to instantly complete a boss phase")] public bool cheat_SkipBossPhase = false;
    [Tooltip("fast forward to when Karl is defeated")] public bool cheat_SkipToOutro = false;
    [Tooltip("fast forward to last phase of microwave")] public bool cheat_SkipToMicroEnd = false;
    [Tooltip("Go stab yourself")] public bool cheat_LowHPPlayer = false;
    //[Tooltip("enable to instantly kill Karl. :(")] public bool cheat_KillBoss = false;
    [Tooltip("enable to make QTEs impossible to lose")] public bool cheat_QTEAlwaysWin = false;


    public void Awake()
    {
        if (instance == null) 
        { 
            instance = this; 
            //DontDestroyOnLoad(gameObject); 
        }
        else Destroy(this);
    }
    private void Start()
    {
        if(GetCamera())             //Find the camera object. If found and valid, track player
            CamFollowPlayer();
        if(UI.instance!=null) UI.instance.FadeIn(); //Added a null checker -M
        AudioManager.instance.PlayMusic("BgMusic");
    }

    // Update is called once per frame
    public void KillPlayer()
    {
        Debug.LogWarning("You are now Dead. Or something");
    }
    public void FixedUpdate()
    {
        testScore += Time.fixedDeltaTime;
    }
    private void Update()
    {
        if (cheat_WarpToKarl) { PlayerMovement.instance.gameObject.transform.position = new Vector3(12.7f, 1.6f, -5.2f); cheat_WarpToKarl = false; }
        if (cheat_WarpToMicrowave) { PlayerMovement.instance.gameObject.transform.position = new Vector3(6, 2, -0.13f); cheat_WarpToMicrowave = false; }
        if (cheat_TimeScale!=1) { Time.timeScale = cheat_TimeScale; }
        if (cheat_SkipToOutro) 
        {
            GameObject.Find("Boss").transform.GetChild(1).gameObject.SetActive(true);
            //KarlBoss K = GameObject.Find("Boss").GetComponentInChildren<KarlBoss>();
            //K.BossDefeated();
            GameObject.Find("Boss").transform.GetChild(2).gameObject.SetActive(false);
            PlayerMovement.instance.gameObject.transform.position = new Vector3(24.0f, 1.6f, 1.2f);
            cheat_SkipToOutro = false;
        }
        if (cheat_SkipToMicroEnd) 
        {
            GameObject.FindGameObjectWithTag("Boss").transform.GetChild(1).gameObject.SetActive(true);
            GameObject.FindObjectOfType<TutorialBoss>().SkipToLastPhase(); 
            cheat_SkipToMicroEnd = false; 
        }
    }
    
    public void TogglePause() => SetPause(!isGamePaused);

    public bool IsGamePaused()
    {
        return isGamePaused;
    }
    public void SetPause(bool targetState)
    {
        //Escape takes priority over all other types of pauses
        //esc and esc works fine
        //if esc we can still open revenge list in the back
        //if revenge list and esc and esc the revenge list closes, fine behaviour

        isGamePaused = targetState;
        if (isGamePaused) Time.timeScale = 0;
        else Time.timeScale = 1;
    }
    public void WarpToKarl() => cheat_WarpToKarl = true;

    public bool GetCamera() 
    {
        try 
        { 
            cam = FindObjectOfType<CinemachineVirtualCamera>(); 
            cam.m_Lens.OrthographicSize = 4; 
        }
        catch { return false; }             
        
        

        perlin = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.m_AmplitudeGain = 0;
        perlin.m_FrequencyGain = 0;

        frame = cam.GetCinemachineComponent<CinemachineFramingTransposer>();
        frame.m_DeadZoneHeight = deadZone;

        return true;
    }

    public void StartQuickTimeEventEverything(string name) 
    {
        
        UI.instance.ToggleQTEScreen();
        QTEManager.instance.QTEStart();

        StartCoroutine(StartMusic(name));
    }

    // Set win State
    public void SetWon(bool targetState)
    {
        isGameWon = targetState;
        if (isGameWon) Time.timeScale = 0;
        else Time.timeScale = 1;
    }

    // Set lost State
    public void SetLost(bool targetState)
    {
        isGameLost = targetState;
        UI.instance.EnableLostScreen();
    }

    public void LockCamera(Transform pos)
    {
        frame.m_DeadZoneHeight = 0;
        frame.m_DeadZoneWidth = 0;
        cam.Follow = pos;
        StartCoroutine(IncreaseCamSize(6));
    }

    public void CamFollowPlayer()
    {
        frame.m_DeadZoneHeight = deadZone;
        frame.m_DeadZoneWidth = deadZone;
        cam.Follow = FindObjectOfType<PlayerMovement>().transform;
        StartCoroutine(DecreaseCamSize(4));
    }

    public void CameraInstantified() => GameManager.instance.GetCamera();


    public void Noise(float amplitude, float frequency)
    {
        perlin.m_AmplitudeGain = amplitude;
        perlin.m_FrequencyGain = frequency;
    }

    public void StopNoise()
    {
        perlin.m_AmplitudeGain = 0;
        perlin.m_FrequencyGain = 0;
    }

    public void CallShake(float shakeIntensity, float shakeTiming)
    {
        StartCoroutine(ScreenShake(shakeIntensity, shakeTiming));
    }

    IEnumerator ScreenShake(float shakeIntensity = 5f, float shakeTiming = 0.5f)
    {
        if (isCurrentlyShaking) yield break;
        isCurrentlyShaking = true;

        Noise(1, shakeIntensity);
        yield return new WaitForSeconds(shakeTiming);
        Noise(0, 0);

        isCurrentlyShaking = false;
    }

    IEnumerator IncreaseCamSize(int x)
    {
        while(cam.m_Lens.OrthographicSize < x)
        {
            cam.m_Lens.OrthographicSize += 10 * Time.fixedDeltaTime;
            yield return null;
        }
    }

    IEnumerator DecreaseCamSize(int x)
    {
        while (cam.m_Lens.OrthographicSize > x)
        {
            cam.m_Lens.OrthographicSize -= 10 * Time.fixedDeltaTime;
            yield return null;
        }
    }

    IEnumerator StartMusic(string name)
    {
        yield return new WaitForSeconds(4);
        AudioManager.instance.PlayMusic(name);
    }

    //I recommend looking into async loading of scenes, this would allow us to have a scene for transitioning (loading screen)
    //When loading a scene, you can choose the mode to load it in, a pretty cool one is "Additive", which basically just allows
    //2 or more scenes to be loaded at once and be able to interact
    //This could be useful to have all the persitant data objects in said scene + the loading screen itself
    //Oh, loading the screen in an async manner, requieres the use of a coroutine - AV

    public static void LoadScene(int _SceneNumber) => SceneHandler.LoadScene(_SceneNumber);
    public static void LoadScene(string _SceneName) => SceneHandler.LoadScene(SceneManager.GetSceneByName(_SceneName).buildIndex);
    public static void LoadMenu() => SceneHandler.LoadScene(0);
    public void QuitToWindows() { Application.Quit(); }


    public void Log(string text)
    {
        Debug.LogWarning(text);
    }
}


public class GameChoices 
{
    public bool didInsultDog = false;
    public bool didInsultBarbeque = false;


}
