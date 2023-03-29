using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
* continously shifts a RAW image along a given path.
* 
*/

[RequireComponent(typeof(RawImage))]
public class MoveConstantly : MonoBehaviour
{
    private RawImage image;
    public Vector2 movement;

    public void OnEnable()
    {
        if (image == null) image=GetComponent<RawImage>();
    }

    void Update()
    {
        image.uvRect = new Rect(image.uvRect.position + movement*Time.deltaTime, image.uvRect.size);
    }
}

