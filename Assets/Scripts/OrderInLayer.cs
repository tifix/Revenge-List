using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class OrderInLayer : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer sprite;
    // Start is called before the first frame update
    void Awake()
    {
        sprite.sortingOrder = (int)transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        sprite.sortingOrder = (int)transform.position.z;
    }
}
