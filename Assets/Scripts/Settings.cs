using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * don't destroy with DATA ONLY for making menu settings and game settings persistent between scenes -audio not blaring once you leave the menu and whatnot
 */

public class Settings : MonoBehaviour
{
    public float typingWait=0.03f;
    public float volume = 0;
    public float volumeSFX = 0;
    public float volumeMusic = 0;
    public static Settings instance;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else Destroy(gameObject);
    }
}
