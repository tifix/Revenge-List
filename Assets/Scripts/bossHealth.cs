using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossHealth : ObjectScript
{
    public bool isCoreExposed= false;
    public float coreHealth = 100;
    [SerializeField] GameObject QTEtriggerPrompt;

    private void Start()
    {
        UI.instance.bossHealth = this;
    }

    protected override void Update() 
    {
        if (health <= 0.0f) { Debug.Log("shield down, QTE time"); OnShieldDepleted(); }
    }

    void OnShieldDepleted() 
    {
        isCoreExposed = true;
        QTEtriggerPrompt.SetActive(true);
    }
}
