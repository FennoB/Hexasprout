using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerCellSpec : MonoBehaviour {

    private float miningFactorBlue = 5;
    private float miningFactorYellow = 5;
    private float miningFactorBlack = 5;
    private float miningFactorGreen = 5;
    private float miningFactorRed = 5;

    private int maxWorkerConnections = 1; 
    
    public MaterialManager[] materialNeighbours;
    public FieldManager Field;
    public CellManager CellManager;

    private void Awake()
    {
        materialNeighbours = new MaterialManager[6];
        CellManager = this.gameObject.GetComponentInParent<CellManager>();

        CellManager.animConnectionUp = this.gameObject.transform.GetChild(2).GetChild(0).GetComponent<Animator>();
        CellManager.animConnectionDown = this.gameObject.transform.GetChild(2).GetChild(1).GetComponent<Animator>();
        CellManager.animConnectionLeftDown = this.gameObject.transform.GetChild(2).GetChild(2).GetChild(0).GetComponent<Animator>();
        CellManager.animConnectionLeftUp = this.gameObject.transform.GetChild(2).GetChild(3).GetChild(0).GetComponent<Animator>();
        CellManager.animConnectionRightDown = this.gameObject.transform.GetChild(2).GetChild(4).GetChild(0).GetComponent<Animator>();
        CellManager.animConnectionRightUp = this.gameObject.transform.GetChild(2).GetChild(5).GetChild(0).GetComponent<Animator>();
    }

    private void Start()
    {
        Field = this.gameObject.transform.parent.GetComponentInParent<FieldManager>();
    }

    // EventHandler
    public void EventHandler(GUI_Event e, GUIManager gm)
    {
        switch (e)
        {
            case GUI_Event.Grow:
                BuildWorkerConnection();
                break;
        }
    }

    void BuildWorkerConnection()
    {
        
        if (GetCountWorkingConnections() < MaxWorkerConnections)
        {
            for (int i = 0; i < Field.neighbours.Length; i++)
            {
                if (Field.neighbours[i] != null)
                {
                    if (Field.neighbours[i].State == FieldState.SuperSelected)
                    {
                        if (Field.neighbours[i].HasMaterial())
                        {
                            Debug.Log(i);
                            MaterialNeighbours[i] = Field.neighbours[i].Material.GetComponent<MaterialManager>();
                            CellManager.BuildVisualConnection(i);
                        }
                    }
                }
            }
        }

    }

    int GetCountWorkingConnections()
    {
        int count = 0;
        for (int i = 0; i < MaterialNeighbours.Length; i++)
        {
            if (MaterialNeighbours[i] != null)
            {
                count++;
            }
        }
        return count;
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

    public int MaxWorkerConnections
    {
        get
        {
            return MaxWorkerConnections1;
        }

        set
        {
            MaxWorkerConnections1 = value;
        }
    }

    public MaterialManager[] MaterialNeighbours
    {
        get
        {
            return materialNeighbours;
        }

        set
        {
            materialNeighbours = value;
        }
    }

    public int MaxWorkerConnections1
    {
        get
        {
            return maxWorkerConnections;
        }

        set
        {
            maxWorkerConnections = value;
        }
    }
}
