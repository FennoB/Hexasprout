using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafCellSpec : MonoBehaviour {

    public float conversionMax;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called fixed
    void FixedUpdate()
    {
    }

    public void Absorb()
    {
        FieldManager fm = GetComponent<Transform>().parent.GetComponent<FieldManager>();
        CellManager cm = GetComponent<CellManager>();
        float conv = cm.juice.blue;

        if(conv > conversionMax)
        {
            conv = conversionMax;
        }

        conv *= fm.warmth * Time.deltaTime;

        cm.juice.blueCharged += conv;
        cm.juice.blue -= conv;
    }

    // EventHandler
    public void EventHandler(GUI_Event e, GUIManager gm)
    {

    }

}
