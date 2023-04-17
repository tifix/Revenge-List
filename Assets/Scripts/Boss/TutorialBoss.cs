using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBoss : BossClass
{
    public GameObject shield;
    public GameObject vulnerableIndicator;

    public GameObject afterFirstAttackDialoguePrompt;
    public GameObject afterSecondAttackDialoguePrompt;
    public GameObject afterThirdAttackDialoguePrompt;
    public GameObject afterFourthAttackDialoguePrompt;
    public GameObject afterFifthAttackDialoguePrompt;
    public GameObject exitToNextScene;

    protected override void OnEnable()
    {
        base.OnEnable();
        canAttack = false;
        //Change back to FirstAttack
        StartCoroutine(FirstAttack());
        GetComponent<bossHealth>().canTakeDamage = false;
        SetQTEandDialogueForRound(0);
        AudioManager.instance.PlayMusic("BossTrack");
    }

    protected override void Update()
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

                endOfPhase:
                if (currentAttack >= phases[currentPhase].attacks.Count || GameManager.instance.cheat_SkipBossPhase)
                {
                    ChangeAttack();
                    //End phase
                    EndPhase();
                    //Wait for shield to break

                    //update Dialogue and QTE
                    //SetQTEandDialogueForRound(currentPhase);
                    //After QTE success, call NextPhase()

                    GameManager.instance.cheat_SkipBossPhase = false;
                }
            }
        }
    }

    public override void EndPhase()
    {
        //End of phase 1
        if (currentPhase == 1)
        {
            StartCoroutine(SecondAttack());
        }

        if (currentPhase == 2)
        {
            StartCoroutine(ThirdAttack());
        }

        //After succesful QTE
        if(currentPhase == 3)
        {
            StartCoroutine(base.Woosh(1f));
            StartCoroutine(FourthAttack());
        }
    }

    public override void NextPhase()
    {
        currentAttack = 0;
        canAttack = false;
        currentPhase = 3;
        EndPhase();
    }

    public override void RepeatPhase()
    {
        currentAttack = 0;
        canAttack = true;
    }

    //Throw 1 envelope and go to second phase
    IEnumerator FirstAttack()
    {
        player.SetLockMovement();
        yield return new WaitForSeconds(1f);
        base.Attack(phases[currentPhase].attacks[0], 0);
        yield return new WaitForSeconds(2f);
        afterFirstAttackDialoguePrompt.SetActive(true);
        yield return new WaitForEndOfFrame();
        player.SetUnLockMovement();
        currentPhase = 1;
        currentAttack = 0;
        ChangeAttack();
    }

    //After envelopes
    IEnumerator SecondAttack()
    {
        yield return new WaitForSeconds(3f);
        afterSecondAttackDialoguePrompt.SetActive(true);
        currentPhase = 2;
        currentAttack = 0;
        attackTimer = 0;
        ChangeAttack();
    }

    //After steaks
    IEnumerator ThirdAttack()
    {
        yield return new WaitForSeconds(3f);
        afterThirdAttackDialoguePrompt.SetActive(true);
        currentPhase = 3;
        currentAttack = 0;
        attackTimer = 0;
        yield return new WaitForSeconds(1f);
        base.Attack(phases[currentPhase].attacks[0], 0);
        yield return new WaitForSeconds(2f);
        //ToDo- check if player took damage, if so repeat the attack
        yield return new WaitForSeconds(2f);
        //After Screen-Wide attack
        afterFourthAttackDialoguePrompt.SetActive(true);
        GetComponent<bossHealth>().canTakeDamage = true;
        shield.SetActive(false);
        vulnerableIndicator.SetActive(true);
    }

    IEnumerator FourthAttack()
    {
        yield return new WaitForSeconds(2f);
        afterFifthAttackDialoguePrompt.SetActive(true);
        player.SetLockMovement();
        GameManager.instance.CallShake(1, 0.5f);
        yield return new WaitForSeconds(1f);
        //Fade to black, anim in UI
        GameManager.instance.CallShake(1, 0.5f);
        UI.instance.FadeOut();
        yield return new WaitForSeconds(1f);
        //SFX broken glass
        GameManager.instance.CallShake(1, 0.5f);
        //AudioManager.instance.Play("GlassSfx");
        //Activate trigger to leave to next scene
        exitToNextScene.SetActive(true);
        //Fade in, anim in UI
        UI.instance.FadeIn();
        GameManager.instance.CamFollowPlayer();
        player.ReleaseBind();
        player.SetUnLockMovement();
        //Remove boss
        gameObject.SetActive(false);
    }
}
