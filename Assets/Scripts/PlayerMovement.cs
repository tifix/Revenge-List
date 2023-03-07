using System.Collections;
using System.Collections.Generic;
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

    [Header("Jump")]
    [Range(1f, 100f), Tooltip("Jump height")]
    public float jumpStrenght = 4f;
    [Range(0f,1f), Tooltip("Ground check ray lenght")]
    public float groundCheckDistance = 0.5f;

    [Header("Gravity")]
    [Range(1f, 10f), Tooltip("Gravity for the player")]
    public float gravityScale = 1f;
    float gravityForce = 0;
    public bool IsGrounded { get; private set; }

    [Header("Screen Limits")]
    [Tooltip("Max depth and minimun depth")]
    public Vector2 zLimits;

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

        //Bind jump to a function
        input.Ground.Jump.performed += DoJump;
        input.Ground.Jump.Enable();

        input.Ground.Dash.performed += DoDash;
        input.Ground.Dash.Enable();
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
            CheckGround();
            ProcessInput();
            GetComponent<Animator>().SetFloat("walkDirection", dir.x);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    void CheckGround()
    {
        //Find bottom of box collision in World Space
        Vector3 bottom = transform.TransformPoint(boxCol.center);
        bottom.y -= boxCol.size.y / 2;
        
        //Create ray pointing down
        Ray ray = new Ray(bottom + transform.up * .01f, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, groundCheckDistance))
        {
            if (hit.distance < groundCheckDistance)
            {
                IsGrounded = true;
                gravityForce = 0;
                return;
            }
        }
        IsGrounded = false;
    }

    void ProcessInput()
    {     
        if (IsGrounded) 
        {
            //Movement
            rb.velocity= Vector3.zero;
            rb.velocity += new Vector3(dir.x, 0, dir.y * verticalSpeedBoost) * moveSpeed;
        }     
        else
        {
            //Stop X-Z movement
            rb.velocity = new Vector3(0, rb.velocity.y - gravityForce, 0);
            gravityForce += gravityScale * Time.deltaTime;
        }

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

    void DoJump(InputAction.CallbackContext obj)
    {
        if(IsGrounded && !isMovementLocked)
        {
            Debug.Log("Jump");
            //Jump up
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up* jumpStrenght,ForceMode.Impulse);
        }
    }

    void DoDash(InputAction.CallbackContext obj)
    {
        if (IsGrounded && !isMovementLocked && dashTime + dashCoolDown < Time.time)
        {
            Debug.Log("Dash");
            PlayerCombat.instance.canBeDamaged = false;
            dashTime = Time.time;
            GetComponentInChildren<SpriteTrail>().CallTrail(dashLenght);
            //Dash
            StartCoroutine("Dash");
        }
    }

    IEnumerator Dash()
    {
        float t = dashLenght;
        Vector3 dashDir = new Vector3((dir.y != 0 ? dir.x : (sprite.flipX ? -1 : 1)) * dashStrenght, 0, dir.y * dashStrenght * verticalSpeedBoost);
        while (t > 0) 
        {
            //Controller can do shorter dashes, diagonal dashes are a bit longer
            rb.velocity = rb.velocity + dashDir;
            t -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
            PlayerCombat.instance.canBeDamaged = true;
            GetComponentInChildren<SpriteTrail>().StopTrail();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position - new Vector3(0,0.5f,0), transform.position - new Vector3(0, 0.5f + groundCheckDistance, 0));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(transform.position.x, 0, 0), new Vector3(transform.position.x, 0, zLimits.x));
        Gizmos.DrawLine(new Vector3(transform.position.x, 0, 0), new Vector3(transform.position.x, 0, zLimits.y));
    }

    private void OnDestroy()
    {
        movement.Disable();
        input.Ground.Jump.Disable();
    }

    public void SetLockMovement() { isMovementLocked = true; movement.Disable(); input.Ground.Disable(); PlayerCombat.instance.DisableAttack(); }   //Globally accessible movement locks
    public void SetUnLockMovement() { isMovementLocked = false; movement.Enable(); input.Ground.Enable(); PlayerCombat.instance.EnableAttack(); GetComponent<Collider>().enabled = true; }
    public void PauseMovement() { isMovementLocked = true; }
    public void UnPauseMovement() { isMovementLocked = false; }

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
}