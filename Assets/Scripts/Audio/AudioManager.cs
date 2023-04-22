using System;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public AudioSource musicSource;
    public AudioSource musicSource2;
    public AudioSource sfxSource;

    //private float lowPitchRange = .95f;
    //private float highPitchRange = 1.05f;

    public static AudioManager instance = null;

    public AudioClip WalkMC;
    public AudioClip DashMC;
    public AudioClip BgMusic;
    public AudioClip BossTrack;
    public AudioClip TutorialTrack;
    public AudioClip MenuClick;
    public AudioClip MenuClickError;
    public AudioClip QteTrack;
    public AudioClip QteHit;
    public AudioClip QteMiss;
    public AudioClip EnvelopeThrow;
    public AudioClip KickSFX;
    public AudioClip FlipSFX;
    public AudioClip DogBark;
    public AudioClip DogWhine;
    public AudioClip CreditsTrack;
    public AudioClip HitSFX;
    public AudioClip KarlScream;
    public AudioClip KarlHit;
    public AudioClip MicroBeep;
    public AudioClip SteakLand;
    public AudioClip UnrollList;
    public AudioClip FlyingGnome;
    public AudioClip MicroHit;
    
    private void Awake()
    {
        // If there is not already an instance of AudioManager
        if (instance == null)
        {
            // set it to this.
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        // If an instance already exists
        else if (instance != this)
        {
            // Obliterate whatever this object is to enforceth the singleton.
            Destroy(gameObject);
        }

        //Set AudioManager to DontDestroyOnLoad, ensuring that it won't be destroyed when reloading our scene.
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

    public void PlayClickEffect() =>sfxSource.PlayOneShot(MenuClick); 
    public void PlayClickBadEffect() =>sfxSource.PlayOneShot(MenuClickError); 
    

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
            case "ClickError":
                sfxSource.clip = MenuClickError;
                break;
            case "qteHit":
                sfxSource.clip = QteHit;
                break;
            case "qteMiss":
                sfxSource.clip = QteMiss;
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
            case "Bark":
                sfxSource.clip = DogBark;
                break;
            case "DogWhine":
                sfxSource.clip = DogWhine;
                break;
            case "Hit":
                sfxSource.clip = HitSFX;
                break;
            case "KarlScream":
                sfxSource.clip = KarlScream;
                break;
            case "KarlHit":
                sfxSource.clip = KarlHit;
                break;
            case "MicroBeep":
                sfxSource.clip = MicroBeep;
                break;
            case "SteakLand":
                sfxSource.clip = SteakLand;
                break;
            case "UnrollList":
                sfxSource.clip = UnrollList;
                break;
            case "Weee!":
                sfxSource.clip = FlyingGnome;
                break;
            case "MicroHit":
                sfxSource.clip = MicroHit;
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
            case "TutorialTrack":
                musicSource.volume = 1f;
                musicSource.clip = TutorialTrack;
                break;
            case "BgMusic":
                musicSource.volume = 1f;
                musicSource.clip = BgMusic;
                break;
            case "MenuClick":
                sfxSource.clip = MenuClick;
                break;
            case "QteTrack":
                musicSource.volume = 1f;
                musicSource.clip = QteTrack;
                break;
            case "CreditsTrack":
                musicSource.volume = 1f;
                musicSource.clip = CreditsTrack;
                break;
            default:
                Debug.LogWarning("Incorrect name " + name + " check spelling");
                break;
        }

        musicSource.Play();
    }

    // NYI - Work in Porgress - function is for fading out current track and into the track we want next
    public void SwapMusic(String name)
    {
        musicSource2.volume = 0;

        switch (name)
        {
            case "BossTrack":
                //musicSource2.volume = 1f;
                musicSource.clip = BossTrack;
                break;
            case "BgMusic":
                //musicSource2.volume = 1f;
                musicSource.clip = BgMusic;
                break;
            case "QteTrack":
                //musicSource2.volume = 1f;
                musicSource.clip = QteTrack;
                break;
            default:
                Debug.LogWarning("Incorrect name " + name + " check spelling");
                break;
        }


        musicSource2.Play();

        musicSource.volume = Mathf.Lerp(musicSource.volume, 0f, 0f * Time.deltaTime);

        musicSource2.volume = Mathf.Lerp(musicSource2.volume, 1f, 0f * Time.deltaTime);

        musicSource.Stop();
    }
}
