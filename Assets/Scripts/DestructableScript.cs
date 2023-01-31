using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableScript : MonoBehaviour
{
    Texture2D texture;
    Rigidbody2D rb;

    bool isAlive;               // Is the Object alive?
    bool isStatic;              // Is the Object static?
    Vector3 position;           // Position Vector x,y,z: z = Depth?
    const float maxHealth = 1;  // The maximum health the Object can have, defaults to 1 for Props.
    float health;               // The current health of the Object 

    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        isAlive = true;
        isStatic = true;
        position = new Vector3(0.0f, 0.0f, 0.0f);
    }
    private void OnEnable()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isStatic)
        {
            // Do not Update position
        }
        else
        {
            // Update position
        }

        // --- Regular updates START --- 

    }

    // For applying damage to the object
    void ApplyDamage(float _value)
    {
        health -= _value;
    }

    // Destroy the object
    void Destroy()
    {
        rb = null;
        if (rb = null)
            Destroy(this.gameObject);
    }
}
