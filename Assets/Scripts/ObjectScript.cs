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
    //Is this used? seems unnecessary when you have transform.position - AV
    protected Vector3 position;                     // Position Vector x,y,z: z = Depth?
    //I'd recommend health being int - AV
    [SerializeField] protected float maxHealth = 1;            // The maximum health the Object can have, defaults to 1 for Props.
    [SerializeField] protected float health;                                   // The current health of the Object 

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        isAlive = true;
        isStatic = true;
        position = new Vector3(0.0f, 0.0f, 0.0f);
        health = maxHealth;
    }
    protected virtual void OnEnable()
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

        //Moved the health check to the ApplyDamage function, more performant - AV

    }

    // For applying damage to the object
    public virtual void ApplyDamage(float _value)
    {
        health -= _value;

        if (health <= 0.0f)
        {
            DeleteObject();
        }
    }

    // Destroy the object
    protected void DeleteObject()
    {
        Destroy(this.gameObject);
    }

    public float GetHealth() { return health; }
    public void SetHealth(float _health) { health = _health; }
    public float GetMaxHealth() { return maxHealth; }
}
