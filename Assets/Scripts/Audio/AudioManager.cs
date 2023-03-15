using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public AudioSource musicSource;
    public AudioSource sfxSource;

    //private float lowPitchRange = .95f;
    //private float highPitchRange = 1.05f;

    public static AudioManager instance = null;

    public AudioClip WalkMC;
    public AudioClip DashMC;
    public AudioClip BgMusic;
    public AudioClip BossTrack;
    public AudioClip MenuClick;
    public AudioClip QteTrack;
    public AudioClip EnvelopeThrow;
    public AudioClip KickSFX;
    public AudioClip FlipSFX;


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
        if (sfxSource.isPlaying)
        {
            sfxSource.Stop();
        }
        sfxSource.clip = clip;
        sfxSource.Play();
    }

    // Plays a clip through the musicSource
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
        musicSource.clip = clip;
        musicSource.Play();
    }
    public void PlaySFX(String name)
    {
        switch (name)
        {
            case "WalkMC":
                sfxSource.clip = WalkMC;
                break;
            case "MenuClick":
                sfxSource.clip = MenuClick;
                break;
            case "Dash":
                sfxSource.clip = DashMC;
                break;
            case "Throw":
                sfxSource.clip = EnvelopeThrow;
                break;
            case "Flip":
                sfxSource.clip = FlipSFX;
                break;
            case "Kick":
                sfxSource.clip = KickSFX;
                break;
            default:
                Debug.LogWarning("Incorrect name " + name + " check spelling");
                break;
        }

        sfxSource.Play();
    }
    public void PlayMusic(String name)
    {
        switch (name)
        {
            case "BossTrack":
                musicSource.volume = 1f;
                musicSource.clip = BossTrack;
                break;
            case "BgMusic":
                musicSource.volume = 0.5f;
                musicSource.clip = BgMusic;
                break;
            case "QteTrack":
                musicSource.volume = 1.0f;
                musicSource.clip = QteTrack;
                break;
            default:
                Debug.LogWarning("Incorrect name " + name + " check spelling");
                break;
        }

        musicSource.Play();
    }
}
