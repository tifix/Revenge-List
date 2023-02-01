using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class QTEManager : MonoBehaviour
{
    public QTEObject currentMap;

    public bool playQTE;
    float beatTimer;
    int beatCounter;

    public GameObject skullPrefab;
    public Transform up, down, left, right;

    void Awake()
    {
        //currentMap = new QTEObject();
        playQTE = false;
        beatCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(playQTE)
        {
            beatTimer += Time.deltaTime;
            if(beatTimer >= currentMap.delay ) 
            {
                SpawnSkull();
                beatTimer = 0;
                beatCounter++;
            }

            if (beatCounter >= currentMap.beats.Count)
                playQTE = false;
        }
    }

    void SpawnSkull()
    {
        Beats currentBeat = currentMap.beats[beatCounter];
        GameObject temp;
        switch(currentBeat) 
        {
            case Beats.Up: temp = Instantiate<GameObject>(skullPrefab, up.position, up.rotation, transform);
                break;
            case Beats.Down: temp = Instantiate<GameObject>(skullPrefab, down.position, down.rotation, transform);
                break;
            case Beats.Left: temp = Instantiate<GameObject>(skullPrefab, left.position, left.rotation, transform);
                break;
            case Beats.Right: temp = Instantiate<GameObject>(skullPrefab, down.position, down.rotation, transform);
                break;
            default: return;
        }
        temp.GetComponent<SkullController>().SetSpeed(currentMap.speed);
    }

    public void QTEStart()
    {
        playQTE = true;
    }

    public void SetBeatMap(QTEObject map)
    {
        currentMap = map;
    }
}
