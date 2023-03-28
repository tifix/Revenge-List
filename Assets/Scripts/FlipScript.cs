using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipScript : MonoBehaviour
{
    public void Play(String name)
    {
        AudioManager.instance.PlaySFX(name);
    }
}
