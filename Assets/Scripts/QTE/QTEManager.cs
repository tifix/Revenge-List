using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QTEManager : MonoBehaviour
{
    public static QTEManager instance;        //cross-script access shorthand
    public QTEObject defaultMap;
    public QTEObject currentMap;

    public bool playQTE;
    public int healthQTE = 3;

    bool isPlaying;
    float beatTimer;
    int beatCounter;
    int comboCount;
    int correctHits;

    public GameObject skullPrefab;
    public GameObject skullVFX;
    public GameObject BeatUI;
    public Image FillSong;
    [Range(0.1f,1.0f)]
    public float beatScale;
    public TMP_Text comboUI;
    public Transform up, down, left, right;

    public List<BeatItem> beatObjects; 
    List<SkullController> _skulls;

    float percentagePerSkull;

    [System.Serializable]
    public struct BeatItem
    {
        public GameObject obj;
        Vector3 ogScale;
        Vector3 doubleScale;
        public BeatItem(GameObject o)
        {
            obj = o;
            ogScale = obj.transform.localScale;
            doubleScale = ogScale * 1.2f;
        }
        public GameObject GetObj() { return obj; }
        public Vector3 GetScale() { return ogScale; }
        public Vector3 GetDoubleScale() { return doubleScale; }
    }

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);

        if (currentMap == null)
            currentMap = defaultMap;

        percentagePerSkull = (float)(1.0f / currentMap.beats.Count);

        playQTE = true;
        isPlaying = true;

        beatCounter = 0;
        correctHits = 0;
        comboUI.text = "";

        _skulls = new List<SkullController>();

        for (int i = 0; i < beatObjects.Count; i++)
        {
            beatObjects[i] = new BeatItem(beatObjects[i].GetObj());        
        }
        beatObjects.Add(new BeatItem(BeatUI));
    }

    void Update()
    {
        if (playQTE)
        {
            beatTimer += Time.deltaTime;
            BeatUI.transform.Rotate(new Vector3(0, 0, -100 * Time.deltaTime));
            if (beatTimer >= currentMap.delay)
            {
                for (int i = 0; i < beatObjects.Count; i++)
                {
                    StartCoroutine(PulseObject(beatObjects[i], beatScale));
                }
                if(isPlaying)
                {
                    SpawnSkull();
                    beatCounter++;
                    FillSong.fillAmount += percentagePerSkull;
                }
                beatTimer = 0;
            }

            //End of map
            if (beatCounter >= currentMap.beats.Count)
            {
                isPlaying = false;
                beatTimer = 0;
                beatCounter = 0;

                Debug.Log("skull track finished");
                Invoke("QTEEnd",3);
            }

            if (_skulls.Count > 0)
            {
                for (int i = 0; i < _skulls.Count; i++)
                {
                    //Hit the player
                    if (Vector3.Distance(_skulls[i].transform.localPosition, Vector3.zero) <= _skulls[i].transform.localScale.x)
                    {
                        //Debug.Log("Player");
                        Instantiate<GameObject>(skullVFX, _skulls[i].transform.position, Quaternion.identity);
                        Destroy(_skulls[i].gameObject);
                        _skulls.RemoveAt(i);
                        comboCount = 0;
                        comboUI.text = "";

                        healthQTE--;
                        //Fail QTE
                        if(healthQTE<=0)
                        {
                            //End QTE
                            correctHits = 0;
                            QTEEnd();
                            QTECleanUp();
                        }
                        break;
                    }

                    //Hit the sword
                    if (!_skulls[i].GetIsAlive())
                    {
                        //Debug.Log("Sword");
                        Instantiate<GameObject>(skullVFX, _skulls[i].transform.position, Quaternion.identity);
                        Destroy(_skulls[i].gameObject);
                        _skulls.RemoveAt(i);
                        comboCount += 1;
                        correctHits++;
                        comboUI.text = "Combo " + comboCount.ToString();
                        break;
                    }

                    //Hit the sword while rotating
                    else if (_skulls[i].GetBadHit())
                    {
                        Instantiate<GameObject>(skullVFX, _skulls[i].transform.position, Quaternion.identity);
                        Destroy(_skulls[i].gameObject);
                        _skulls.RemoveAt(i);
                    }
                }
                
            }
            else if (!isPlaying)
                playQTE = false;
        }
    }

    void SpawnSkull()
    {
        Beats currentBeat = currentMap.beats[beatCounter].myBeat;
        switch(currentBeat) 
        {
            case Beats.Up: _skulls.Add(Instantiate<GameObject>(skullPrefab, up.position, up.rotation, transform).GetComponent<SkullController>());
                break;
            case Beats.Down: _skulls.Add(Instantiate<GameObject>(skullPrefab, down.position, down.rotation, transform).GetComponent<SkullController>());
                break;
            case Beats.Left: _skulls.Add(Instantiate<GameObject>(skullPrefab, left.position, left.rotation, transform).GetComponent<SkullController>());
                break;
            case Beats.Right: _skulls.Add(Instantiate<GameObject>(skullPrefab, right.position, right.rotation, transform).GetComponent<SkullController>());
                break;
            default: return;
        }       
        _skulls.Last<SkullController>().SetSpeed(currentMap.speed * currentMap.beats[beatCounter].speedMod);
    }

    public void QTEStart()
    {
        //UI.instance.ToggleQTEScreen();
        playQTE = true;
        isPlaying = true;

        GameManager.instance.SetPause(false);

        if (currentMap == null)
            currentMap = defaultMap;

        percentagePerSkull = (float)(1.0f / currentMap.beats.Count);
        FillSong.fillAmount = 0;
    }
    public int QTEEnd()
    {
        if (UI.instance.bossHealth != null)
        {
            UI.instance.bossHealth.coreHealth -= UI.instance.bossHealth.damageQTEcomplete;
            UI.instance.bossHealth.coreHealth -= comboCount * UI.instance.bossHealth.damageQTEcomboMultiplier;
            UI.instance.AfterQTE_UI(comboCount);
        }

        UI.instance.ToggleQTEScreen();

        //Win QTE
        if (correctHits >= currentMap.beatsForWin)
        {
            if(UI.instance.bossHealth.gameObject.TryGetComponent<KarlBoss>(out KarlBoss karl)){ karl.NextPhase(); }
        }

        //Lose - Repeat phase
        else 
        {
            Debug.LogWarning("too bad, repeat the phase");
            //Reset phase
        }

        correctHits = 0;
        return comboCount;
        //Score;
    }

    //Change the current beat map
    public void SetBeatMap(QTEObject map)
    {
        currentMap = map;
        percentagePerSkull = (float)(1.0f / currentMap.beats.Count);
        FillSong.fillAmount = 0;
    }

    //Change to default beat map
    public void SetDefaultMap()
    {
        currentMap = defaultMap;
        percentagePerSkull = (float)(1.0f / currentMap.beats.Count);
        FillSong.fillAmount = 0;
    }

    public void QTECleanUp()
    {
        currentMap = defaultMap;
        percentagePerSkull = (float)(1.0f / currentMap.beats.Count);
        FillSong.fillAmount = 0;

        beatCounter = 0;
        beatTimer = 0;
        comboCount = 0;
        correctHits = 0;
        healthQTE = 3;
        
        playQTE = false;
        isPlaying = false;



        for (int i = 0; i < _skulls.Count; i++)
        {
            Destroy(_skulls[i].gameObject);
            _skulls.RemoveAt(i);
        }
    }

    //Scale up object over time
    IEnumerator PulseObject(BeatItem obj, float rate)
    {
        Vector3 finalScale = obj.GetDoubleScale();
        for (float i = 0; i <= 1; i += Time.fixedDeltaTime / rate)
        {
            obj.GetObj().transform.localScale = Vector3.Slerp(obj.GetObj().transform.localScale, finalScale, rate);
            yield return null;
        }
        StartCoroutine(EndPulseObject(obj, rate));
    }

    //Scale down object over time
    IEnumerator EndPulseObject(BeatItem obj, float rate)
    {
        Vector3 finalScale = obj.GetScale();
        for (float i = 0; i <= 1; i += Time.fixedDeltaTime / rate)
        {
            obj.GetObj().transform.localScale = Vector3.Slerp(obj.GetObj().transform.localScale, finalScale, rate);
            yield return null;
        }
    }
}
