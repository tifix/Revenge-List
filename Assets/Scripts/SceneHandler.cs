using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public static SceneHandler instance;
    public Slider loadSlider;
    public TextMeshProUGUI progressText;

    public void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public static void LoadScene(int index) => instance.LoadLevel(index);
    public void LoadLevel(int index)
    {
        StartCoroutine(LoadLevelAsync(index));
    }

    IEnumerator LoadLevelAsync(int index)   // plugged into UI - M
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single);
        
        //Ensuring valid reference, debugging otherwise
        try { Debug.Log(loadSlider.gameObject.name + " Loading screen obtained properly. Default loading screen valid"); }
        catch { Debug.LogWarning("Loading bar object missing! Please grab it from UI prefab - Loading Panel"); }

        //Resetting values for immediate display
        progressText.text = "Now Loading!";
        loadSlider.value = 0;
        loadSlider.GetComponentInParent<CanvasGroup>().alpha = 1;
        yield return null;

        //Loading with text display and progress slide.
        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            Debug.Log("Loading! " + progress);
            //Percentage loaded
            loadSlider.value = progress;
            progressText.text = (progress * 100f).ToString();

            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        loadSlider.GetComponentInParent<CanvasGroup>().alpha = 0;
    }
}
