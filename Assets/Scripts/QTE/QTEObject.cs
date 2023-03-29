using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if (UNITY_EDITOR)
using UnityEditor;
#endif

public enum Beats
{
    Up, Down, Left, Right
}

[CreateAssetMenu(fileName = "beatMap", menuName = "QTE")]
public class QTEObject : ScriptableObject
{
    [System.Serializable]
    public struct BeatType
    {
        [Range(0,360), Tooltip("Timing / 2 = Song Real Time\n1 = at 0.5s of the song\n20 = at 10s\n45 = at 22.5s\n60 = at 30s")]
        public int timing;
        public Beats myBeat;
        [Range(1, 6)]
        public int speed;

        public BeatType(int t, Beats b, int s)
        {
            timing = t;
            myBeat = b;
            speed = s;
        }
    }

    public int bpm = 120;
    public float startTime;
    public float endTime = 30;
    public List<BeatType> beats = new List<BeatType>();
    public int beatsForWin;
    public AudioClip song;

    public float GetBpm() { return bpm; }
    public int GetBeatsForWin() { return beatsForWin; }
    public List<BeatType> GetBeats() { return beats; }
}

#if (UNITY_EDITOR)
[CustomEditor(typeof(QTEObject))]
public class QTEditor : Editor
{
    public SerializedProperty bpm, startTime, endTime, beats, beatsForWin, song;

    private void OnEnable()
    {
        bpm = serializedObject.FindProperty("bpm");
        startTime = serializedObject.FindProperty("startTime");
        endTime = serializedObject.FindProperty("endTime");
        beats = serializedObject.FindProperty("beats");
        beatsForWin = serializedObject.FindProperty("beatsForWin");
        song = serializedObject.FindProperty("song");
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        var s = target as QTEObject;

        s.bpm = EditorGUILayout.IntSlider("BPM", s.bpm, 0, 120);
        s.beatsForWin = EditorGUILayout.IntSlider("Beats For Win", s.beatsForWin, 1, 30);

        EditorGUILayout.PropertyField(song);
        s.song = (AudioClip)song.objectReferenceValue;

        if (s.song != null)
        {
            s.startTime = EditorGUILayout.Slider("Start Time", s.startTime, 0, 360);
            s.endTime = EditorGUILayout.Slider("End Time", s.startTime, 0, 360);

            EditorGUILayout.PropertyField(beats, true);
            serializedObject.ApplyModifiedProperties();
        }

        EditorUtility.SetDirty(s);
    }
}
#endif