using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //Input Action Map Script
    Controls input;
    InputAction movement;

    Rigidbody rb;
    BoxCollider boxCol;
    Vector2 dir;

    SpriteRenderer sprite;

    [Header("Dialogue")]
    public bool isDialogue = false;

    [Header("Move")]
    [Range(1f, 10f), Tooltip("Player movement speed")]
    public float moveSpeed = 5f;
    [Range(1f, 10f), Tooltip("Depth speed offset")]
    public float verticalSpeedBoost = 2f;

    [Header("Jump")]
    [Tooltip("Can the player jump")]
    public bool allowJump = true;
    [Range(1f, 100f), Tooltip("Jump height")]
    public float jumpStrenght = 4f;
    [Range(0f,1f), Tooltip("Ground check ray lenght")]
    public float groundCheckDistance = 0.5f;

    [Header("Gravity")]
    [Range(1f, 10f), Tooltip("Gravity for the player")]
    public float gravityScale = 1f;
    float gravityForce = 0;


    [Header("Screen Limits")]
    [Tooltip("Max depth and minimun depth")]
    public Vector2 zLimits;
    public bool IsGrounded { get; private set; }

    void Awake()
    {
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
    }

    void Update()
    {
        //Movement axis
        if(!isDialogue) 
        {
            dir = movement.ReadValue<Vector2>();
            if (dir.x < 0)
                sprite.flipX = true;
            else if (dir.x > 0)
                sprite.flipX = false;
            CheckGround();
            ProcessInput();
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
    }

    void DoJump(InputAction.CallbackContext obj)
    {
        if(IsGrounded && allowJump)
        {
            Debug.Log("Jump");
            //Jump up
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up* jumpStrenght,ForceMode.Impulse);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position - new Vector3(0,0.5f,0), transform.position - new Vector3(0, 0.5f + groundCheckDistance, 0));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(Vector3.zero, new Vector3(0, 0, zLimits.x));
        Gizmos.DrawLine(Vector3.zero, new Vector3(0, 0, zLimits.y));
    }
}
