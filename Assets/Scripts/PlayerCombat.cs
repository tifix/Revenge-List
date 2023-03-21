using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//Removed unused "usings" - AV

[RequireComponent(typeof(Animator))]
public class PlayerCombat : ObjectScript
{
    Controls input;
    public static PlayerCombat instance;

    [Range(0,1)] public float attackIntervalMinimum=0.4f;
    [Range(0f,1)] public float attackToIdleInterval=0.7f;
    public float attackLastTimestamp=0;
    public Transform attackOrg;
    public float attackRange = 2.0f;
    public LayerMask enemyLayers;
    public bool canBeDamaged = true;
    public float invincivilityTime = 1f;

    public bool canAttack = true;
    protected override void Awake()                             //Ensuring single instance of the script
    {
        base.Awake();
        if (instance == null) instance = this;
        else Destroy(this);
    }

    void Start()
    {
        input = new Controls();
        this.isAlive = true;
        this.maxHealth = 100.0f;
        this.health = this.maxHealth;

        input.Ground.Attack.performed += Attack;
        input.Ground.Attack.Enable();

        input.Ground.KillSelf.performed += KillSelf;
        input.Ground.KillSelf.Enable();

        canAttack= true;
    }

    //Now the ApplyDamage function is virtual, so this object gets called directly for the damage check only when necessary - AV

    private void FixedUpdate()
    {

        if (Time.time > attackLastTimestamp + attackToIdleInterval)
        {
            if (PlayerMovement.isMovementLocked) 
            {
                GetComponent<Animator>().SetBool("isAttacking", false);
                PlayerMovement.instance.UnPauseMovement();
                Debug.LogWarning("Attack Chain broken! "+ (Time.time - attackLastTimestamp).ToString()+"s from "+ attackLastTimestamp);
            }
            
        }
    }

    public void DisableAttack()
    {
        input.Ground.Disable();
        canAttack = false;
    }

    public void EnableAttack()
    {
        input.Ground.Enable();
        canAttack = true;
    }

    public void Attack(InputAction.CallbackContext obj)
    {
        //Debug.Log("T diff:" + (Time.time - attackLastTimestamp).ToString());

        if (canAttack)
        {
            
            if (Time.time>attackLastTimestamp + attackIntervalMinimum) //if attack off cooldown
            {
                attackLastTimestamp= Time.time;
                Debug.LogWarning("Timestamp set! " + attackLastTimestamp);
                GetComponent<Animator>().SetBool("isAttacking", true);
                PlayerMovement.instance.PauseMovement();
                //StartCoroutine(AttackAnimationChaining());

                // Detect enemies in range
                Collider[] hitEnemies = Physics.OverlapSphere(attackOrg.position, attackRange, enemyLayers);

                // Damage destructibles hit by collider
                foreach (Collider enemy in hitEnemies)
                {
                    Debug.Log("Enemy hit: " + enemy.name);
                    if(enemy.TryGetComponent<ObjectScript>(out ObjectScript OS)) OS.ApplyDamage(10.0f);     //Fixed error where kicking boss proectiles crashed the game -MC
                }
            }            
            //if attack ON cooldown
            else
            {
                Debug.Log("Attacking on cooldown!");
            }

        }
    }

    /*
    IEnumerator AttackAnimationChaining()
    {
        GetComponent<Animator>().SetBool("isAttacking", true);
        //if(GetComponent<Animator>().GetBool("isAttacking"))
        yield return new WaitForSeconds(1);
        GetComponent<Animator>().SetBool("isAttacking", false);
    }*/

    // For applying healing to the player
    void ApplyHealing(float _value)
    {
        health += _value;
        Debug.Log("Health: %f" + health);
    }

    public override void ApplyDamage(float _value)
    {
        if (!canBeDamaged || GameManager.instance.cheat_GodMode)
            return;

        health -= _value;
        GetComponentInChildren<Animator>().SetTrigger("takeDamage");
        canBeDamaged = false;
        StartCoroutine(Invincible(invincivilityTime));

        if (health <= 0.0f)
        {
            OnDeath();
        }
    }

    public IEnumerator Invincible(float duration)
    {
        yield return new WaitForSeconds(duration);
        canBeDamaged = true;
    }

    private void OnDrawGizmosSelected()
    {
        if(attackOrg == null)
            return;
        Gizmos.DrawWireSphere(attackOrg.position, attackRange);
    }

    void OnDeath()
    {
        //Debug.Log("PLAYER DEAD... [WIP]");

        GameManager.instance.SetLost(true);
    }

    void KillSelf(InputAction.CallbackContext obj)
    {
        this.ApplyDamage(maxHealth);
    }

    private void OnDestroy()    //Disabling attack input when exiting to menu
    {
        input.Ground.Attack.Disable();

    }
}
