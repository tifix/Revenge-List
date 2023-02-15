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

    public void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(this);
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

    public void TogglePause() => SetPause(!isGamePaused);
    public void SetPause(bool targetState)
    {
        isGamePaused = targetState;
        if (isGamePaused) Time.timeScale = 0;
        else Time.timeScale = 1;
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

    #region Scene Switching
    public static void LoadScene(int _SceneNumber) => SceneManager.LoadScene(_SceneNumber);
    public static void LoadScene(string _SceneName) => SceneManager.LoadScene(_SceneName);
    public static void LoadMenu() => SceneManager.LoadScene("Menu");
    //public void LoadGame() => LoadScene();

    public void QuitToWindows() { Application.Quit(); }

    #endregion
}
