using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafCellSpec : MonoBehaviour {

    public float conversionMax;
    public CellManager CellManager;
    public FieldManager Field;

	// Use this for initialization
	void Start ()
    {
        CellManager = GetComponent<CellManager>();
        Field = GetComponent<Transform>().parent.GetComponent<FieldManager>();
    }

    // Update is called fixed
    public void OwnFixedUpdate()
    {
        Absorb();
    }

    public void Absorb()
    {
        if(CellManager == null)
        {
            CellManager = GetComponent<CellManager>();
            Field = GetComponent<Transform>().parent.GetComponent<FieldManager>();
        }

        FieldManager fm = Field;
        CellManager cm = CellManager;
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
        switch(e)
        {
            case GUI_Event.OpenMenu:
                gm.AddSliderButton(GUI_Event.BtnDegenerate);
                break;
            case GUI_Event.BtnSpecialize:
                gm.AddSliderButton(GUI_Event.BtnLeafSpeed);
                break;
            case GUI_Event.BtnLeafSpeed:
                gm.CloseCellMenu();
                CellManager.loadBarPicture = GUI_Event.BtnLeafSpeed;
                gm.OpenLoadBar(GUI_Event.BtnLeafSpeed);
                conversionMax *= 1.2f;
                CellManager.BuildManager.Build(20, new Juice(0, 0, 0.2f, 0, 0, 0.5f), "Leaf Speed");
                break;

        }
    }
}
