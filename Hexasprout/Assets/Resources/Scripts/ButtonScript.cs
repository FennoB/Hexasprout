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
    
    // Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
