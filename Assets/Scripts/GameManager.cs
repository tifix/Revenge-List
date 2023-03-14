using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    bool isCurrentlyShaking = false;    //is the camera shaking at the moment - used for removing multiple shakes at once

    [SerializeField] bool cheat_WarpToKarl = false;
    public bool cheat_SkipBossPhase = false;
    public bool cheat_FastForwardDialogue = false;

    public void Awake()
    {
        if (instance == null) 
        { 
            instance = this; 
            DontDestroyOnLoad(gameObject); 
        }
        else Destroy(this);
    }
    private void Start()
    {
        AudioManager.instance.Play("BgMusic");
        GetCamera();
        CamFollowPlayer();
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
    }
    public void TogglePause() => SetPause(!isGamePaused);
    public void SetPause(bool targetState)
    {
        isGamePaused = targetState;
        if (isGamePaused) Time.timeScale = 0;
        else Time.timeScale = 1;
    }

    public void GetCamera() 
    {
        cam = FindObjectOfType<CinemachineVirtualCamera>();
        cam.m_Lens.OrthographicSize = 4;

        perlin = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.m_AmplitudeGain = 0;
        perlin.m_FrequencyGain = 0;
    }

    public static void StartQuickTimeEventEverything() 
    {
        UI.instance.ToggleQTEScreen();
        QTEManager.instance.QTEStart();
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
        if (isGameLost) Time.timeScale = 0;
        else Time.timeScale = 1;
        UI.instance.EnableLostScreen();
    }

    public void LockCamera(Transform pos)
    {
        cam.Follow = pos;
        StartCoroutine(IncreaseCamSize(5));
    }

    public void CamFollowPlayer()
    {
        cam.Follow = FindObjectOfType<PlayerMovement>().transform;
        StartCoroutine(DecreaseCamSize(4));
    }

    public void CameraInstantified() => GameManager.instance.GetCamera();


    public void Noise(float amplitude, float frequency)
    {
        perlin.m_AmplitudeGain = amplitude;
        perlin.m_FrequencyGain = frequency;
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

    //I recommend looking into async loading of scenes, this would allow us to have a scene for transitioning (loading screen)
    //When loading a scene, you can choose the mode to load it in, a pretty cool one is "Additive", which basically just allows
    //2 or more scenes to be loaded at once and be able to interact
    //This could be useful to have all the persitant data objects in said scene + the loading screen itself
    //Oh, loading the screen in an async manner, requieres the use of a coroutine - AV
    #region Scene Switching
    public static void LoadScene(int _SceneNumber) => SceneManager.LoadScene(_SceneNumber);
    public static void LoadScene(string _SceneName) => SceneManager.LoadScene(_SceneName);
    public static void LoadMenu() => SceneManager.LoadScene("Menu");
    //public void LoadGame() => LoadScene();

    public void QuitToWindows() { Application.Quit(); }

    #endregion
}
