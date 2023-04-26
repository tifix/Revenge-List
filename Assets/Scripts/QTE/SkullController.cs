using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullController : MonoBehaviour
{
    float speed = 1;
    bool isAlive = true;
    bool badHit = false;
    void Awake()
    {
        transform.localScale = new Vector3(25,25,1);
    }
    void Update()
    {
        transform.Translate(speed * Time.deltaTime, 0, 0);
    }

    public void SetSpeed(float s) { speed = s; }

    public void Kill() { isAlive = false; }

    public bool GetIsAlive() { return isAlive; }

    public void BadHit() { badHit = true; }

    public bool GetBadHit() { return badHit; }
}
