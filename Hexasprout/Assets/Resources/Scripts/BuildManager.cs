using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour {

    public Juice cellBuildCache = new Juice();
    public bool buildFlag = false;
    public float progress = 0;
    public CellManager cm;

    string buildName;
    int seconds;
    Juice goalJuice;

    // Use this for initialization
    void Awake ()
    {
        cm = this.gameObject.GetComponent<CellManager>();
	}
	
    /**
     *This method gets the juice out of a cell 
     * till a specific goal is reached,
     *then the event is triggered, that the 
     * BuildJob is completed
     */
	public void Build(int seconds, Juice juice, string name)
    {
        //says that this Manager is already used
        buildFlag = true;

        //general necessary information
        buildName = name;
        this.seconds = seconds;
        goalJuice = juice;

        //add juice so long, until the goal is reached
        //each component of the juice must be handled separately, because there is no way to iterate thorugh the juice
        if (cellBuildCache < goalJuice)
        {
            if (cellBuildCache.black < goalJuice.black)
            {
                cm.juice.black = cm.juice.black - (cm.juice.black / seconds) * Time.deltaTime;
                cellBuildCache.black = cellBuildCache.black + (cm.juice.black / seconds) * Time.deltaTime;
            }
            if (cellBuildCache.green < goalJuice.green)
            {
                cm.juice.green = cm.juice.green - (cm.juice.green / seconds) * Time.deltaTime;
                cellBuildCache.green = cellBuildCache.green + (cm.juice.green / seconds) * Time.deltaTime;
            }
            if (cellBuildCache.blue < goalJuice.blue)
            {
                cm.juice.blue = cm.juice.blue - (cm.juice.blue / seconds) * Time.deltaTime;
                cellBuildCache.blue = cellBuildCache.blue + (cm.juice.blue / seconds) * Time.deltaTime;
            }
            if (cellBuildCache.blueCharged < goalJuice.blueCharged)
            {
                cm.juice.blueCharged = cm.juice.blueCharged - (cm.juice.blueCharged / seconds) * Time.deltaTime;
                cellBuildCache.blueCharged = cellBuildCache.blueCharged + (cm.juice.blueCharged / seconds) * Time.deltaTime;
            }
            if (cellBuildCache.red < goalJuice.red)
            {
                cm.juice.red = cm.juice.red - (cm.juice.red / seconds) * Time.deltaTime;
                cellBuildCache.red = cellBuildCache.red + (cm.juice.red / seconds) * Time.deltaTime;
            }
            if (cellBuildCache.yellow < goalJuice.yellow)
            {
                cm.juice.yellow = cm.juice.yellow - (cm.juice.yellow / seconds) * Time.deltaTime;
                cellBuildCache.yellow = cellBuildCache.yellow + (cm.juice.yellow / seconds) * Time.deltaTime;
            }
            progress = cellBuildCache.Sum / juice.Sum;
        }
        else
        {
            //goal is reached, cell is allowed to build
            cellBuildCache = new Juice();
            progress = 0f;
            buildFlag = false;
            name = null;
            cm.EventHandler(GUI_Event.BuildReady, null);
        }
    }
    public void OwnFixedUpdate()
    {
        if (buildFlag)
        {
            Build(seconds, goalJuice, buildName);
        }
    }
}
