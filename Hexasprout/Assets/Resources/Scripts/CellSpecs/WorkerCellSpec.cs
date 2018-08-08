using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerCellSpec : MonoBehaviour {

    private float miningFactorBlue = 5;
    private float miningFactorYellow = 5;
    private float miningFactorBlack = 5;
    private float miningFactorGreen = 5;
    private float miningFactorRed = 5;

    int blocked;

    GameObject cell;

    GameObject[] materialNeighbours;

    

    void MakeConnection()
    {

	}

    public void BlockConnection()
    {

	}

    // EventHandler
    public void EventHandler(GUI_Event e, GUIManager gm)
    {
        
    }

    // getters and setters for the field
    public float MiningFactorBlue
    {
        get
        {
            return miningFactorBlue;
        }

        set
        {
            miningFactorBlue = value;
        }
    }

    public float MiningFactorYellow
    {
        get
        {
            return miningFactorYellow;
        }

        set
        {
            miningFactorYellow = value;
        }
    }

    public float MiningFactorBlack
    {
        get
        {
            return miningFactorBlack;
        }

        set
        {
            miningFactorBlack = value;
        }
    }

    public float MiningFactorGreen
    {
        get
        {
            return miningFactorGreen;
        }

        set
        {
            miningFactorGreen = value;
        }
    }

    public float MiningFactorRed
    {
        get
        {
            return miningFactorRed;
        }

        set
        {
            miningFactorRed = value;
        }
    }
}
