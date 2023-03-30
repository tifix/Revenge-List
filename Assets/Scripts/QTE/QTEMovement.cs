using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class QTEMovement : MonoBehaviour
{
    Controls input;

    public GameObject up, down, left, right;

    bool rotating = false;
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
    private void OnDisable()
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

    private void BeatCheck(GameObject obj)
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
                //Good hit
                if(Vector2.Distance(obj.transform.position,col.transform.position) < col.transform.localScale.x/0.2f)
                {
                    col.gameObject.GetComponent<SkullController>().Kill();
                    return;
                }
                //Meh hit
                else
                {
                    col.gameObject.GetComponent<SkullController>().BadHit();
                    return;
                }
            }
        }
        //Hit there is smth, check distance for perfect or good hit.
    }
}
