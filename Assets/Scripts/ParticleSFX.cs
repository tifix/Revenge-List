using CartoonFX;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSFX : MonoBehaviour
{

    private ParticleSystem parentParticleSystem;

    private int currentNumberOfParticles;

    public AudioClip onBirthSound;
    public AudioClip onDeathSound;

    void Start()
    {
        parentParticleSystem = this.GetComponent<ParticleSystem>();
        if(parentParticleSystem == null)
        {
            Debug.LogError("Missing Particle System", this);
        }
    }

    void Update()
    {
        var amount = Mathf.Abs(currentNumberOfParticles - parentParticleSystem.particleCount);

        if(parentParticleSystem.particleCount < currentNumberOfParticles)
        {
            //Play Death Sound
            StartCoroutine(PlaySound(onDeathSound));
        }

        if(parentParticleSystem.particleCount > currentNumberOfParticles)
        {
            //play birth sound
            StartCoroutine(PlaySound(onBirthSound));
        }

        currentNumberOfParticles = parentParticleSystem.particleCount;
    }

    private IEnumerator PlaySound(AudioClip clip)
    {
        yield return new WaitForSeconds(0.05f);

        AudioManager.instance.sfxSource.PlayOneShot(clip);
    }
}


