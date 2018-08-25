using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour {

    public Juice cellBuildCache = new Juice();
    public bool buildFlag = false;
    public float progress = 0;
    public CellManager cm;

    public string buildName;
    public float seconds;
    Juice goalJuice;

    //just for testing
    //public float duration = 0;

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
	public void Build(float seconds, Juice juice, string name)
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
                //through seconds is the maximum rate juice get transferred limited
                cm.juice.black = cm.juice.black - (goalJuice.black / seconds) * Time.deltaTime;
                cellBuildCache.black = cellBuildCache.black + (goalJuice.black  / seconds) * Time.deltaTime;
            }
            if (cellBuildCache.green < goalJuice.green)
            {
                cm.juice.green = cm.juice.green - (goalJuice.green / seconds) * Time.deltaTime;
                cellBuildCache.green = cellBuildCache.green + (goalJuice.green / seconds) * Time.deltaTime;
            }
            if (cellBuildCache.blue < goalJuice.blue)
            {
                cm.juice.blue = cm.juice.blue - (goalJuice.blue / seconds) * Time.deltaTime;
                cellBuildCache.blue = cellBuildCache.blue + (goalJuice.blue / seconds) * Time.deltaTime;
            }
            if (cellBuildCache.blueCharged < goalJuice.blueCharged)
            {
                cm.juice.blueCharged = cm.juice.blueCharged - (goalJuice.blueCharged / seconds) * Time.deltaTime;
                cellBuildCache.blueCharged = cellBuildCache.blueCharged + (goalJuice.blueCharged / seconds) * Time.deltaTime;
            }
            if (cellBuildCache.red < goalJuice.red)
            {
                cm.juice.red = cm.juice.red - (goalJuice.red / seconds) * Time.deltaTime;
                cellBuildCache.red = cellBuildCache.red + (goalJuice.red / seconds) * Time.deltaTime;
            }
            if (cellBuildCache.yellow < goalJuice.yellow)
            {
                cm.juice.yellow = cm.juice.yellow - (goalJuice.yellow / seconds) * Time.deltaTime;
                cellBuildCache.yellow = cellBuildCache.yellow + (goalJuice.yellow / seconds) * Time.deltaTime;
            }
            progress = cellBuildCache.Sum / juice.Sum;
            //just for testing
            //duration += Time.deltaTime;
        }
        else
        {
            //just for testing
            //Debug.Log(duration);
            //duration = 0f;

            //goal is reached, cell is allowed to build
            cellBuildCache = new Juice();
            progress = 0f;
            buildFlag = false;
            name = null;
            //send Event to build to that cell, which wanted to build
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
