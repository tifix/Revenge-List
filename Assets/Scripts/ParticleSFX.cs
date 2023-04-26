using CartoonFX;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSFX : MonoBehaviour
{

    public ParticleSystem mySubEmitter;
    public AudioClip onDeathSound;

    void Update()
    {
        if (!onDeathSound) { return; }
        if(mySubEmitter.isStopped)
        {
            AudioManager.instance.sfxSource.PlayOneShot(onDeathSound);
        }
    }
}


