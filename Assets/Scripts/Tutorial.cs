using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public GameObject progressWall1;

    bool listGot = false;

    public void GotList()
    {
        listGot = true;
        //Allow to go to next area
        Destroy(progressWall1);
    }
}
