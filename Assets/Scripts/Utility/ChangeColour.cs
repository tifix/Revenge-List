using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColour : MonoBehaviour
{
    // Update is called once per frame
    static Color GenerateRandomColour()
    {
        float hue= Random.Range(0f,1f);
        Color c = new Color();
        c= Color.HSVToRGB(hue, 1, 1);
        return c;
    }

    public void SetColurRandom() 
    {
    if(TryGetComponent<SpriteRenderer>(out SpriteRenderer SR))SR.color = GenerateRandomColour();
    if(TryGetComponent<MeshRenderer>(out MeshRenderer MR)) MR.material.color = GenerateRandomColour();
    }
}
