using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSFX : MonoBehaviour
{
    public AudioClip sfxName;

    private void Awake()
    {
        AudioManager.instance.sfxSource.PlayOneShot(sfxName);
    }
}
