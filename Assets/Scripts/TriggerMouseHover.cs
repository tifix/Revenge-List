using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class TriggerMouseHover : MonoBehaviour
{
    public UnityEvent WhenOverDO = new UnityEvent();
    private void OnMouseOver()
    {
        Debug.Log("woosh!");
        WhenOverDO.Invoke();
    }
}
