using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesScript : ObjectScript
{
    public Transform attackOrg;
    public float attackRange = 2.0f;
    public LayerMask playerLayer;

    override protected void Update()
    {
        base.Update();
    }

    protected void FixedUpdate()
    {
        closeAttack();
    }

    void closeAttack()
    {
        Debug.Log(this.name+ " attacking now");
        Collider[] hitThings = Physics.OverlapSphere(attackOrg.position, attackRange, playerLayer);

        // Damage player if hit by collider
        foreach (Collider player in hitThings)
        {
            Debug.Log("Enemy hit: " + player.name);
            player.GetComponent<ObjectScript>().ApplyDamage(1.0f);
        }
    }
}
