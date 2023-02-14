using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class KarlBoss : MonoBehaviour
{
    [System.Serializable]
    public enum ProjectileType
    {
        OVERHEAD, STRAIGHT, DOG
    }

    [System.Serializable]
    public struct AttackType
    {
        public ProjectileType myType;
        [Range(0.1f, 10f)]
        public float waitTime;
        public float speed;
        public float timeAlive;
    }

    public int health;
    public int shield;

    int currentPhase = 0;
    int currentAttack = 0;
    float attackTimer = 0;
    bool canAttack = true;

    public GameObject steak;
    public GameObject envelope;
    public GameObject dog;

    public List<AttackType> phase1 = new List<AttackType>();
    public List<AttackType> phase2 = new List<AttackType>();
    public List<AttackType> phase3 = new List<AttackType>();

    void Update()
    {
        if (canAttack)
        {
            attackTimer += Time.deltaTime;
            switch (currentPhase)
            {
                case 0:
                    if(attackTimer >= phase1[currentAttack].waitTime)
                    {
                        Attack(phase1[currentAttack]);
                        
                        attackTimer = 0;
                        currentAttack++;

                        if(currentAttack >= phase1.Count)
                        {
                            //End phase
                            //Dialogue and QTE
                            currentAttack = 0;
                            currentPhase++;
                        }
                    }
                    break;
                case 1:
                    if (attackTimer >= phase2[currentAttack].waitTime)
                    {
                        Attack(phase2[currentAttack]);

                        attackTimer = 0;
                        currentAttack++;

                        if (currentAttack >= phase2.Count)
                        {
                            //End phase
                            //Dialogue and QTE
                            currentAttack = 0;
                            currentPhase++;
                        }
                    }
                    break;
                case 2:
                    if (attackTimer >= phase3[currentAttack].waitTime)
                    {
                        Attack(phase3[currentAttack]);

                        attackTimer = 0;
                        currentAttack++;

                        if (currentAttack >= phase3.Count)
                        {
                            //End phase
                            //Dialogue and QTE
                            currentAttack = 0;
                            currentPhase = 0;
                        }
                    }
                    break;
            }

        }
    }

    void Attack(AttackType a)
    {
        if(a.myType == ProjectileType.OVERHEAD)
        {
            GameObject temp = Instantiate<GameObject>(steak, transform.position, Quaternion.identity);
            temp.GetComponent<BossProjectile>().SetSpeed(a.speed);
            temp.GetComponent<BossProjectile>().SetDistance(a.timeAlive);
        }

        else if(a.myType == ProjectileType.STRAIGHT)
        {
            GameObject temp = Instantiate<GameObject>(envelope, transform.position, Quaternion.identity);
            temp.GetComponent<BossProjectile>().SetSpeed(a.speed);
            temp.GetComponent<BossProjectile>().SetDistance(a.timeAlive);
        }

        else if (a.myType == ProjectileType.DOG)
        {
            GameObject temp = Instantiate<GameObject>(dog, transform.position, Quaternion.identity);
            temp.GetComponent<BossProjectile>().SetSpeed(a.speed);
            temp.GetComponent<BossProjectile>().SetDistance(a.timeAlive);
        }
    }

    public void nextPhase()
    {
        currentPhase++;
    }

    public void ChangeAttack()
    {
        canAttack = !canAttack;
    }
}
