using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    /*
    [System.Serializable]
    public struct Attacks
    {
        public BossAttacks attack;
        [Range(0.1f, 10f)]
        public float time;
    }

    public List<Attacks> attacks;
    public List<GameObject> projectiles;

    float timer = 0;
    int actionCount = 0; 

    void Update()
    {
        timer += Time.deltaTime;

        if(timer >= attacks[actionCount].time)
        {
            BossAttacks currentAttack = attacks[actionCount].attack;
            switch (currentAttack.type)
            {
                case BossAttacks.AttackType.Melee:
                    currentAttack.MeleeAttack();
                    break;

                case BossAttacks.AttackType.Projectile:
                    int r = Random.Range(0, projectiles.Count);
                    currentAttack.ProjectileAttack(projectiles.Count > 1 ? projectiles[0] : projectiles[r], transform.position);
                    break;
            }

            actionCount++;

            //Restart attacks
            if (actionCount + 1 > attacks.Count)
            {
                actionCount = 0;
                timer = 0;
            }
        }

    }*/
}
