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
    [SerializeField] protected float health;
    public bool knockBackWhenHit = false;
    public float explosionStrength = 3;
    public bool canTakeDamage = true;

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
        AudioManager.instance.PlaySFX("Hit");
        health -= _value;

        if (knockBackWhenHit) 
        {
            Knockback();
            if(health < 0) StartCoroutine(ShrinkAndVanish(1.5f));
        } 
        else if (health < 1)   DeleteObject();
    }

    protected void Knockback() 
    {
        Vector3 kickOrigin = PlayerMovement.instance.transform.position;
        kickOrigin -= new Vector3(0, 3f, 0);
        Vector3 direction = transform.position - kickOrigin;
        direction = new Vector3(direction.x, direction.y, 0);                                   //removing z component
        GetComponent<Rigidbody>().AddForceAtPosition(direction.normalized* explosionStrength, transform.position, ForceMode.Impulse);
        Debug.DrawLine(kickOrigin, transform.position, Color.red, 1);
    }

    // Destroy the object
    protected virtual void DeleteObject()
    {
        Destroy(this.gameObject);
    }
    protected IEnumerator ShrinkAndVanish(float time) 
    {
        AudioManager.instance.PlaySFX("Weee!");
        Vector3 initScale=transform.localScale;
        float norm = 0;
        while (norm<1)
        {
            transform.localScale = Vector3.Lerp(initScale, Vector3.zero, norm);
            yield return new WaitForEndOfFrame();
            norm += Time.deltaTime / time;
        }
        DeleteObject();
    }

    public float GetHealth() { return health; }
    public void SetHealth(float _health) { health = _health; }
    public float GetMaxHealth() { return maxHealth; }
}
