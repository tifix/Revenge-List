using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;



[CreateAssetMenu(fileName = "bossAttack", menuName = "BossAttack")]
public class BossAttacks : ScriptableObject
{
    public enum AttackType {Melee, Projectile}

    public AttackType type;

    public int a, b;

    public int damage = 1;

    //Melee


    //Projectile
    public int speed = 1;
    public int distance = 1;
    public float fireRate = 1;

    public void MeleeAttack()
    {

    }

    public void ProjectileAttack(GameObject projectile, Vector3 pos)
    {
        GameObject temp = Instantiate<GameObject>(projectile, pos, Quaternion.identity);
        temp.GetComponent<BossProjectile>().SetSpeed(speed);
        temp.GetComponent<BossProjectile>().SetDistance(distance);
    }

    public void ProjectileAttack(GameObject projectile, Vector3 pos, Quaternion rot)
    {
        GameObject temp = Instantiate<GameObject>(projectile, pos, rot);
        temp.GetComponent<BossProjectile>().SetSpeed(speed);
        temp.GetComponent<BossProjectile>().SetDistance(distance);
    }
}

[CustomEditor(typeof(BossAttacks))]
public class Attackeditor : Editor
{
    public SerializedProperty type ,a, b;

    private void OnEnable()
    {
        type = serializedObject.FindProperty("type");
        a = serializedObject.FindProperty("a");
        b = serializedObject.FindProperty("b");
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        var s = target as BossAttacks;

        EditorGUILayout.PropertyField(type);


        s.type = (BossAttacks.AttackType)type.enumValueIndex;
        s.damage = EditorGUILayout.IntSlider("Damage", s.damage, 1, 10);

        switch(s.type)
        {
            case BossAttacks.AttackType.Melee:

                break;
            case BossAttacks.AttackType.Projectile:
                s.speed = EditorGUILayout.IntSlider("Speed", s.speed, 1, 10);
                s.distance = EditorGUILayout.IntSlider("Distance", s.distance, 1, 100);
                s.fireRate = EditorGUILayout.FloatField("Fire rate", s.fireRate);
                break;
        }

    }
}
