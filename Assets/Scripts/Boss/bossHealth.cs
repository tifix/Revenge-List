using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Handles the boss health, invulnerability and such lovelies.
 */

public class bossHealth : ObjectScript
{
    public bool isCoreExposed= false;
    [Tooltip("The health damaged by QTEs")] public float coreHealth = 100;
    [Range(0, 100f)] public float damageQTEcomplete=40;
    [Range(0,2f)]public float damageQTEcomboMultiplier=2f;
    public GameObject QTEtriggerPrompt;

    protected override void OnEnable()
    {
        base.OnEnable();
        UI.instance.bossHealth = this;
        UI.instance.InitialiseHealthBoss(this);
    }

    public override void ApplyDamage(float _value)
    {
        if(canTakeDamage)
        {
            health -= _value;
            AudioManager.instance.PlaySFX("KarlHit");
            if (health <= 0.0f)OnShieldDepleted();         
        }
    }

    public void OnShieldDepleted() 
    {
        if (isCoreExposed == false)
        {
            isCoreExposed = true;
            Debug.Log("shield down, QTE time");
            GetComponent<BossClass>().ShowDialogueRoundEnd();       //Display the dialogue before the QTE
            QTEtriggerPrompt.SetActive(true);
        }
    }
}
