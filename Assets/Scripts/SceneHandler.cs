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
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(index, LoadSceneMode.Single); //, LoadSceneMode.Single

        //Ensuring valid reference, debugging otherwise
        try { Debug.Log(loadSlider.gameObject.name + " Loading screen obtained properly. Default loading screen valid"); }
        catch { Debug.LogWarning("Loading bar object missing! Please grab it from UI prefab - Loading Panel"); }

        //Resetting values for immediate display
        progressText.text = "Now Loading!";
        loadSlider.value = 0;
        loadSlider.GetComponentInParent<CanvasGroup>().alpha = 1;
        yield return new WaitForSecondsRealtime(0.5f);

        //Loading with text display and progress slide.
        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            Debug.Log("Loading! " + progress);
            //Percentage loaded
            loadSlider.value = progress;
            if(Mathf.RoundToInt(progress * 100f) % 2==0 ) yield return new WaitForSeconds(0.15f);
            if(Mathf.RoundToInt(progress * 100f) % 3==0 ) yield return new WaitForSeconds(0.15f);
            progressText.text = (Mathf.RoundToInt(progress * 100f) ).ToString();

            //yield return null;
            yield return new WaitForSecondsRealtime(0.1f);
        }
        loadSlider.value = 100;
        yield return new WaitForSecondsRealtime(0.5f);
        loadSlider.GetComponentInParent<CanvasGroup>().alpha = 0;
    }
}
