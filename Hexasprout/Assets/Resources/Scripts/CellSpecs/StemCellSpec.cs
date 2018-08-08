using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StemCellSpec : MonoBehaviour {

    public void ActivateOptionScript()
    {
        this.gameObject.transform.GetChild(3).GetComponent<Canvas>().enabled = true;
    }
    public void DeactivateOptionScript()
    {
        this.gameObject.transform.GetChild(3).GetComponent<Canvas>().enabled = false;
    }

    // EventHandler
    public void EventHandler(GUI_Event e, GUIManager gm)
    {

    }

    // Update is called once per frame
    public void OwnFixedUpdate()
    {

    }
}
