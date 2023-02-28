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
    public Color colourOverride = Color.white;
    [Tooltip("Size decrement overime")]
    public Vector3 scaleOverTime = Vector3.one;
    [Tooltip("Colour drop overime")]
    public Color colourOverTime = Color.white;
    public float fadeSpeed = 1.0f;

    void Update()
    {
        if(clones.Count > 0)
        {
            for (int i = 0; i < clones.Count; i++)
            {
                clones[i].color -= colourOverTime * fadeSpeed * Time.deltaTime;
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
            obj.transform.position = parentPos.position + new Vector3(0, sr.sprite.vertices[3].y / 2, 0);
            obj.transform.localScale = parentPos.localScale;

            SpriteRenderer tempSR = obj.AddComponent<SpriteRenderer>();
            tempSR.sprite = Sprite.Create(sr.sprite.texture, sr.sprite.textureRect, new Vector2(0.5f, 0), sr.sprite.pixelsPerUnit);
            tempSR.color = colourOverride;
            tempSR.flipX = sr.flipX;
            tempSR.flipY = sr.flipY;
            tempSR.sortingOrder = sr.sortingOrder - 1;

            clones.Add(tempSR);
            yield return new WaitForSeconds(duration / clonesPerSec);
            t -= Time.deltaTime;
        }
        Debug.Log("Stop");
        useTrail = false;
    }
}
