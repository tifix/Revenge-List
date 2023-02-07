using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameData : MonoBehaviour
{
    public float testScore = 0;
    public static GameData instance;
    [SerializeField] private bool isGamePaused=false;

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

    #region Scene Switching
    public static void LoadScene(int _SceneNumber) => SceneManager.LoadScene(_SceneNumber);
    public static void LoadScene(string _SceneName) => SceneManager.LoadScene(_SceneName);
    public static void LoadMenu() => LoadScene("Menu");
    //public void LoadGame() => LoadScene();

    public void QuitToWindows() { Application.Quit(); }

    #endregion
}
