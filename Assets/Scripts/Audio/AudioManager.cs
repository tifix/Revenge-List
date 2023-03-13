using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public AudioSource musicSource;
    public AudioSource sfxSource;

    private float lowPitchRange = .95f;
    private float highPitchRange = 1.05f;

    public static AudioManager instance = null;

    private void Awake()
    {
        // If there is not already an instance of AudioManager
        if (instance == null)
        {
            // set it to this.
            instance = this;
        }
        // If an instance already exists
        else if (instance != this)
        {
            // Obliterate whatever this object is to enforceth the singleton.
            Destroy(gameObject);
        }

        //Set AudioManager to DontDestroyOnLoad, ensuring that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
    }

    // Plays a clip through the sfxSource
    public void Play(AudioClip clip)
    {
        sfxSource.clip = clip;
        sfxSource.Play();
    }

    // Plays a clip through the musicSource
    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

}
