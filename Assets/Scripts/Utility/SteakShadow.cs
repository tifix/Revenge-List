using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteakShadow : MonoBehaviour
{
    SpriteRenderer sprite;
    public Transform pT;
    Color c;
    float d;
    float ogD;
    Vector3 ogS;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();

        c = new Color(0, 0, 0, 0);
        sprite.color = c;

        ogS = transform.localScale;
        transform.localScale /= 8;

        ogD = Vector3.Distance(transform.position, pT.position);
    }

    void Update()
    {
        d = Vector3.Distance(transform.position, pT.position);
        c.a += (1 - (d / ogD)) * Time.deltaTime;
        sprite.color = c;

        transform.localScale = Vector3.Lerp(transform.localScale, ogS, c.a);
    }
}
