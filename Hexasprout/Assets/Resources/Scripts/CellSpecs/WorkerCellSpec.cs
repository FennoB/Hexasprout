﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerCellSpec : MonoBehaviour {

    float miningFactorRed = 5;
    float miningFactorBlue = 5;
    float miningFactorYellow = 5;
    float miningFactorBlack = 5;
    float miningFactorGreen = 5;

    int blocked;

    GameObject cell;

    GameObject[] materialNeighbours;

    void MakeConnection()
    {

    }

    public void BlockConnection()
    {

    }
    public void EventHandler(GUI_Event e, GUIManager gm)
    { }
}
