using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    float beatTimer;
    int beatCounter;
    int beatProgress;
    int correctHits;

    public GameObject skullPrefab;
    public GameObject skullVFX;
    public GameObject BeatUI;

    public Image FillSong;

    [Range(0.1f,1.0f)]
    public float beatScale;

    public TMP_Text countDownUI;

    public Transform up, down, left, right;

    public List<BeatItem> beatObjects; 
    List<SkullController> _skulls;

    [SerializeField] float percentagePerSkull;

    Animator anim;

    float spawnRate = 0.5f;
    int beatOffset = 0;
    bool isFinishingQTE=false;    //qte outro is invoked multiple times, leading to issues. THis assures no multiple instances are run in parallel

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

        percentagePerSkull = (float)(1.0f / (currentMap.beats.Count - healthQTE + 1));

        playQTE = false;

        beatCounter = 0;
        beatProgress = 0;
        correctHits = 0;
        beatTimer = Time.time - 0.5f;

        _skulls = new List<SkullController>();

        for (int i = 0; i < beatObjects.Count; i++)
        {
            //Force constructor
            beatObjects[i] = new BeatItem(beatObjects[i].GetObj());        
        }
        beatObjects.Add(new BeatItem(BeatUI));
        beatOffset = beatObjects.Count();

        anim = GetComponent<Animator>();

        countDownUI.SetText("3");
    }

    public void QTEStart()
    {
        QTECleanUp();
        anim.SetTrigger("QTE_entered");
        GameManager.instance.SetPause(false);
        PlayerMovement.instance.SetLockMovement();

        if (currentMap == null)
            currentMap = defaultMap;

        percentagePerSkull = (float)(1.0f / (currentMap.beats.Count - healthQTE + 1));      //for fill you need
        FillSong.fillAmount = 0;

        anim.SetBool("QTE_Playing", true);
        StartCoroutine(QTECountDown());
    }

    IEnumerator QTECountDown()
    {
        countDownUI.gameObject.SetActive(true);
        countDownUI.SetText("3");

        for (int i = 3; i > 0; i--)
        {
            countDownUI.SetText(i.ToString());
            PlayerMovement.instance.PauseMovement();
            yield return new WaitForSeconds(1);
        }
        countDownUI.gameObject.SetActive(false);
        countDownUI.SetText("");
        PlayerMovement.instance.PauseMovement();
        playQTE = true;
    }

    void Update()
    {       
        if (playQTE)
        {          
            //BeatUI.transform.Rotate(new Vector3(0, 0, -100 * Time.deltaTime));
            //Every 0.5s, assuming 120bpm
            if (beatTimer + 0.5f <= Time.time)
            {
                //Get beat time
                beatTimer = Time.time;
                beatCounter++;

                //Animate objects
                for (int i = 0; i < beatObjects.Count; i++)
                {
                    StartCoroutine(PulseObject(beatObjects[i], beatScale));
                }

                //Spawn skull if there is a beat in the current progress and beats should spawn
                if(beatProgress < currentMap.beats.Count && beatCounter == currentMap.beats[beatProgress].timing)
                {
                    SpawnSkull();
                    beatProgress++;
                }
            }

            //If there are no more beats
            if (beatProgress >= currentMap.beats.Count)
            {
                //Wait until no more skulls
                if(_skulls.Count<=0)
                {
                    if (!isFinishingQTE) 
                    { 
                        isFinishingQTE = true;
                        beatTimer = Time.time - 0.5f;
                        Debug.Log("skull track finished");
                        Invoke("QTEEnd", 0.5f);
                        //QTEEnd();
                    }
                    else { }

                }
            }

            if (_skulls.Count > 0)
            {
                for (int i = 0; i < _skulls.Count; i++)
                {
                    //Hit the player
                    if (Vector3.Distance(_skulls[i].transform.localPosition, Vector3.zero) <= _skulls[i].transform.localScale.x)
                    {
                        //Death VFX
                        Instantiate<GameObject>(skullVFX, _skulls[i].transform.position, Quaternion.identity);
                        Debug.Log("miss! Going red");
                        QTEMovement.instance.SpawnBadFeedback();
                        _skulls[i].gameObject.GetComponentInChildren<Animator>().SetTrigger("Miss");
                        anim.SetTrigger("QTE_MissCircle");  //playing the miss animation on the qte ring itself
                        
                        Destroy(_skulls[i].gameObject, 0.2f);
                        _skulls.RemoveAt(i);
                        beatObjects.RemoveAt(i + beatOffset);
                        AudioManager.instance.PlaySFX("qteMiss");
                        healthQTE--;

                        //Fail QTE
                        if(healthQTE <= 0)
                        {
                            //End QTE
                            correctHits = 0;
                            QTEEnd();
                        }
                        break;
                    }

                    //Perfect hit
                    else if (!_skulls[i].GetIsAlive())
                    {
                        //Death VFX
                        Instantiate<GameObject>(skullVFX, _skulls[i].transform.position, Quaternion.identity);
                        Debug.Log("perfect");
                        AudioManager.instance.PlaySFX("qteHit");
                        _skulls[i].gameObject.GetComponentInChildren<Animator>().SetTrigger("Perfect");  
                        Destroy(_skulls[i].gameObject,0.2f);
                        _skulls.RemoveAt(i);
                        beatObjects.RemoveAt(i + beatOffset);
                        correctHits++;
                        FillSong.fillAmount += percentagePerSkull;
                        break;
                    }

                    //Good hit
                    else if (_skulls[i].GetBadHit())
                    {
                        //Death VFX
                        Instantiate<GameObject>(skullVFX, _skulls[i].transform.position, Quaternion.identity);
                        Debug.Log("meh hit");
                        _skulls[i].gameObject.GetComponentInChildren<Animator>().SetTrigger("Hit");
                        Destroy(_skulls[i].gameObject, 0.2f);
                        _skulls.RemoveAt(i);
                        beatObjects.RemoveAt(i + beatOffset);
                        FillSong.fillAmount += percentagePerSkull;
                    }
                }
                
            }
        }
    }

    void SpawnSkull()
    {
        Beats currentBeat = currentMap.beats[beatProgress].myBeat;
        switch (currentBeat) 
        {   
            case Beats.Up: _skulls.Add(Instantiate<GameObject>(skullPrefab, up.position, up.rotation, transform.GetChild(0)).GetComponent<SkullController>());              //Set skull parent to QTE child, so leftover skulls will be hidden along with the QTE UI -M
                break;
            case Beats.Down: _skulls.Add(Instantiate<GameObject>(skullPrefab, down.position, down.rotation, transform.GetChild(0)).GetComponent<SkullController>());
                break;
            case Beats.Left: _skulls.Add(Instantiate<GameObject>(skullPrefab, left.position, left.rotation, transform.GetChild(0)).GetComponent<SkullController>());
                break;
            case Beats.Right: _skulls.Add(Instantiate<GameObject>(skullPrefab, right.position, right.rotation, transform.GetChild(0)).GetComponent<SkullController>());
                break;
            default: return;
        }       
        _skulls.Last<SkullController>().SetSpeed(currentMap.beats[beatProgress].speed);
        beatObjects.Add(new BeatItem(_skulls.Last<SkullController>().gameObject));
    }

    public void QTEEnd()
    {
        //Lose - Repeat phase
        if(healthQTE <= 0 || correctHits < currentMap.beatsForWin)
        {
            Debug.LogWarning("too bad, repeat the phase");
            if (UI.instance.bossHealth != null)
            {
                UI.instance.CleanupHealthBoss(false);
            }
            if (UI.instance.bossHealth.gameObject.TryGetComponent<BossClass>(out BossClass boss)) { boss.RepeatPhase(); }

            //Play the QTE won animation
            anim.SetBool("QTE_Playing", false);
            anim.SetBool("QTE_Won", false);
        }

        //Win QTE
        else if (correctHits >= currentMap.beatsForWin || GameManager.instance.cheat_QTEAlwaysWin)
        {           
            if (UI.instance.bossHealth != null)
            {
                UI.instance.bossHealth.coreHealth -= UI.instance.bossHealth.damageQTEcomplete;

                //Check if the boss is completelly killed, if so, invoke the boss death stuffs
                if(UI.instance.bossHealth.gameObject.TryGetComponent<BossClass>(out BossClass boss)) 
                {
                    if (UI.instance.bossHealth.coreHealth > 0)
                        boss.NextPhase();
                    else boss.BossDefeated();
                }

                //clean-up health bars as well :)
                if (UI.instance.bossHealth.coreHealth < 1) UI.instance.CleanupHealthBoss(true);
                else UI.instance.CleanupHealthBoss(false);
            }

            //Play the QTE won animation
            anim.SetBool("QTE_Playing", false);
            anim.SetBool("QTE_Won", true);
            AudioManager.instance.PlayMusic("BossTrack");
        }

        QTECleanUp();
    }

    public void QTECleanUp()
    {
        percentagePerSkull = (float)(1.0f / (currentMap.beats.Count - healthQTE + 1));
        FillSong.fillAmount = 0;

        correctHits = 0;
        beatCounter = 0;
        beatProgress = 0;
        beatTimer = Time.time - 0.5f;
        healthQTE = 3;

        playQTE = false;

        for (int i = 0; i < _skulls.Count; i++)
        {
            Destroy(_skulls[i].gameObject);
            _skulls.RemoveAt(i);
            beatObjects.RemoveAt(i + beatOffset);
        }
        isFinishingQTE = false; // - resetting the single outro checker for the next phase
    }

    //Change the current beat map
    public void SetBeatMap(QTEObject map)
    {
        currentMap = map;
        percentagePerSkull = (float)(1.0f / (currentMap.beats.Count - healthQTE+1));
        FillSong.fillAmount = 0;
    }

    //Change to default beat map
    public void SetDefaultMap()
    {
        currentMap = defaultMap;
        percentagePerSkull = (float)(1.0f / (currentMap.beats.Count - healthQTE + 1));
        FillSong.fillAmount = 0;
    }

    //Scale up object over time
    IEnumerator PulseObject(BeatItem obj, float rate)
    {
        Vector3 finalScale = obj.GetDoubleScale();
        for (float i = 0; i <= 1; i += Time.fixedDeltaTime / rate)
        {
            try { obj.GetObj().transform.localScale = Vector3.Slerp(obj.GetObj().transform.localScale, finalScale, rate); }
            catch { Debug.LogWarning("Pulsing object would continue pulsing past it's death"); }            
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
            try 
            {
                obj.GetObj().transform.localScale = Vector3.Slerp(obj.GetObj().transform.localScale, finalScale, rate);
            }
            catch { Debug.Log("Skull expired"); }
            yield return null;
        }
    }
}
