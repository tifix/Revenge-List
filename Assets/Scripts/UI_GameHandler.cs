using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
public struct TextData 
{
    public List<string> textBody;// = new List<string>;
    public List<string> textSpeaker;// = new List<string>;

    public TextData(List<string> _body, List<string> _speaker) { textBody = _body; textSpeaker = _speaker; }
    //public TextData(List<string> _body, string _speaker) { textBody.Add(" "); textSpeaker = _speaker; }
}
*/


public class UI_GameHandler : MonoBehaviour
{
    public static UI_GameHandler instance;
    public Dialogue dialogueCur;

    public bool isWaiting = false;
    protected bool choosing;
    [SerializeField] protected bool runCoroutine; //we need this so that the coroutine doesn't try to run every frame
    protected string pageText="VOID";

    [SerializeField] protected GameObject textBox; public GameObject interactPrompt;  //object that displays dialogue , the prompt to interact object 
    [SerializeField] Text txtMain, txtSpeaker;                //the text that displays the dialogue in UI.   Aaaand the caption of WHO is speaking

    [Tooltip("What part of the dialogue is displayed"), Range(0,99)] public int txtPageNr = 0; //this is used to get dialogue onwards with switch case

    [Tooltip("time in s between each letter typed")]public float typingWait = 0.03f; //how much time passes between the letters typed


    public void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }


    public void Update()
    {
      if(dialogueCur!=null) DialogueStoppingLogic();
    }


    public virtual void DialogueStoppingLogic() //advancing the dialogue and determining if we can skip it
    {
        
        if (Input.GetKeyDown(KeyCode.Backspace) && !choosing)
        {
            if (isWaiting) { txtPageNr++; isWaiting = false; }
            else
            {
                runCoroutine = false;
            }
        }
    }


    protected IEnumerator Typer(Dialogue _dialogue) //typing the text over time
    {
        //Debug.Log("Starting Display of "+_dialogue.textBody[0]);
        /*
         * PlayerMovement.canMove=false;
         */
        
        dialogueCur = _dialogue;
        txtPageNr = 0;      //page iterator

        while (txtPageNr < _dialogue.textBody.Count)
        {
            runCoroutine = true;
            //speaker set
            if (_dialogue.textSpeaker.Count > 1) txtSpeaker.text = _dialogue.textSpeaker[txtPageNr];      //if dialogue switches between speakers - updates speaker. If just one, doesn't bother checking.
            else txtSpeaker.text = _dialogue.textSpeaker[0];

            //get text data
            pageText = _dialogue.textBody[txtPageNr];


            //Slowly display the page text
            for (int i = 0; i < (pageText.Length + 1); i++)
            {
                txtMain.text = pageText.Substring(0, i);               //slice the text 
                yield return new WaitForSeconds(typingWait);
                if (!runCoroutine) break;
            }
            
            txtMain.text = pageText;                                   //display the text
            isWaiting = true;

            while (isWaiting == true) yield return null;               //hold until player progresses the text
            
        }
        runCoroutine = false;
        dialogueCur = null;
        textBox.SetActive(false);

        /*
         * PlayerMovement.canMove=false;
         */
    }
    public void Show(Dialogue _dialogue)
    {
        textBox.SetActive(true);

        Debug.Log("initialising text");
        if (!runCoroutine)
        {
            StartCoroutine(Typer(_dialogue));
            runCoroutine = true;
        }
    }    

}
