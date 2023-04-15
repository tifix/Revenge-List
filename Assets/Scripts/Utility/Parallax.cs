using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    float lenght, startPos;
    public GameObject cam;
    public float parallaxStrenght;

    void Start()
    {
        startPos = transform.position.x;
        lenght = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float tmp = cam.transform.position.x * (1 - parallaxStrenght);
        float dist = cam.transform.position.x * parallaxStrenght;

        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

        if (tmp > startPos + lenght) startPos += lenght;
        else if (tmp < startPos - lenght) startPos -= lenght;
    }
}
