using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Beats
{
    Up, Down, Left, Right
}

[System.Serializable]
public struct BeatType
{
    public float timing;
    public Beats myBeat;
    public int speed;

    public BeatType(float t, Beats b, int s)
    {
        timing = t;
        myBeat = b;
        speed = s;
    }
}

[CreateAssetMenu(fileName = "beatMap", menuName = "QTE")]
public class QTEObject : ScriptableObject
{
    public int bpm = 120;
    public float startTime;
    public float endTime = 30;
    public List<BeatType> beats = new List<BeatType>();
    public int beatsForWin;

    public float GetBpm() { return bpm; }
    public int GetBeatsForWin() { return beatsForWin; }
    public List<BeatType> GetBeats() { return beats; }
}
