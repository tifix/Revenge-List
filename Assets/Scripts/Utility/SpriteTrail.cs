using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteTrail : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer sr;
    [SerializeField]
    Transform parentPos;
    private List<SpriteRenderer> clones = new List<SpriteRenderer>();
    
    public bool useTrail { get; set; }

    public int clonesPerSec = 10;
    [Tooltip("Size decrement overime")]
    public Vector3 scaleOverTime = Vector3.one;
    [Tooltip("Colour drop overime")]
    public Color colourOverTime = Color.white;

    void Update()
    {
        if(clones.Count > 0)
        {
            for (int i = 0; i < clones.Count; i++)
            {
                clones[i].color -= colourOverTime * Time.deltaTime;
                clones[i].transform.localScale -= scaleOverTime * Time.deltaTime;
                if (clones[i].color.a<=0f || clones[i].transform.localScale.x <= 0f)
                {
                    Destroy(clones[i].gameObject);
                    clones.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    public void CallTrail(float duration)
    {
        useTrail = true;
        StartCoroutine(trailRender(duration/2));
    }

    public void StopTrail()
    {
        useTrail = false;
        StopCoroutine(trailRender(0));
    }

    IEnumerator trailRender(float duration)
    {
        float t = duration;
        while(t > 0)
        {
            GameObject obj = new GameObject("CloneTrail");
            obj.transform.position = parentPos.position;
            obj.transform.localScale = parentPos.localScale;

            SpriteRenderer tempSR = obj.AddComponent<SpriteRenderer>();
            tempSR.sprite = sr.sprite;
            tempSR.flipX = sr.flipX;
            tempSR.flipY = sr.flipY;
            tempSR.sortingOrder = sr.sortingOrder - 1;

            clones.Add(tempSR);
            t -= Time.deltaTime;
            yield return new WaitForSeconds(duration / clonesPerSec);
        }
        Debug.Log("Stop");
        useTrail = false;
    }
}
