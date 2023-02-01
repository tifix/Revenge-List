using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesScript : ObjectScript
{
    override protected void Update()
    {
        base.Update();
        closeAttack();
    }
    void closeAttack()
    {
        Debug.Log(this.name+ " attacking now");
    }
}
