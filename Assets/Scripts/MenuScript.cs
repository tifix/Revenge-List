using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MenuScript : MonoBehaviour
{

    public GameObject CreditsPanel;
    public GameObject SettingsPanel; 
    [SerializeField] AudioMixer AudioMixer;

    public void Start()
    {

        PlayTrack("CreditsTrack");
    }

    public void ToggleCreditsPanel()=>CreditsPanel.SetActive(!CreditsPanel.activeInHierarchy);
    public void ToggleSettingsPanel()=> SettingsPanel.SetActive(!SettingsPanel.activeInHierarchy);

    public void SetTypingSpeed(float typeRate) => Settings.instance.typingWait = Mathf.Lerp(0.04f, 0.01f, typeRate); //left to slow, right to fast
    public void SetVolume(float value) { float t = Mathf.Lerp(-80, 0, value); Debug.Log(t); AudioMixer.SetFloat("masterVolume", t); Settings.instance.volume = t; }  //left to mute, right to loud


    public void PlayTrack(string name)
    {
        AudioManager.instance.PlayMusic(name);
    }


}
