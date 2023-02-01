using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Beats
{
    Pause, Up, Down, Left, Right
}

[CreateAssetMenu(fileName = "beatMap", menuName = "QTE")]
public class QTEObject : ScriptableObject
{
    [Range(0f, 10f)]
    public int speed;
    [Range(0f, 10f)]
    public float delay;
    public List<Beats> beats = new List<Beats>();

    public float GetDelay() { return delay; }
    public List<Beats> GetBeats() { return beats; }
}
