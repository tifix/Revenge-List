using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerCombat : ObjectScript
{
    Controls input;

    const float maxStamina = 100.0f;                                                                // The maximum stamina of the Player
    [SerializeField, Tooltip("Stamina Regen Per Second")]float staminaRegenPerSec = 2.0f;           // Stamina regen rate *MAKE CONST WHEN FINALISED*
    [SerializeField, Tooltip("current stamina of the Player")]float stamina;                        // The current stamina of the Player

    public Transform attackOrg;
    public float attackRange = 2.0f;
    public LayerMask enemyLayers;

    // Start is called before the first frame update
    void Start()
    {
        input = new Controls();
        this.isAlive = true;
        this.maxHealth = 100.0f;
        this.health = this.maxHealth;
        stamina = maxStamina;

        input.Ground.Attack.performed += Attack;
        input.Ground.Attack.Enable();
    }

    // Update is called once per frame
    void Update()
    {

    }

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

    public void Attack(InputAction.CallbackContext obj)
    {
        if(stamina >= 10.0f)
        {
            stamina -= 5.0f;
            Debug.Log("Attacking now. Stamina: %f" + stamina);

            // Detect enemies in range
            Collider[] hitEnemies = Physics.OverlapSphere(attackOrg.position, attackRange, enemyLayers);

            // Damage destructibles hit by collider
            foreach(Collider enemy in hitEnemies)
            {
                Debug.Log("Enemy hit: " + enemy.name);
            }
        }
    }

    // For applying healing to the player
    void ApplyHealing(float _value)
    {
        health += _value;
    }

    private void OnDrawGizmosSelected()
    {
        if(attackOrg == null)
            return;
        Gizmos.DrawWireSphere(attackOrg.position, attackRange);
    }


}