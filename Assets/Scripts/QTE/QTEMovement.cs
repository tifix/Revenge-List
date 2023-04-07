using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class QTEMovement : MonoBehaviour
{
    Controls input;

    public GameObject up, down, left, right, fbck;

    public float perfectMin, badMin;

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
        BeatCheck(up);
    }

    void Down(InputAction.CallbackContext obj)
    {
        BeatCheck(down);
    }

    void Left(InputAction.CallbackContext obj)
    {
        BeatCheck(left);
    }

    void Right(InputAction.CallbackContext obj)
    {
        BeatCheck(right);
    }

    void BeatCheck(GameObject obj)
    {
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
                    GameObject temp = Instantiate(fbck, obj.transform.parent);
                    temp.GetComponent<TMP_Text>().text = "Bad";
                    return;
                }
                //Good hit
                else if (Vector2.Distance(obj.transform.position,col.transform.position) < perfectMin)
                {
                    col.gameObject.GetComponent<SkullController>().Kill();
                    GameObject temp = Instantiate(fbck, obj.transform.parent);
                    temp.GetComponent<TMP_Text>().text = "Perfect";
                    return;
                }
                //Meh hit
                else
                {
                    col.gameObject.GetComponent<SkullController>().BadHit();
                    GameObject temp = Instantiate(fbck, obj.transform.parent);
                    temp.GetComponent<TMP_Text>().text = "Good";
                    return;
                }
            }
        }
        //Check there is smth, check distance for perfect or good hit.
    }
}
