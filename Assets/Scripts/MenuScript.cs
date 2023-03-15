using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{

    public void Start()
    {

        PlayTrack("CreditsTrack");
    }

    public GameObject CreditsPanel;

    public void ToggleCreditsPanel()=>CreditsPanel.SetActive(!CreditsPanel.activeInHierarchy);
    
    public void PlayTrack(string name)
    {
        AudioManager.instance.PlayMusic(name);
    }


}
