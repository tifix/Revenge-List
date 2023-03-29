using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{

    //Input Action Map Script
    public static PlayerMovement instance;
    Controls input;
    InputAction movement;

    Rigidbody rb;
    BoxCollider boxCol;
    Vector2 dir;

    SpriteRenderer sprite;

    bool bossFightBind { get; set; } = false;
    Transform left, right;

    [Header("Locks"), Tooltip("for example when player is in dialogue or in QTE")]
    public static bool isMovementLocked = false;

    [Header("Move")]
    [Range(1f, 10f), Tooltip("Player movement speed")]
    public float moveSpeed = 5f;
    [Range(1f, 10f), Tooltip("Depth speed offset")]
    public float verticalSpeedBoost = 2f;

    [Header("Dash")]
    [Range(1,100), Tooltip("How fast the player will travel")]
    public float dashStrenght = 1.0f;
    [Range(0.1f,2), Tooltip("How long the dash is")]
    public float dashLenght = 1.0f;
    [Range(0.1f,2), Tooltip("How often the player can dash")]
    public float dashCoolDown = 1.0f;
    float dashTime = 0;

    [Header("Screen Limits")]
    [Tooltip("Max depth and minimun depth")]
    public Vector2 zLimits;

    public bool hasList = true;
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);

        input = new Controls();
        rb = GetComponent<Rigidbody>();
        boxCol = GetComponent<BoxCollider>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        //Bind movement to and action
        movement = input.Ground.Move;
        movement.Enable();

        //Bind dash to a function
        input.Ground.Dash.performed += DoDash;
        input.Ground.Dash.Enable();

        input.Menu.List.performed += RevengeList;
        input.Menu.List.Enable();
    }

    void Update()
    {
        //Movement axis
        if(!isMovementLocked) 
        {
            dir = movement.ReadValue<Vector2>();
            if (dir.x < 0)
                sprite.flipX = true;
            else if (dir.x > 0)
                sprite.flipX = false;
            ProcessInput();
            GetComponent<Animator>().SetFloat("walkDirection", dir.magnitude);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    void ProcessInput()
    {
        //Movement
        rb.velocity = Vector3.zero;
        rb.velocity += new Vector3(dir.x, 0, dir.y * verticalSpeedBoost) * moveSpeed;

        //Depth limits
        if (transform.position.z > zLimits.x || transform.position.z < zLimits.y)
            transform.position = transform.position.z > 0 ? new Vector3(transform.position.x, transform.position.y, zLimits.x) : new Vector3(transform.position.x, transform.position.y, zLimits.y);

        if(bossFightBind)
        {
            //Max to the right
            if (transform.position.x > right.position.x)
                transform.position = new Vector3(right.position.x, transform.position.y, transform.position.z);
            //Max to the left
            else if (transform.position.x < left.position.x)
                transform.position = new Vector3(left.position.x, transform.position.y, transform.position.z);
        }
    }

    void DoDash(InputAction.CallbackContext obj)
    {
        if (!isMovementLocked && dashTime + dashCoolDown < Time.time)
        {
            Debug.Log("Dash");
            StartCoroutine(PlayerCombat.instance.Invincible(dashLenght + 0.5f));
            dashTime = Time.time;
            GetComponentInChildren<SpriteTrail>().CallTrail(dashLenght);
            //Dash
            StartCoroutine("Dash");
        }
    }

    IEnumerator Dash()
    {
        //Dash animation starting
        GetComponent<Animator>().SetBool("isDashing", true);
        try
        {
            AudioManager.instance.PlaySFX("Dash");
        }
        catch(Exception e)
        {
            print("sfx fail");
        }

        float t = dashLenght;
        Vector3 dashDir = new Vector3((dir.y != 0 ? dir.x : (sprite.flipX ? -1 : 1)) * dashStrenght, 0, dir.y * dashStrenght * verticalSpeedBoost);
        while (t > 0) 
        {
            //Controller can do shorter dashes, diagonal dashes are a bit longer
            rb.velocity = rb.velocity + dashDir;
            t -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
            GetComponentInChildren<SpriteTrail>().StopTrail();
        }
        //dash animation ending
        GetComponent<Animator>().SetBool("isDashing", false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(transform.position.x, 0, 0), new Vector3(transform.position.x, 0, zLimits.x));
        Gizmos.DrawLine(new Vector3(transform.position.x, 0, 0), new Vector3(transform.position.x, 0, zLimits.y));
    }

    private void OnDestroy()
    {
        movement.Disable();
    }

    public void SetLockMovement() { isMovementLocked = true; movement.Disable(); input.Ground.Disable(); PlayerCombat.instance.DisableAttack(); }   //Globally accessible movement locks
    public void SetUnLockMovement() { isMovementLocked = false; movement.Enable(); input.Ground.Enable(); PlayerCombat.instance.EnableAttack(); GetComponent<Collider>().enabled = true; }
    public void PauseMovement() { isMovementLocked = true; }//PlayerCombat.instance.DisableAttack();
    public void UnPauseMovement() { isMovementLocked = false; }// PlayerCombat.instance.EnableAttack();

    public void BossFightBinding(Transform t)
    {
        bossFightBind = true;
        left = t.GetChild(0).transform;
        right = t.GetChild(1).transform;
    }

    public void ReleaseBind()
    {
        bossFightBind = false;
        left = null;
        right = null;
    }

    public void Play(String name)
    {
        AudioManager.instance.PlaySFX(name);
    }

    void RevengeList(InputAction.CallbackContext obj)
    {
        if(hasList)
            UI.instance.ToggleRevengeList();
    }

    public void SetList()
    {
        hasList = true;
    }
}