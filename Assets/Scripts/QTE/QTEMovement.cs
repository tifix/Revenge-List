using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class QTEMovement : MonoBehaviour
{
    public static QTEMovement instance;

    Controls input;

    public GameObject up, down, left, right;
    public GameObject perfect, good, bad;

    public float perfectMin, badMin;

    public float coolDown = 0.1f;
    float lastPress = 0;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        input = new Controls();

        input.QTE.Up.performed += Up;
        input.QTE.Up.Enable();

        input.QTE.Down.performed += Down;
        input.QTE.Down.Enable();

        input.QTE.Left.performed += Left;
        input.QTE.Left.Enable();

        input.QTE.Right.performed += Right;
        input.QTE.Right.Enable();
    }
    void OnDisable()
    {
        input.QTE.Up.Disable();
        input.QTE.Down.Disable();
        input.QTE.Left.Disable();
        input.QTE.Right.Disable();
    }

    void Up(InputAction.CallbackContext obj)
    {
        if(Time.time >= lastPress + coolDown)
            BeatCheck(up);
    }

    void Down(InputAction.CallbackContext obj)
    {
        if (Time.time >= lastPress + coolDown)
            BeatCheck(down);
    }

    void Left(InputAction.CallbackContext obj)
    {
        if (Time.time >= lastPress + coolDown)
            BeatCheck(left);
    }

    void Right(InputAction.CallbackContext obj)
    {
        if (Time.time >= lastPress + coolDown)
            BeatCheck(right);
    }

    void BeatCheck(GameObject obj)
    {
        lastPress = Time.time;
        //Trigger anim for obj
        obj.GetComponent<Animator>().SetTrigger("Scale");

        //Check if there is a skull inside the collider, if not, bad hit
        List<Collider2D> cols = new List<Collider2D>();
        obj.GetComponent<BoxCollider2D>().OverlapCollider(new ContactFilter2D().NoFilter(), cols);

        //Go through colliders
        foreach (Collider2D col in cols) 
        { 
            //If there is a skull
            if(col.CompareTag("Skull"))
            {
                //Bad hit
                if (Vector2.Distance(obj.transform.position, col.transform.position) >= badMin)
                {
                    col.gameObject.GetComponent<SkullController>().BadHit();
                    GameObject temp = Instantiate(bad, obj.transform.parent);
                    temp.transform.localScale = new Vector3(40, 40, 40);
                    return;
                }
                //Good hit
                else if (Vector2.Distance(obj.transform.position,col.transform.position) < perfectMin)
                {
                    col.gameObject.GetComponent<SkullController>().Kill();
                    GameObject temp = Instantiate(perfect, obj.transform.parent);
                    temp.transform.localScale = new Vector3(40, 40, 40);
                    return;
                }
                //Meh hit
                else
                {
                    col.gameObject.GetComponent<SkullController>().Kill();
                    GameObject temp = Instantiate(good, obj.transform.parent);
                    temp.transform.localScale = new Vector3(40, 40, 40);
                    return;
                }
            }
        }
        //Nothing inside, check if hit too early/late
        cols = new List<Collider2D>();
        obj.transform.GetChild(0).GetComponent<BoxCollider2D>().OverlapCollider(new ContactFilter2D().NoFilter(), cols);
        foreach (Collider2D col in cols)
        {
            //If there is a skull
            if (col.CompareTag("Skull"))
            {
                col.gameObject.GetComponent<SkullController>().BadHit();
                GameObject temp = Instantiate(bad, obj.transform.parent);
                temp.transform.localScale = new Vector3(40, 40, 40);
                return;
            }
        }
        //Check there is smth, check distance for perfect or good hit.
    }

    public void SpawnBadFeedback()
    {
        GameObject temp = Instantiate(bad, transform.parent);
        temp.transform.localScale = new Vector3(40, 40, 40);
    }
}
