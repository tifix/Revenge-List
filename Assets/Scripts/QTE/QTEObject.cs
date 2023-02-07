using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Beats
{
    Pause, Up, Down, Left, Right
}

[System.Serializable]
public struct BeatType
{
    public Beats myBeat;
    [Range(1,10)]
    public int speedMod;

    public BeatType(Beats b, int s)
    {
        myBeat = b;
        speedMod = s;
    } 
}

[CreateAssetMenu(fileName = "beatMap", menuName = "QTE")]
public class QTEObject : ScriptableObject
{
    [Range(0f, 10f)]
    public int speed;
    [Range(0f, 10f), Tooltip("Spwan rate")]
    public float delay;
    public List<BeatType> beats = new List<BeatType>();

    public float GetDelay() { return delay; }
    public List<BeatType> GetBeats() { return beats; }
}
