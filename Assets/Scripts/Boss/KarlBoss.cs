using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using DS.Data.Save;

[System.Serializable]
public class Phases
{
    public List<BossAttacks> attacks = new List<BossAttacks>();
}

public class KarlBoss : MonoBehaviour
{
    public bossHealth health;

    int currentPhase = 0;
    int currentAttack = 0;
    float attackTimer = 0;
    bool canAttack = true;

    public GameObject steak;
    public GameObject envelope;
    public GameObject gnome;

    public GameObject fireWall;

    public float playerRadius;
    public List<Phases> phases = new List<Phases>();

    [Space(3)]
    [Tooltip("each phase is to have a different QTE, set them here"), SerializeField] List<QTEObject> phase_QTEs;
    [Tooltip("each phase will have different dialogue, set it here"), SerializeField] List<DSGraphSaveDataSO> phase_Dialogues;

    PlayerMovement player;
    private Vector3 knockbackPosition = Vector3.zero; //to avoid player spam-attacking when behind the firewall, knock the player behind the firewall;

    private void OnEnable()
    {
        player = FindObjectOfType<PlayerMovement>();
        StartParticles();
    }

    void Update()
    {
        if (canAttack)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= phases[currentPhase].attacks[currentAttack].waitTime)
            {
                //Burst attack
                if (phases[currentPhase].attacks[currentAttack].style == BossAttacks.ProjectileStyle.BURST)
                {
                    float extraTimer = 0;
                    int count = phases[currentPhase].attacks[currentAttack].amount;
                    while (count > 0)
                    {
                        extraTimer += Time.deltaTime;
                        if (extraTimer >= phases[currentPhase].attacks[currentAttack].delay)
                        {
                            Attack(phases[currentPhase].attacks[currentAttack]);
                            extraTimer = 0;
                            count--;
                        }
                    }
                }

                //Single attack
                else if (phases[currentPhase].attacks[currentAttack].style == BossAttacks.ProjectileStyle.SINGLE)
                {
                    Attack(phases[currentPhase].attacks[currentAttack]);
                }

                //Attacked
                attackTimer = 0;
                currentAttack++;

                if (currentAttack >= phases[currentPhase].attacks.Count)
                {
                    //End phase
                    EndPhase();
                    //Wait for shield to break

                    //update Dialogue and QTE
                    SetQTEandDialogueForRound(currentPhase);
                    //After QTE success, call NextPhase()
                }
            }
        }
    }

    void Attack(BossAttacks a)
    {
        if(a.type == BossAttacks.ProjectileType.OVERHEAD)
        {
            float xOffset = Random.Range(-playerRadius, playerRadius);
            float zOffset = Random.Range(-playerRadius, playerRadius);

            GameObject temp = Instantiate<GameObject>(steak, player.transform.position + new Vector3(xOffset, 10, zOffset), Quaternion.identity);
            temp.GetComponent<BossProjectile>().SetSpeed(a.speed + 1);
            temp.GetComponent<BossProjectile>().SetDistance(a.timeAlive * 2);
        }

        else if(a.type == BossAttacks.ProjectileType.STRAIGHT)
        {
            float zOffset = Random.Range(-1, 1);

            GameObject temp = Instantiate<GameObject>(envelope, transform.position/* + new Vector3(0, 2, 0)*/, Quaternion.identity);    //previous spawn position spawned them underground and insta-despawned
            temp.GetComponent<BossProjectile>().SetSpeed(a.speed + 5);
            temp.GetComponent<BossProjectile>().SetDistance(a.timeAlive);
            Vector3 dir = player.transform.position - temp.transform.position;
            dir.y = 0;
            dir.z += zOffset;
            temp.GetComponent<BossProjectile>().SetDirection(dir.normalized);
        }

        else if (a.type == BossAttacks.ProjectileType.GNOME)
        {
            GameObject temp = Instantiate<GameObject>(steak, player.transform.position, Quaternion.identity);
            temp.GetComponent<BossProjectile>().SetSpeed(a.speed);
            temp.GetComponent<BossProjectile>().SetDistance(a.timeAlive);
        }
    }

    public void EndPhase()
    {
        RecordPosition();   //records player position so we can knock them away from spamming the boss as soon as the QTE ends
        ChangeAttack();
        StopParticles();
    }

    public void NextPhase()
    {
        KnockBackToPosition(2f);    
        currentAttack = 0;
        currentPhase++;
        ChangeAttack();
        StartParticles();
    }
    public void SetQTEandDialogueForRound(int round) 
    {
        //health.QTEtriggerPrompt.GetComponent<Inter_TextTrigger>().preUseDialogue = phase_Dialogues[round];
        //QTE needs to be initialie beforehand i.e. instance doesnt exist
        QTEManager.instance.SetBeatMap(phase_QTEs[round]);
    }

    public void ChangeAttack()
    {
        canAttack = !canAttack;
    }

    public void StopParticles()
    {
        fireWall.SetActive(false);
    }

    public void StartParticles()
    {
        fireWall.SetActive(true);
    }


    #region post QTE knockback
    public void RecordPosition() 
    {
        knockbackPosition = player.transform.position;
    }
    public void KnockBackToPosition(float duration)   //Knocks the player back into their position when the phase attacks ended right after going out of QTE 
    {
        StartCoroutine(Woosh(duration));
    }
    public IEnumerator Woosh(float duration) 
    {
        Vector3 init = player.transform.position;
        float progress = 0;
        while (progress < 1) 
        {
            player.GetComponent<Collider>().enabled = false;
            yield return new WaitForFixedUpdate();
            player.transform.position = Vector3.Lerp(init, knockbackPosition, progress);
            progress += Time.fixedDeltaTime/ duration;
        }
        player.GetComponent<Collider>().enabled = true;
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + new Vector3(0, 2, 0),.25f);
    }
}
