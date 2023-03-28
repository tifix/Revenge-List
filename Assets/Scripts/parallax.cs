using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

/*
 * as the player moves to the right, slowly scroll to the right, as they move left, move left slowly 
 * 
 */

public class parallax : MonoBehaviour
{
    public float intensity;
    public GameObject playerTrack;
    [SerializeField] Vector3 originPosition;
    public CinemachineVirtualCamera cam;
    [SerializeField] Vector3 camPosMemory;

    private void Start()
    {
       originPosition = transform.position;
       if(playerTrack==null) playerTrack = PlayerMovement.instance.gameObject;
       if(cam == null) cam = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
    }

    void LateUpdate()
    {
        if (!Equals(cam.transform.position, camPosMemory)) //if the cinemachine camera is moving (not in dead zone)
        {
            transform.position = originPosition + new Vector3(playerTrack.transform.position.x, originPosition.y, originPosition.z) * intensity; //move the parallax 

        } 
        camPosMemory = cam.transform.position;
    }
}
