using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
    public GameObject CreditsPanel;

    public void ToggleCreditsPanel()=>CreditsPanel.SetActive(!CreditsPanel.activeInHierarchy);
    


}
