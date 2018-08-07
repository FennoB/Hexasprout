using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageCellSpec : MonoBehaviour
{

    public Juice content;
    public Juice capacities;
    public Juice target;
    public float maxSpeedPerSecond;

	// Use this for initialization
	void Start ()
    {
        content.red             = 0;
        content.black           = 0;
        content.blue            = 0;
        content.blueCharged     = 0;
        content.green           = 0;
        content.yellow          = 0;
        capacities.red          = 10.0f;
        capacities.black        = 10.0f;
        capacities.blue         = 10.0f;
        capacities.blueCharged  = 10.0f;
        capacities.green        = 10.0f;
        capacities.yellow       = 10.0f;
        target.red              = 0;
        target.black            = 0;
        target.blue             = 0;
        target.blueCharged      = 0;
        target.green            = 0;
        target.yellow           = 0;
    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    // Storage functions
    void StorageHandling()
    {
        Juice juice = GetComponent<CellManager>().juice;
        Juice deltas;
        
    }

    // EventHandler
    public void EventHandler(GUI_Event e, GUIManager gm)
    {

    }

}
