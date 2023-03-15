using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossClass : MonoBehaviour
{
    [System.Serializable]
    public class Phases
    {
        public List<BossAttacks> attacks = new List<BossAttacks>();
    }

    public GameObject overHeadAttack;
    public GameObject straightAttack;

    public bossHealth health;

    public float playerRadius;

    public int currentPhase = 0;
    protected int currentAttack = 0;
    protected float attackTimer = 0;
    protected bool canAttack = true;

    public List<Phases> phases = new List<Phases>();

    [Space(3)]
    [Tooltip("each phase is to have a different QTE, set them here"), SerializeField] protected List<QTEObject> phase_QTEs;
    // [Tooltip("each phase will have different dialogue, set it here"), SerializeField] List<DSGraphSaveDataSO> phase_Dialogues;

    [Tooltip("the projectiles are spawned this many s after the animation starts playing"), Range(0, 1), SerializeField] protected float overHeadAnimationDelay;
    [Tooltip("the projectiles are spawned this many s after the animation starts playing"), Range(0, 1), SerializeField] protected float straightAnimationDelay;

    protected PlayerMovement player;
    protected Vector3 knockbackPosition = Vector3.zero; //to avoid player spam-attacking when behind the firewall, knock the player behind the firewall;
    [SerializeField]
    protected Transform camCenter;
    public Animator anim;

    protected virtual void OnEnable()
    {
        player = FindObjectOfType<PlayerMovement>();
        knockbackPosition = new Vector3(camCenter.position.x, player.transform.position.y, camCenter.position.z);
        if (anim == null) anim = GetComponentInChildren<Animator>();
    }

    protected virtual void Update()
    {
        if (canAttack)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= phases[currentPhase].attacks[currentAttack].waitTime)
            {
                if (GameManager.instance.cheat_SkipBossPhase) { goto endOfPhase; }

                //GameManager.instance.CallShake(5, 0.5f);
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
                            //Attack(phases[currentPhase].attacks[currentAttack]);
                            Attack(phases[currentPhase].attacks[currentAttack]);
                            extraTimer = 0;
                            count--;
                        }
                    }
                }

                //Single attack
                else if (phases[currentPhase].attacks[currentAttack].style == BossAttacks.ProjectileStyle.SINGLE)
                {
                    //Attack(phases[currentPhase].attacks[currentAttack]);
                    Attack(phases[currentPhase].attacks[currentAttack]);
                }

                //Attacked
                attackTimer = 0;
                currentAttack++;

                endOfPhase:
                if (currentAttack >= phases[currentPhase].attacks.Count || GameManager.instance.cheat_SkipBossPhase)
                {

                    //End phase
                    EndPhase();
                    //Wait for shield to break

                    //update Dialogue and QTE
                    SetQTEandDialogueForRound(currentPhase);
                    //After QTE success, call NextPhase()

                    GameManager.instance.cheat_SkipBossPhase = false;
                }
            }
        }
    }

    protected virtual void Attack(BossAttacks a)
    {
        if (a.type == BossAttacks.ProjectileType.OVERHEAD)
        {
            float xOffset = Random.Range(-playerRadius, playerRadius);
            float zOffset = Random.Range(-playerRadius, playerRadius);

            GameObject temp = Instantiate<GameObject>(overHeadAttack, player.transform.position + new Vector3(xOffset, 10, zOffset), Quaternion.identity);
            temp.GetComponent<BossProjectile>().SetSpeed(a.speed + 1);
            temp.GetComponent<BossProjectile>().SetDistance(a.timeAlive * 2);
        }

        else if (a.type == BossAttacks.ProjectileType.STRAIGHT)
        {
            float zOffset = Random.Range(-10, 10);

            GameObject temp = Instantiate<GameObject>(straightAttack, transform.position/* + new Vector3(0, 2, 0)*/, Quaternion.identity);    //previous spawn position spawned them underground and insta-despawned
            temp.GetComponent<BossProjectile>().SetSpeed(a.speed + 5);
            temp.GetComponent<BossProjectile>().SetDistance(a.timeAlive);
            Vector3 dir = player.transform.position - temp.transform.position;
            dir.y = 0;
            dir.z += zOffset;
            dir.x += zOffset;
            temp.GetComponent<BossProjectile>().SetDirection(dir.normalized);
        }
    }

    protected virtual void Attack(BossAttacks a, float offset)
    {
        if (a.type == BossAttacks.ProjectileType.OVERHEAD)
        {
            float xOffset = offset;
            float zOffset = offset;

            GameObject temp = Instantiate<GameObject>(overHeadAttack, player.transform.position + new Vector3(xOffset, 10, zOffset), Quaternion.identity);
            temp.GetComponent<BossProjectile>().SetSpeed(a.speed + 1);
            temp.GetComponent<BossProjectile>().SetDistance(a.timeAlive * 2);
        }

        else if (a.type == BossAttacks.ProjectileType.STRAIGHT)
        {
            float zOffset = offset;

            GameObject temp = Instantiate<GameObject>(straightAttack, transform.position/* + new Vector3(0, 2, 0)*/, Quaternion.identity);    //previous spawn position spawned them underground and insta-despawned
            temp.GetComponent<BossProjectile>().SetSpeed(a.speed + 5);
            temp.GetComponent<BossProjectile>().SetDistance(a.timeAlive);
            Vector3 dir = player.transform.position - temp.transform.position;
            dir.y = 0;
            dir.z += zOffset;
            dir.x += zOffset;
            temp.GetComponent<BossProjectile>().SetDirection(dir.normalized);
        }
    }

    public virtual void EndPhase()
    {
        ChangeAttack();
    }

    public virtual void NextPhase()
    {
        StartCoroutine(Woosh(1f)); //Knocks the player back into their position when the phase attacks ended right after going out of QTE 
        currentAttack = 0;
        currentPhase++;
    }

    public virtual void RepeatPhase()
    {
        StartCoroutine(Woosh(1f)); //Knocks the player back into their position when the phase attacks ended right after going out of QTE 
        currentAttack = 0;
    }

    public virtual void SetQTEandDialogueForRound(int round)
    {
        //health.QTEtriggerPrompt.GetComponent<Inter_TextTrigger>().preUseDialogue = phase_Dialogues[round];

        //QTE needs to be initialie beforehand i.e. instance doesnt exist
        QTEManager.instance.SetBeatMap(phase_QTEs[round]);
    }

    public virtual void ChangeAttack()
    {
        canAttack = !canAttack;
    }

    public virtual IEnumerator Woosh(float duration)
    {
        Vector3 init = player.transform.position;
        float progress = 0;
        while (progress < 1)
        {
            player.SetLockMovement();
            player.GetComponent<Collider>().enabled = false;

            yield return new WaitForFixedUpdate();

            player.transform.position = Vector3.Lerp(init, knockbackPosition, progress);
            progress += Time.fixedDeltaTime / duration;
        }
        player.SetUnLockMovement();
        player.GetComponent<Collider>().enabled = true;
    }
}