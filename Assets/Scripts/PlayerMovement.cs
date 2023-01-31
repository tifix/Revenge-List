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
    private void Awake()
    {
        input = new Controls();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        //Bind movement to and action
        movement = input.Ground.Move;
        movement.Enable();

        //Bind jump to a function
        input.Ground.Jump.performed += DoJump;
        input.Ground.Jump.Enable();

        //Bind jump to a function
        input.Ground.Attack.performed += Attack;
        input.Ground.Attack.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(movement.ReadValue<Vector2>());
    }

    void DoJump(InputAction.CallbackContext obj)
    {
        Debug.Log("Jump");
    }

    void Attack(InputAction.CallbackContext obj)
    {
        Debug.Log("Attack");

    }
}
