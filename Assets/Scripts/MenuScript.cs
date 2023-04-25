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

    public void ToggleCreditsPanel()
    {
        AudioManager.instance.PlaySFX("MenuClick");
        CreditsPanel.SetActive(!CreditsPanel.activeInHierarchy);
    }

    public void ToggleSettingsPanel()
    {
        AudioManager.instance.PlaySFX("MenuClick");
        SettingsPanel.SetActive(!SettingsPanel.activeInHierarchy);
    }


    public void SetTypingSpeed(float typeRate) => Settings.instance.typingWait = Mathf.Lerp(0.04f, 0.01f, typeRate); //left to slow, right to fast
    public void SetVolume(float value) { float t = Mathf.Lerp(-70, 0, value); Debug.Log(t); AudioMixer.SetFloat("masterVolume", t); Settings.instance.volume = t; AudioManager.instance.PlayClickEffect(); }  //left to mute, right to loud
    public void SetVolumeMusic(float value) { float t = Mathf.Lerp(-70, 0, value); Debug.Log(t); AudioMixer.SetFloat("musicVolume", t); Settings.instance.volumeMusic = t; AudioManager.instance.PlayClickEffect(); }  //left to mute, right to loud
    public void SetVolumeSFX(float value) { float t = Mathf.Lerp(-70, 0, value); Debug.Log(t); AudioMixer.SetFloat("sfxVolume", t); Settings.instance.volumeSFX = t; AudioManager.instance.PlayClickEffect(); }  //left to mute, right to loud


    public void PlayTrack(string name)
    {
        AudioManager.instance.PlayMusic(name);
    }


}
