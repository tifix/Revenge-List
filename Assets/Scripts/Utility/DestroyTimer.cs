using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    [SerializeField]
    float time_to_die;
    float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if(timer>= time_to_die)
        {
            Destroy(gameObject);
        }
    }
}
