using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    Controls input;
    InputAction movement;
    InputAction jump;
    InputAction attack;

    Rigidbody2D rb;

    private void Awake()
    {
        input = new Controls();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
