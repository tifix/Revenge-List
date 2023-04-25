using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//using DS.Data.Save;

public class KarlBoss : BossClass
{
    public GameObject fireWall;

    protected override void OnEnable()
    {
        base.OnEnable();
        if (anim == null) anim = GetComponentInChildren<Animator>();
        StartCoroutine(base.Woosh(1f));
        AudioManager.instance.PlayMusic("BossTrack");
        Invoke("StartParticles", 0.5f); //Tiny delay so animations are in sync
    }

    protected override void Update()
    {
        if (canAttack)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= phases[currentPhase].attacks[currentAttack].waitTime)
            {
                if (GameManager.instance.cheat_SkipBossPhase) { goto endOfPhase; }

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
                            StartCoroutine(DelayedAttack(phases[currentPhase].attacks[currentAttack]));
                            extraTimer = 0;
                            count--;
                        }
                    }
                }

                //Single attack
                else if (phases[currentPhase].attacks[currentAttack].style == BossAttacks.ProjectileStyle.SINGLE)
                {
                    StartCoroutine(DelayedAttack(phases[currentPhase].attacks[currentAttack]));
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

    IEnumerator DelayedAttack(BossAttacks a)
    {
        float animCookTime = 1;
        switch (a.type) 
        {
            case (BossAttacks.ProjectileType.OVERHEAD):
                {
                    //Show the throwing animation for overhead attacks
                    anim.SetTrigger("attackFlipping");
                    animCookTime = overHeadAnimationDelay;
                    //AudioManager.instance.PlaySFX("Flip");
                    break; 
                }
            case (BossAttacks.ProjectileType.STRAIGHT):
                {
                    //Show the throwing animation for overhead attacks
                    anim.SetTrigger("attackThrowing");
                    AudioManager.instance.PlaySFX("Throw");
                    animCookTime = straightAnimationDelay;
                    break;
                }

        }
        yield return new WaitForSeconds(animCookTime);
        Attack(a);
    }

    public override void EndPhase()
    {
        base.EndPhase();
        StopParticles();
        anim.SetBool("Vulnerable", true);
    }

    public override void NextPhase()
    {
        base.NextPhase();
        ChangeAttack();
        anim.SetBool("Vulnerable", false);
    }

    public override void RepeatPhase()
    {
        base.RepeatPhase();
        ChangeAttack();
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
    public override IEnumerator Woosh(float duration) 
    {
        StartCoroutine(base.Woosh(duration));
        StartParticles();
        yield return new Null();
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, 2, 0),.25f);
    }
}
