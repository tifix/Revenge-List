using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class QTEMovement : MonoBehaviour
{
    Controls input;

    public GameObject up, down, left, right;

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
                GameObject tx = SpawnFeedback(obj);
                //Bad hit
                if (Vector2.Distance(obj.transform.position, col.transform.position) > col.transform.localScale.x)
                {
                    col.gameObject.GetComponent<SkullController>().Kill();
                    tx.GetComponent<TextMeshPro>().text = "Bad";
                    Destroy(tx, 1);
                    return;
                }
                //Good hit
                else if (Vector2.Distance(obj.transform.position,col.transform.position) < col.transform.localScale.x/0.2f)
                {
                    col.gameObject.GetComponent<SkullController>().Kill();
                    tx.GetComponent<TextMeshPro>().text = "Perfect";
                    Destroy(tx, 1);
                    return;
                }
                //Meh hit
                else
                {
                    col.gameObject.GetComponent<SkullController>().BadHit();
                    tx.GetComponent<TextMeshPro>().text = "Good";
                    Destroy(tx, 1);
                    return;
                }
            }
        }
        //Check there is smth, check distance for perfect or good hit.
    }

    GameObject SpawnFeedback(GameObject obj)
    {
        GameObject temp = Instantiate(new GameObject(), obj.transform.position, Quaternion.identity);
        temp.AddComponent<TextMeshPro>();
        return temp;
    }
}
