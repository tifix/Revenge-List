using System.Collections;
using System.Collections.Generic;
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

    [Range(1f, 10f)]
    public float moveSpeed = 5f;
    [Range(1f, 10f)]
    public float verticalSpeedBoost = 2f;
    [Tooltip("Can the player jump")]
    public bool allowJump = true;
    [Range(1f, 10f)]
    public float jumpSpeed = 4f;
    [Range(0f,1f)]
    public float groundCheckDistance = 0.5f;
    [Range(1f, 10f)]
    public float gravityScale = 1f;

    public bool IsGrounded { get; private set; }

    void Awake()
    {
        input = new Controls();
        rb = GetComponent<Rigidbody>();
        boxCol = GetComponent<BoxCollider>();

        //Bind movement to and action
        movement = input.Ground.Move;
        movement.Enable();

        //Bind jump to a function
        input.Ground.Jump.performed += DoJump;
        input.Ground.Jump.Enable();
    }

    void Update()
    {
        dir = movement.ReadValue<Vector2>();
        CheckGround();
        ProcessInput();
    }

    void CheckGround()
    {
        IsGrounded = false;
        Vector3 bottom = transform.TransformPoint(boxCol.center);
        bottom.y -= boxCol.size.y / 2;
        
        Ray ray = new Ray(bottom + transform.up * .01f, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, groundCheckDistance))
        {
            if (hit.distance < groundCheckDistance)
                IsGrounded = true;
        }
    }

    void ProcessInput()
    {     
        if (IsGrounded) 
        { 
            rb.velocity= Vector3.zero;
            rb.velocity += new Vector3(dir.x, 0, dir.y * verticalSpeedBoost) * moveSpeed;
        }      
    }

    void DoJump(InputAction.CallbackContext obj)
    {
        if(IsGrounded && allowJump)
        {
            Debug.Log("Jump");
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.up* jumpSpeed,ForceMode.Impulse);
        }
    }
}
