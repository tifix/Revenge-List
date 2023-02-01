using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullController : MonoBehaviour
{
    int speed = 1;
    void Awake()
    {
        transform.localScale = new Vector3(30,30,1);
    }
    void Update()
    {
        transform.Translate(speed * Time.deltaTime, 0, 0);
        if(Vector3.Distance(transform.localPosition,Vector3.zero) <= transform.localScale.x)
        {
            Destroy(gameObject);
        }

    }

    public void SetSpeed(int s) { speed = s; }
}
