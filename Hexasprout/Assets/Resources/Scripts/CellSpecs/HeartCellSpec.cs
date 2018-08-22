using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartCellSpec : MonoBehaviour {

    // Heartmap. A list of lists of decisions
    public List<List<int>> heartmap;

    // Cyclespeeds
    public List<float> cyclespeed;
    public List<float> cycletimers;

    public float heartspeed;
    public float maxspeed = 1;
    public int pumpdirection;
    public float resistanceUnit;
    public bool pumping = false;

    public CellManager CellManager;

    // Use this for initialization
    void Awake()
    {
        heartmap = null;
        cyclespeed = null;
        cycletimers = new List<float>();
        pumpdirection = 0;
        resistanceUnit = 0.2f;
        CellManager = GetComponent<CellManager>();
        CellManager.ConnectionMax = 2;
        CellManager.cellType = CellType.Heartcell;
    }

    // Fixed Update
    public void OwnFixedUpdate()
    {
        // Orientation
        transform.rotation = Quaternion.identity;
        int dir = (pumpdirection + 3) % 6;
        if (dir - 1 == -1)
        {
            transform.Rotate(new Vector3(0, 0, -60));
        }
        else
        {
            transform.Rotate(new Vector3(0, 0, (dir - 1) * 60));
        }

        for (int i = 0; i < cycletimers.Count; i++)
        {
            cycletimers[i] += Time.deltaTime;
        }

        if (heartmap != null && cyclespeed != null)
        {
            pumping = true;
            Pump();
        }
        else
        {
            pumping = false;
            UpdateHeartmap();
        }

        GetComponentInChildren<Animator>().SetBool("Pump", pumping);
    }

    // Pump direction
    public void UpdatePumpDirection()
    {
        // Find first connection and reset Pump Direction
        // This function has to be called after every Connection Change of the cell
        for (int i = 0; i < CellManager.connections.Length; i++)
        {
            if (CellManager.connections[i] != null)
            {
                GetComponent<HeartCellSpec>().pumpdirection = (i + 3) % 6;

                if (i - 1 == -1)
                {
                    GetComponent<Transform>().Rotate(new Vector3(0, 0, -60));
                }
                else
                {
                    GetComponent<Transform>().Rotate(new Vector3(0, 0, (i - 1) * 60));
                }
                break;
            }
        }
    }

    // Pumps Juice through the heartcycles in heartmap
    void Pump()
    {
        // Get the CellManager of this cell
        CellManager cm = GetComponent<CellManager>();

        // Go through all circles
        for (int i = 0; i < heartmap.Count; i++)
        {
            // Calculate the number ob ticks this circle has to do
            int ticks = (int)(cyclespeed[i] * cycletimers[i]);

            // Decrease timer
            cycletimers[i] -= ticks / cyclespeed[i];

            // Do the ticks
            for(int tick = 0; tick < ticks; tick++)
            {
                // Start with this cell. 
                CellManager cm2 = cm.connections[pumpdirection].GetComponent<CellManager>();

                // Buffer the juice
                Juice buf = cm2.juice;

                // Go through the heartmap circle
                for (int j = 0; j < heartmap[i].Count; j++)
                {
                    // Get the next exchange partner
                    CellManager cm3 = cm2.connections[heartmap[i][j]].GetComponent<CellManager>();

                    // Another buffer
                    Juice lbuf = cm3.juice;

                    // Exchange
                    cm3.juice = buf;
                    buf = lbuf;

                    // Go to the next cell
                    cm2 = cm3;
                }

                // Put the rest back together
                cm.connections[pumpdirection].GetComponent<CellManager>().juice = cm.juice;
                cm.juice = buf;
            }
        }
    }

    // Recreate the Heartmap
    public void UpdateHeartmap()
    {
        // Check if connection count is 2
        if(CellManager.ConnectionCounter != 2)
        {
            heartmap = null;
            // There is nothing to be done here
            return;
        }

        // Call this function when:
        // - Connections removed
        // - Connections added
        // - Heartcell added or changed
        // - Other Heartcell events

        // The Heartmap is a List of Heartcycles. 
        // Every Heart has a heartmap
        // A Heartcycle describes:
        // - The speed of the cycle
        // - The decisions (forks) the path takes
        //   Decisions are simply the next connection index (0-6)
        // The Heartcycles will be collected and calculated within this function.

        // First we have to create a graph of all cycles. To do so we create a graph of the whole
        // Organism and then cut away all dead ends.
        // Create a graph of the whole organism
        GameObject[] cells = GameObject.FindGameObjectsWithTag("Cell");
        int[][] connectionGraph = new int[cells.Length][];

        // Every cell gets a temporary ID 
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].GetComponent<CellManager>().tempid = i;
        }

        // Create a graph of the whole organism
        for (int i = 0; i < cells.Length; i++)
        {
            // The first six elements are the connections of this cell.
            // Every connection is the tempID of the connected cell
            connectionGraph[i] = new int[8];
            connectionGraph[i][6] = 1;          // Sets this cell to active, we will use that later
            connectionGraph[i][7] = 0;          // This a mark we use later for pathfinding

            // Go through the connections
            for (int j = 0; j < 6; j++)
            {
                // Connection?
                CellManager cm = cells[i].GetComponent<CellManager>();
                if (cm.connections[j] != null)
                {
                    // Yes, tempid
                    connectionGraph[i][j] = cm.connections[j].tempid;
                }
                else
                {
                    // No, -1
                    connectionGraph[i][j] = -1;
                }
            }
        }

        // Now we have a graph of the organism. Next:
        // Cut away the dead ends
        // Not really necessary but maybe useful to safe some time. Because fractals
        for (int i = 0; i < cells.Length; i++)
        {
            // Algorithm:
            // A deadend is a cell with one or less connections.
            // When we find a deadend, we set it non-active and delete its connection for both sides.
            // We will ignore this cells later on
            // Because a deadend removed can often lead to the creation of another deadend,
            // we can use this opportunity to save time and deal with it right now.
            // Therefore we use this while loop and the workindex. With this we can repeat the same 
            // process for the cell that was connected to or previous cell and so forth.
            // Its kinda complicated but it makes it easy to get rid of all deadends at one time

            int workindex = i;      // The cell we are working on next
            bool done = false;
            while (!done)
            {
                int counter = 0;        // The number of connections this cell has
                int conindex = -1;      // The last found connection of this cell

                // Find an dead end: Count connections
                for (int j = 0; j < 6; j++)
                {
                    if (connectionGraph[workindex][j] != -1)
                    {
                        counter++;
                        conindex = j;
                    }
                }

                // Dead end?
                if (counter == 1)
                {
                    // Dead end! Remove from list
                    connectionGraph[workindex][6] = 0;
                    if (conindex != -1)
                    {
                        // Remove me from the connections of my connection lol
                        // In short words it means: "You dont know me anymore"
                        connectionGraph[connectionGraph[workindex][conindex]][(conindex + 3) % 6] = -1;

                        // New deadend?
                        workindex = connectionGraph[workindex][conindex];

                        // I dont know you anymore
                        connectionGraph[workindex][conindex] = -1;
                    }
                }
                else
                {
                    // I am done
                    done = true;
                }
            }
        }

        // Check if I still have both connection
        // Otherwise there is no cycle for me
        int c = 0;
        for (int i = 0; i < 6; i++)
        {
            if(connectionGraph[CellManager.tempid][i] != -1)
            {
                c++;
            }
        }

        if(c != 2)
        {
            heartmap = null;
            return;
        }

        // Now we create our Heartmap.
        // the pathfinder will find a number of cycles. These cycles will be added to the Heartmap

        // So how do we find the cycles?
        // First every cell has to know its connection count. We put the connection count into graph[i][6]
        int startindex = -1;
        int endindex = -1;
        for (int i = 0; i < connectionGraph.Length; i++)
        {
            // count connections
            int counter2 = 0;
            for (int j = 0; j < 6; j++)
            {
                if (connectionGraph[i][j] != -1)
                {
                    counter2++;
                }
            }

            // Put connection count in there
            if (connectionGraph[i][6] != 0)
            {
                connectionGraph[i][6] = counter2;
            }

            // Remove my own cell
            if (CellManager.tempid == i)
            {
                // I found myself! Im outa here
                connectionGraph[i][6] = 0;

                // Mark start and end of the cycles
                // Start
                startindex = connectionGraph[i][pumpdirection];
                connectionGraph[startindex][7] = -1;

                // End
                endindex = connectionGraph[i][(pumpdirection + 3) % 6];
                connectionGraph[endindex][7] = -2;
            }
        }

        // The output cell of the heart doesnt know me anymore
        connectionGraph[connectionGraph[CellManager.tempid][pumpdirection]][(pumpdirection + 3) % 6] = -1;
        connectionGraph[connectionGraph[CellManager.tempid][pumpdirection]][6]--;
        connectionGraph[connectionGraph[CellManager.tempid][(pumpdirection + 3) % 6]][pumpdirection] = -1;
        connectionGraph[connectionGraph[CellManager.tempid][(pumpdirection + 3) % 6]][6]--;

        //Reset heartmap
        heartmap = null;

        // Next we start the pathfinder:
        if (CyclePathFinder(connectionGraph, startindex, 1))
        {
            if(cyclespeed.Count != heartmap.Count)
            {
                // There is something wrong. Awfully wrong
                // Terribly wrong
                // Disasterous
                // Fucking ugly fucked up
                // What to do? Just blow it all up!
                heartmap = null;
                return;
            }

            // Heart is working, heartmap is generated.
            // Important: The path starts at the last decision. 
            // We have to change its order
            for(int i = 0; i < heartmap.Count; i++)
            {
                heartmap[i].Reverse();
            }

            // Setup cycle timers
            cycletimers = new List<float>();

            // Now with the paths in the right order, we will calculate the cyclespeeds
            for(int i = 0; i < heartmap.Count; i++)
            {
                cycletimers.Add(0);
                cyclespeed[i] *= heartspeed / (resistanceUnit * (float)(heartmap[i].Count) + 2);
            }

            // Done!
        }
        else
        {
            heartmap = null;
            // Heart is not working, empty heartmap
        }
    }

    // Recursive function to find cycles
    bool CyclePathFinder(int[][] conGraph, int startindex, int iterationdepth)
    {
        if (heartmap == null)
        {
            // Create a new heartmap
            heartmap = new List<List<int>>();
            cyclespeed = new List<float>();
        }

        // Reached the end? 
        if (conGraph[startindex][7] == -2)
        {
            // Memorize path
            List<int> decisions = new List<int>();
            heartmap.Add(decisions);
            cyclespeed.Add(1.0f);
            return true;
        }

        // Diggin too deep?
        if(iterationdepth > 500)
        {
            Debug.Log("Maximum of iteration depth reached.");
            return false;
        }

        // Already part of the path?
        if (conGraph[startindex][7] > 0)
        {
            return false;
        }

        // Did we find any solutions yet? 
        bool solutions = false;

        // Now I am already part of the path
        conGraph[startindex][7] = 1;

        // We have to count the successful decisions because we need that value later
        int solcounter = 0;

        // Recursive fork part
        for (int i = 0; i < 6; i++)
        {
            if (conGraph[startindex][i] != -1)
            {
                // Connection! Lets check it out
                // Make a backup of the heartmap and clear it
                List<List<int>> hmbuf = heartmap;
                List<float> csbuf = cyclespeed;
                heartmap = null;
                cyclespeed = null;
               
                // Recursive: returns true if it finds at least one solution.
                // heartmap then contains all solutions path from here
                if (CyclePathFinder(conGraph, conGraph[startindex][i], iterationdepth + 1))
                {
                    // This path is working!
                    // We have to add this decision to all new found solution paths
                    for (int j = 0; j < heartmap.Count; j++)
                    {
                        heartmap[j].Add(i);
                    }

                    // And merge them with the other solutions this call has found
                    for (int j = 0; j < hmbuf.Count; j++)
                    {
                        heartmap.Add(hmbuf[j]);
                        cyclespeed.Add(csbuf[j]);
                    }

                    // Weve got some solutions!
                    solutions = true;
                    solcounter++;
                }
                else
                {
                    // No solution in this direction. heartmap can be set back to the 
                    // other solutions this call has found
                    heartmap = hmbuf;
                    cyclespeed = csbuf;
                }
            }
        }

        // All found paths 
        if (solutions)
        {
            // The speed is getting shared by the pathes
            for (int j = 0; j < cyclespeed.Count; j++)
            {
                cyclespeed[j] /= (float)solcounter;
            }
        }

        // I am no longer part of this path
        conGraph[startindex][7] = 0;

        // Return if we found any solutions
        return solutions;
    }

    // EventHandler
    public void EventHandler(GUI_Event e, GUIManager gm)
    {
        switch (e)
        {
            case GUI_Event.OpenMenu:
                gm.AddSliderButton(GUI_Event.BtnHeartMenu);
                gm.AddSliderButton(GUI_Event.BtnDegenerate);
                break;
            case GUI_Event.BtnHeartMenu:
                // Open heart menu
                break;
        }
    }

}
