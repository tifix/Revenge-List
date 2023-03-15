using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUpDownOverTime : MonoBehaviour
{
    [Range(0.1f,2)]
    public float amplitude = 0.1f;
    [Range(0, 10)]
    public float speed = 6;
    float y;

    private void OnEnable()
    {
        y = transform.position.y;
    }

    void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, (amplitude * Mathf.Sin(speed * Time.time) + y), transform.position.z);
    }
}

