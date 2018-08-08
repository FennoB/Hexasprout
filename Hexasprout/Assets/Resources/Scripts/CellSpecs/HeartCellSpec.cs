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
    public int pumpdirection;
    public float resistanceUnit;

    // Use this for initialization
    void Awake()
    {
        heartmap = null;
        cyclespeed = null;
        cycletimers = new List<float>();
        heartspeed = 1.0f;
        pumpdirection = 0;
        resistanceUnit = 0.2f;
        UpdateHeartmap();
    }

    // Fixed Update
    public void OwnFixedUpdate()
    {
        for (int i = 0; i < cycletimers.Count; i++)
        {
            cycletimers[i] += Time.deltaTime;
        }

        if (heartmap != null && cyclespeed != null)
        {
            Pump();
        }
        else
        {
            UpdateHeartmap();
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

                // Go through the heartmap
                for (int j = 0; j < heartmap[i].Count; j++)
                {
                    // Get the next exchange partner
                    CellManager cm3 = cm2.connections[heartmap[i][j]].GetComponent<CellManager>();

                    // Another buffer
                    Juice lbuf = cm3.juice;

                    // Exchange
                    cm3.juice = buf;
                    buf = lbuf;
                }

                // Put the rest back together
                cm2.juice = cm.juice;
                cm2 = cm.connections[(pumpdirection + 3) % 6].GetComponent<CellManager>();
                cm.juice = cm2.juice;
                cm2.juice = buf;
            }
        }
    }

    // Recreate the Heartmap
    void UpdateHeartmap()
    {
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

        // The Heartcycles will be collected and calculated within this function.

        // First we have to create a graph of all cycles. To do so we create a graph of the whole
        // Organism and then cut away all dead ends.
        // Create a graph of the whole organism
        GameObject[] cells = GameObject.FindGameObjectsWithTag("Cell");
        int[][] connectionGraph = new int[cells.Length][];

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].GetComponent<CellManager>().tempid = i;
        }

        for (int i = 0; i < cells.Length; i++)
        {
            connectionGraph[i] = new int[8];
            connectionGraph[i][6] = 1;          // Sets this cell to active, we will use that later
            connectionGraph[i][7] = 0;          // This a mark we use later for pathfinding
            for (int j = 0; j < 6; j++)
            {
                CellManager cm = cells[i].GetComponent<CellManager>();
                if (cm.connections[i] != null)
                {
                    connectionGraph[i][j] = cm.tempid;
                }
                else
                {
                    connectionGraph[i][j] = -1;
                }
            }
        }

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

                // Find an dead end
                for (int j = 0; j < 6; j++)
                {
                    if (connectionGraph[workindex][j] != -1)
                    {
                        counter++;
                        conindex = j;
                    }
                }

                if (counter <= 1)
                {
                    // Deadend! Remove from list
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

            CellManager cm = GetComponent<CellManager>();
            if (cm.tempid == i)
            {
                // I found myself! Im outa here
                connectionGraph[i][6] = 0;
                connectionGraph[connectionGraph[i][pumpdirection]][(pumpdirection + 3) % 6] = -1;
                connectionGraph[connectionGraph[i][pumpdirection]][6]--;
                connectionGraph[connectionGraph[i][(pumpdirection + 3) % 6]][pumpdirection] = -1;
                connectionGraph[connectionGraph[i][(pumpdirection + 3) % 6]][6]--;

                // Mark start and end of the cycles
                // Start
                startindex = connectionGraph[i][pumpdirection];
                connectionGraph[startindex][7] = -1;

                // End
                endindex = connectionGraph[i][(pumpdirection + 3) % 6];
                connectionGraph[endindex][7] = -2;
            }
        }

        //Reset heartmap
        heartmap = null;

        // Next we start the pathfinder:
        if (CyclePathFinder(connectionGraph, startindex))
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
            // TODO: Some message to the user that he fucked up building a working heartcycle
        }
    }

    // Recursive function to find cycles
    bool CyclePathFinder(int[][] conGraph, int startindex)
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
                if (CyclePathFinder(conGraph, conGraph[startindex][i]))
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

    }

}
