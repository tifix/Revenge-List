using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class ObjectScript : MonoBehaviour
{
    Texture2D texture;
    Rigidbody rb;
    
    protected bool isAlive, isStatic;               // Is the Object alive? isStatic;              // Is the Object static?
    protected Vector3 position;                     // Position Vector x,y,z: z = Depth?
    protected float maxHealth = 1;            // The maximum health the Object can have, defaults to 1 for Props.
    [SerializeField] protected float health;                                   // The current health of the Object 

    // Start is called before the first frame update
    protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
        isAlive = true;
        isStatic = true;
        position = new Vector3(0.0f, 0.0f, 0.0f);
        health = maxHealth;
    }
    protected void OnEnable()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
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

        if(health <= 0.0f)
        {
            DeleteObject();
        }

    }

    // For applying damage to the object
    public void ApplyDamage(float _value)
    {
        health -= _value;
    }

    // Destroy the object
    protected void DeleteObject()
    {
        Destroy(this.gameObject);
    }
}
