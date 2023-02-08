using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    int speed = 1;
    int timeAlive = 5;
    float t = 0;

    void Update()
    {
        t += Time.deltaTime;
        if(t>timeAlive)
            Destroy(gameObject);

        transform.Translate(-speed * Time.deltaTime, 0, 0);
    }

    public void SetSpeed(int s) { speed = s; }
    public void SetDistance(int d) { timeAlive= d; }
}
