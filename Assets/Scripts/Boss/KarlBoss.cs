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
        [Range(0.1f, 4f)]
        public float waitTime;
        [Range(1f, 10f)]
        public float speed;
        [Range(1f, 10f)]
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

    public float playerRadius;
    public List<AttackType> phase1 = new List<AttackType>();
    public List<AttackType> phase2 = new List<AttackType>();
    public List<AttackType> phase3 = new List<AttackType>();

    PlayerMovement player;

    private void OnEnable()
    {
        player = FindObjectOfType<PlayerMovement>();    
    }

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
            float xOffset = Random.Range(-playerRadius, playerRadius);
            float zOffset = Random.Range(-playerRadius, playerRadius);

            GameObject temp = Instantiate<GameObject>(steak, player.transform.position + new Vector3(xOffset, 5, zOffset), Quaternion.identity);
            temp.GetComponent<BossProjectile>().SetSpeed(a.speed);
            temp.GetComponent<BossProjectile>().SetDistance(a.timeAlive);
        }

        else if(a.myType == ProjectileType.STRAIGHT)
        {
            float zOffset = Random.Range(-1, 1);

            GameObject temp = Instantiate<GameObject>(envelope, transform.position + new Vector3(0, -1, 0), Quaternion.identity);
            temp.GetComponent<BossProjectile>().SetSpeed(a.speed);
            temp.GetComponent<BossProjectile>().SetDistance(a.timeAlive);
            temp.GetComponent<BossProjectile>().SetDirection(new Vector3(-1, 0, zOffset));
        }

        else if (a.myType == ProjectileType.DOG)
        {
            GameObject temp = Instantiate<GameObject>(steak, player.transform.position, Quaternion.identity);
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
