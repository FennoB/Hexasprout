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
            case GUI_Event.LeafSpeed:
                IncreaseLeafSpeed(gm);
                break;
            case GUI_Event.BuildReady:
                FinishBuild(CellManager.BuildManager.buildName);
                break;
        }
    }
    void FinishBuild(string buildName)
    {
        float dif = GameObject.FindGameObjectWithTag("GUI").transform.GetChild(6).gameObject.GetComponent<JobCache>().dif;
        switch (buildName)
        {
            case "Increase Conversion Rate":
                conversionMax += dif;
                break;
        }
    }
    public void IncreaseLeafSpeed(GUIManager gm)
    {
        gm.CloseCellMenu();
        CellManager.loadBarPicture = GUI_Event.BtnLeafSpeed;
        gm.OpenLoadBar(GUI_Event.BtnLeafSpeed);

        JobCache jobCache = gm.transform.GetChild(6).gameObject.GetComponent<JobCache>();
        CellManager.BuildManager.Build(jobCache.seconds, jobCache.juice, jobCache.title);
    }
}
