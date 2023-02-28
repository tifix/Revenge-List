using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
#if (UNITY_EDITOR)
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "bossAttack", menuName = "BossAttack")]
public class BossAttacks : ScriptableObject
{
    public enum ProjectileStyle
    {
        SINGLE, BURST
    }
   
    public enum ProjectileType
    {
        OVERHEAD, STRAIGHT, GNOME
    }

    public ProjectileStyle style;

    public ProjectileType type;

    public int a, b, c, d, e;

    //GLOBAL
    [Range(0.1f, 4f)]
    public float waitTime = 1;
    [Range(1f, 10f)]
    public float speed = 1;
    [Range(1f, 10f)]
    public float timeAlive = 1;

    //BURST
    [Range(1f, 10f)]
    public int amount = 1;
    [Range(0.1f, 4f)]
    public float delay = 0.1f;
    [Range(0f, 10f)]
    public float speedModifier = 0;
}

#if (UNITY_EDITOR)
[CustomEditor(typeof(BossAttacks))]
public class Attackeditor : Editor
{
    public SerializedProperty type , style, a, b, c, d, e;

    private void OnEnable()
    {
        style = serializedObject.FindProperty("style");
        a = serializedObject.FindProperty("a");
        b = serializedObject.FindProperty("b");
        type = serializedObject.FindProperty("type");
        c = serializedObject.FindProperty("c");
        d = serializedObject.FindProperty("d");
        e = serializedObject.FindProperty("e");
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        var s = target as BossAttacks;

        EditorGUILayout.PropertyField(style);
        s.style = (BossAttacks.ProjectileStyle)style.enumValueIndex;
        
        EditorGUILayout.PropertyField(type);
        s.type = (BossAttacks.ProjectileType)type.enumValueIndex;

        s.waitTime = EditorGUILayout.Slider("Wait Time", s.waitTime, 0.1f, 4);
        s.speed = EditorGUILayout.Slider("Speed", s.speed, 1, 10);
        s.timeAlive = EditorGUILayout.Slider("Distance", s.timeAlive, 1, 10);
        
        if(s.style == BossAttacks.ProjectileStyle.BURST)
        {
            s.amount = EditorGUILayout.IntSlider("Amount", s.amount, 1, 10);
            s.delay = EditorGUILayout.Slider("Delay", s.delay, 0.1f, 4);
            s.speedModifier = EditorGUILayout.Slider("Speed Modifier", s.speedModifier, 0, 10);
        }

        EditorUtility.SetDirty(s);
    }
}
#endif