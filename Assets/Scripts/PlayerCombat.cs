using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
//Removed unused "usings" - AV

[RequireComponent(typeof(Animator))]
public class PlayerCombat : ObjectScript
{
    Controls input;
    public static PlayerCombat instance;

    [Range(0,1)] public float attackBuffer=0.4f;          //Buffer
    [Range(0,1)] public float attackIntervalMinimum=0.6f; //Can attack again
    [Range(0f,1)] public float attackToIdleInterval=0.7f; //Back to idle to late
    public float attackLastTimestamp=0;
    public Transform attackOrg;
    public float attackRange = 2.0f;
    public LayerMask enemyLayers;
    public bool canBeDamaged = true;
    public float invincivilityTime = 1f;

    public bool hasAttacked = false;
    //Check if player wants to continue the combo
    bool comboAttackBuffer = false;

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

        input.Ground.Attack.started += Attack;
        input.Ground.Attack.Enable();

        hasAttacked = false;
    }

    //Now the ApplyDamage function is virtual, so this object gets called directly for the damage check only when necessary - AV

    private void FixedUpdate()
    {
        //Player wants to attack
        if(comboAttackBuffer && Time.time > attackLastTimestamp + attackIntervalMinimum)
        {
            comboAttackBuffer = false;
            Attack();
        }
        
        else if (Time.time > attackLastTimestamp + attackToIdleInterval)
        {
            if (PlayerMovement.isMovementLocked) 
            {
                GetComponent<Animator>().SetBool("isAttacking", false);
                PlayerMovement.instance.UnPauseMovement();
            }
            
        }
        if (GameManager.instance.cheat_LowHPPlayer) { health = 1; GameManager.instance.cheat_LowHPPlayer = false; } //added a way to test player death state stuffs -M
    }

    public void DisableAttack()
    {
        input.Ground.Disable();
        hasAttacked = false;
    }

    public void EnableAttack()
    {
        input.Ground.Enable();
        hasAttacked = true;
    }

    //Overloaded
    public void Attack()
    { 
        //If attack is pressed before the end of the anim
        if (hasAttacked && Time.time > attackLastTimestamp + attackBuffer)
        {
            comboAttackBuffer = true;
            hasAttacked = false;
            return;
        }

        else if (Time.time > attackLastTimestamp + attackIntervalMinimum)
        {
            hasAttacked = true;
            attackLastTimestamp = Time.time;
            GetComponent<Animator>().SetBool("isAttacking", true);
            PlayerMovement.instance.PauseMovement();
        }
        //if attack ON cooldown
        else
        {
            Debug.Log("Attacking on cooldown!");
        }
    }

    public void Attack(InputAction.CallbackContext obj)
    {
        //If attack is pressed before the end of the anim
        if (hasAttacked && Time.time > attackLastTimestamp + attackBuffer)
        {
            comboAttackBuffer = true;
            hasAttacked = false;
            return;
        }

        else if (Time.time > attackLastTimestamp + attackIntervalMinimum)
        {
            hasAttacked = true;
            attackLastTimestamp = Time.time;
            GetComponent<Animator>().SetBool("isAttacking", true);
            PlayerMovement.instance.PauseMovement();
        }
        else
        {
            Debug.Log("Attacking on cooldown!");
        }
    }

    // For applying healing to the player
    void ApplyHealing(float _value)
    {
        health += _value;
        Debug.Log("Health: %f" + health);
    }

    public void DoDamage()
    {
        // Detect enemies in range
        Collider[] hitEnemies = Physics.OverlapSphere(attackOrg.position, attackRange, enemyLayers);

        // Damage destructibles hit by collider
        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log("Enemy hit: " + enemy.name);
            if (enemy.TryGetComponent<ObjectScript>(out ObjectScript OS)) OS.ApplyDamage(10.0f);     //Fixed error where kicking boss proectiles crashed the game -MC
        }
    }

    public override void ApplyDamage(float _value)
    {
        if (!canBeDamaged || GameManager.instance.cheat_GodMode)
            return;

        health -= _value;
        GetComponentInChildren<Animator>().SetTrigger("takeDamage");
        canBeDamaged = false;
        StartCoroutine(Invincible(invincivilityTime));

        if (health <= 0.0f && health!=-999)
        {
            health = 999;
            StartCoroutine(OnDeath());
        }
    }

    public IEnumerator Invincible(float duration)
    {
        canBeDamaged = false;
        yield return new WaitForSeconds(duration);
        canBeDamaged = true;
    }

    private void OnDrawGizmosSelected()
    {
        if(attackOrg == null)
            return;
        Gizmos.DrawWireSphere(attackOrg.position, attackRange);
    }

    IEnumerator OnDeath()
    {
        //Debug.Log("PLAYER DEAD... [WIP]");
        UI.instance.FadeOut();
        yield return new WaitForSeconds(1f);
        GameManager.instance.SetLost(true);
        UI.instance.EnableLostScreen();
        UI.instance.FadeIn();
    }

    private void OnDestroy()    //Disabling attack input when exiting to menu
    {
        input.Ground.Attack.Disable();
    }
}
