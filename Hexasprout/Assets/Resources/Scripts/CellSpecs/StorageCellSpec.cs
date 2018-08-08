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
    public void OwnFixedUpdate()
    {

    }

    // Storage functions
    void StorageHandling()
    {
        // Get some data and create some variables
        Juice juice = GetComponent<CellManager>().juice;    // Juice of the cell
        Juice delta = target - content;     // Amount of juice to be exchanged
        float room = 1f - juice.Sum;

        delta *= Time.deltaTime;

        // Is there enough of each material in juice for delta?
        // Apply delta, get negatives as positive values
        Juice overhead = ((juice - delta) * -1f).PositivesOnly;
        // Get rid of the overhead
        delta -= overhead;

        // Does -delta fit in room?
        float overflow = -delta.Sum / room;
        if(overflow > 1f)
        {
            delta = delta.PositivesOnly + ((delta * -1.0f).PositivesOnly / -overflow);
        }

        // Is deltas absolute sum bigger than max?
        if (delta.AbsoluteValues.Sum > maxSpeedPerSecond * Time.deltaTime)
        {
            delta /= (delta.AbsoluteValues.Sum / (maxSpeedPerSecond * Time.deltaTime));
        }

        // Apply delta
        content += delta;
        juice -= delta;
    }

    // EventHandler
    public void EventHandler(GUI_Event e, GUIManager gm)
    {

    }

}
