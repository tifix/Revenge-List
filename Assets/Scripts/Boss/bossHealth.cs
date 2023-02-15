using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossHealth : ObjectScript
{
    public bool isCoreExposed= false;
    [Tooltip("The health damaged by QTEs")] public float coreHealth = 100;
    [Range(0, 100f)] public float damageQTEcomplete=40;
    [Range(0,2f)]public float damageQTEcomboMultiplier=2f;
    [SerializeField] GameObject QTEtriggerPrompt;

    protected override void OnEnable()
    {
        base.OnEnable();
        UI.instance.bossHealth = this;
        UI.instance.BossInitialiseHealthBar(this);
    }

    protected override void Update() 
    {
        if (health <= 0.0f) { Debug.Log("shield down, QTE time"); OnShieldDepleted(); }
    }

    public void OnShieldDepleted() 
    {
        isCoreExposed = true;
        QTEtriggerPrompt.SetActive(true);
    }
}
