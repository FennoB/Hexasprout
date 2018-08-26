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
    public float duration = 0;

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
        if (cellBuildCache < goalJuice)
        {
            //iterate through all components of the juice
            for(int i = 0; i < cellBuildCache.Length; i++)
            {
                //is the component not reached?
                if (cellBuildCache[i] < goalJuice[i])
                {
                    HandleJuiceComponent(i);
                }
            }
            progress = cellBuildCache.Sum / juice.Sum;
            //just for testing
            duration += Time.deltaTime;
        }
        else
        {
            //just for testing
            Debug.Log("Dauer des Builds : " + duration);
            duration = 0f;

            //goal is reached, cell is allowed to build
            cellBuildCache = new Juice();
            progress = 0f;
            buildFlag = false;
            name = null;
            //send Event to build to that cell, which wanted to build
            cm.EventHandler(GUI_Event.BuildReady, null);
        }
    }

    void HandleJuiceComponent(int i)
    {
        //is the next step for the second possible?
        if ((goalJuice[i] / seconds) < cm.juice[i])
        {
            //is there enough place in the buildcache for the next load of juice?
            if (cellBuildCache[i] + (goalJuice[i] / seconds) * Time.deltaTime < goalJuice[i])
            {
                //through seconds is the maximum rate juice get transferred limited
                float step = (goalJuice[i] / seconds) * Time.deltaTime;

                cm.juice[i] = cm.juice[i] - step;
                cellBuildCache[i] = cellBuildCache[i] + step;
            }
            else
            {
                //get the rest of the juice
                float dif = goalJuice[i] - cellBuildCache[i];

                cm.juice[i] = cm.juice[i] - dif;
                cellBuildCache[i] = cellBuildCache[i] + dif;
            }
        }
        //getting the rest of the juice in one second, if there's not enough juice for the normal steps
        else
        {
            //is there enough place in the cache for the next load?
            if (cellBuildCache[i] + cm.juice[i] * Time.deltaTime < goalJuice[i])
            {
                float step = cm.juice[i] * Time.deltaTime;

                cm.juice[i] = cm.juice[i] - step;
                cellBuildCache[i] = cellBuildCache[i] + step;
            }
            else
            {
                float dif = goalJuice[i] - cellBuildCache[i];

                cm.juice[i] = cm.juice[i] - dif;
                cellBuildCache[i] = cellBuildCache[i] + dif;
            }
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
