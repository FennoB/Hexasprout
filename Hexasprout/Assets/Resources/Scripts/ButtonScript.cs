using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public GUI_Event ButtonID;
    public GameObject gui;

    // Called, when button clicked
    public void HandleClick()
    {
        gui.GetComponent<GUIManager>().EventHandler(ButtonID);
    }
}
