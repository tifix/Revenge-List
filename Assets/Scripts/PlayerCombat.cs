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

    //I'd recommend most stamina stuff to be int - AV
    const float maxStamina = 100.0f;                                                                // The maximum stamina of the Player
    [SerializeField, Tooltip("Stamina Regen Per Second")]float staminaRegenPerSec = 2.0f;           // Stamina regen rate *MAKE CONST WHEN FINALISED*
    [SerializeField, Tooltip("current stamina of the Player")]float stamina;                        // The current stamina of the Player

    public Transform attackOrg;
    public float attackRange = 2.0f;
    public LayerMask enemyLayers;
    public bool canBeDamaged = true;
    public float invincivilityTime = 1f;

    bool canAttack = true;
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
        stamina = maxStamina;

        input.Ground.Attack.performed += Attack;
        input.Ground.Attack.Enable();

        input.Ground.KillSelf.performed += KillSelf;
        input.Ground.KillSelf.Enable();
    }

    //Now the ApplyDamage function is virtual, so this object gets called directly for the damage check only when necessary - AV

    private void FixedUpdate()
    {
        if(stamina < maxStamina)
        {
            stamina += staminaRegenPerSec * Time.fixedDeltaTime;
            //Debug.Log("Stamina: %f" + stamina);
        }
        else if(stamina > maxStamina)
        {
            stamina = maxStamina;
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
        if(stamina >= 10.0f && canAttack)
        {
            stamina -= 5.0f;
            Debug.Log("Attacking now. Stamina: %f" + stamina);
            GetComponent<Animator>().SetTrigger("attack");

            // Detect enemies in range
            Collider[] hitEnemies = Physics.OverlapSphere(attackOrg.position, attackRange, enemyLayers);

            // Damage destructibles hit by collider
            foreach(Collider enemy in hitEnemies)
            {
                Debug.Log("Enemy hit: " + enemy.name);
                enemy.GetComponent<ObjectScript>().ApplyDamage(10.0f);
            }
            comboAttackBuffer = false;
        }
        else if(!canAttack) { comboAttackBuffer = true; };
    }

    // For applying healing to the player
    void ApplyHealing(float _value)
    {
        health += _value;
        Debug.Log("Health: %f" + health);
    }

    public override void ApplyDamage(float _value)
    {
        if (!canBeDamaged)
            return;
        health -= _value;
        canBeDamaged = false;
        StartCoroutine(Invincible(invincivilityTime));


        if (health <= 0.0f)
        {
            OnDeath();
        }
    }

    IEnumerator Invincible(float duration)
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
