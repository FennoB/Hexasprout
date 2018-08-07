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
        content = new Juice();
        capacities = new Juice();
        target = new Juice();
        capacities.SetAllTo(10.0f);
    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    // Storage functions
    void StorageHandling()
    {
        // Get rid of buggy situations:
        
        
        // Get some data and create some variables
        Juice juice = GetComponent<CellManager>().juice;    // Juice of the cell
        Juice delta = target - content;     // Amount of juice to be stored/released

    }

    // EventHandler
    public void EventHandler(GUI_Event e, GUIManager gm)
    {

    }

}
