using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    Texture2D texture;
    Controls input;
    InputAction movement;
    InputAction jump;
    InputAction attack;

    Rigidbody2D rb;

    bool isAlive;                           // Is the Player alive?
    Vector3 position;                       // Position Vector x,y,z: z = Depth?
    float maxHealth = 100.0f;               // The maximum health the Player can have
    float health;                           // The current health of the Player                    
    const float maxStamina = 100.0f;        // The maximum stamina of the Player
    float stamina;                          // The current stamina of the Player

    // Constructor
    private void Awake()
    {
        input = new Controls();
        rb = GetComponent<Rigidbody2D>();
        isAlive = true;
        position = new Vector3(0.0f, 0.0f, 0.0f);
        health = maxHealth;

    }

    private void OnEnable()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   

    }

    // For applying damage to the player
    void ApplyDamage(float _value)
    {
        health -= _value;
    }

    // For applying healing to the player
    void ApplyHealing(float _value)
    {
        health += _value;
    }

    // Destroy the player object
    void Destroy()
    {
        rb = null;
        if(rb = null)
        {
            Destroy(this.gameObject);
        }
    }
}
