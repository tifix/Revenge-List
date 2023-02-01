using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class QTEMovement : MonoBehaviour
{
    Controls input;
    //0 up, 90 left, 180 down, 270 right
    bool up, down, left, right;

    [Range(0f, 1f)]
    public float rotationSpeed = 0.1f;
    // Start is called before the first frame update
    void Awake()
    {
        input = new Controls();

        input.QTE.Up.performed += Up;
        input.QTE.Up.Enable();

        input.QTE.Down.performed += Down;
        input.QTE.Down.Enable();

        input.QTE.Left.performed += Left;
        input.QTE.Left.Enable();

        input.QTE.Right.performed += Right;
        input.QTE.Right.Enable();

        up = true; down = false; left = false; right = false;
    }

    void Up(InputAction.CallbackContext obj)
    {
        if(!up)
        {
            up = true; down = false; left = false; right = false;
            StartCoroutine(SmoothRotate(0, rotationSpeed));
        }
    }

    void Down(InputAction.CallbackContext obj)
    {
        if (!down)
        {
            up = false; down = true; left = false; right = false;
            StartCoroutine(SmoothRotate(2, rotationSpeed));
        }
    }

    void Left(InputAction.CallbackContext obj)
    {
        if (!left)
        {
            up = false; down = false; left = true; right = false;
            StartCoroutine(SmoothRotate(1, rotationSpeed));
        }
    }

    void Right(InputAction.CallbackContext obj)
    {
        if (!right)
        {
            up = false; down = false; left = false; right = true;
            StartCoroutine(SmoothRotate(3, rotationSpeed));
        }
    }

    IEnumerator SmoothRotate(int pos,float time)
    {
        Vector3 finalRot = Vector3.zero;
        switch(pos)
        {
            case 0:finalRot.z = 0;
                break;
            case 1:finalRot.z = 90;
                break;
            case 2:finalRot.z = 180;
                break;
            case 3:finalRot.z = 270;
                break;
        }

        Quaternion currentAngle = transform.rotation;
        Quaternion finalAngle = Quaternion.Euler(finalRot);

        for (float i = 0; i <= 1; i+= Time.deltaTime/time)
        {
            transform.rotation = Quaternion.Slerp(currentAngle, finalAngle, i);
            yield return null;
        }
    }
}
