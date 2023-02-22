using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOvertime : MonoBehaviour
{
    public Vector3 axis;
    public float speed = 10;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(axis * (speed * Time.deltaTime));
    }
}
