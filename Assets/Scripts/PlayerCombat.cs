using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : ObjectScript
{
    Controls input;
           
    const float maxStamina = 100.0f;            // The maximum stamina of the Player
    float stamina;                              // The current stamina of the Player


    // Start is called before the first frame update
    void Start()
    {
        input = new Controls();
        this.isAlive = true;
        this.maxHealth = 100.0f;
        this.health = this.maxHealth;
        stamina = maxStamina;

        input.Ground.Attack.performed += Attack;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void Attack(InputAction.CallbackContext obj)
    {
        Debug.Log("Attacking now");
    }

    // For applying healing to the player
    void ApplyHealing(float _value)
    {
        health += _value;
    }
}
