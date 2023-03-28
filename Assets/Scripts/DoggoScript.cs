using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoggoScript : MonoBehaviour
{
    public void Play(String name)
    {
        AudioManager.instance.PlaySFX(name);
    }
}
