using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
* continously shifts a PLAIN image along a given path, relative to it's up rotation.
* movement.x is up translation, movement.y is right translation, basically
*/

[RequireComponent(typeof(Image))]
public class MoveConstantly : MonoBehaviour
{
    public Vector2 movement;

    void Update()
    {
        Vector3 rel = transform.up * movement.x + transform.right * movement.y;
        GetComponent<Image>().rectTransform.position += rel; 
    }
}

