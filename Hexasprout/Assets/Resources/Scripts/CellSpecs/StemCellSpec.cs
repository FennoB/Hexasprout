using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StemCellSpec : MonoBehaviour {

    public CellManager CellManager;

    private void Start()
    {
        CellManager = this.gameObject.transform.GetComponentInParent<CellManager>();
    }

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
        switch (e)
        {
            case GUI_Event.Grow:
                BuildConnection();
                break;
        }
    }
    void BuildConnection()
    {
        for (int i = 0; i < CellManager.connections.Length; i++)
        {

        }
    }

}
