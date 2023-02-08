using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowPos : MonoBehaviour
{
    float yPos = 0;

    void Awake()
    {
        //Create ray pointing down
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10))
        {
            if (hit.collider.gameObject.CompareTag("Ground"))
            {
                yPos = hit.point.y;
            }
        }
    }

    void Update()
    {
            
        //Vector3 x = new Vector3(transform.position.x, 0, transform.position.z);
        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
    }
}
