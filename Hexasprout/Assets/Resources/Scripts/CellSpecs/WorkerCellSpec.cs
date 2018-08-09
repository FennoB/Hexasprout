using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerCellSpec : MonoBehaviour {

    private float miningFactorBlue = 0.01f;
    private float miningFactorYellow = 0.01f;
    private float miningFactorBlack = 0.01f;
    private float miningFactorGreen = 0.01f;
    private float miningFactorRed = 0.01f;

    private int maxWorkerConnections = 1; 
    
    public MaterialManager[] materialNeighbours;
    public FieldManager Field;
    public CellManager CellManager;
    private Juice juice;
    public bool StartFlag = true;

    private void Awake()
    {
        materialNeighbours = new MaterialManager[6];
        CellManager = this.gameObject.GetComponentInParent<CellManager>();
        Juice = CellManager.juice;

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
    public void OwnFixedUpdate()
    {
        Juice = CellManager.juice;
        for (int i = 0; i < materialNeighbours.Length; i++)
        {
            if (materialNeighbours[i] != null)
            {
                MaterialManager.Type type = materialNeighbours[i].type;   
                if (!materialNeighbours[i].LoadEmptyAfterTake(GetRightMiningFactor(type)) && (1 - Juice.Sum > GetRightMiningFactor(type)))
                {
                    switch (materialNeighbours[i].type)
                    {
                        case MaterialManager.Type.black:
                            materialNeighbours[i].DecreaseLoad(miningFactorBlack);
                            Juice.black += miningFactorBlack * Time.deltaTime;
                            break;
                        case MaterialManager.Type.blue:
                            materialNeighbours[i].DecreaseLoad(miningFactorBlue);
                            Juice.blue += miningFactorBlue * Time.deltaTime;
                            break;
                        case MaterialManager.Type.green:
                            materialNeighbours[i].DecreaseLoad(miningFactorGreen);
                            Juice.green += miningFactorGreen * Time.deltaTime;
                            break;
                        case MaterialManager.Type.red:
                            materialNeighbours[i].DecreaseLoad(miningFactorRed);
                            Juice.red += miningFactorRed * Time.deltaTime;
                            break;
                        case MaterialManager.Type.yellow:
                            materialNeighbours[i].DecreaseLoad(miningFactorYellow);
                            Juice.yellow += miningFactorYellow * Time.deltaTime;
                            break;
                    }
                }
            }
        }
    }
    /*IEnumerator FarmMaterial()
    {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < materialNeighbours.Length; i++)
        {
            if (materialNeighbours[i] != null)
            {
                Debug.Log("Boing");
                MaterialManager.Type type = materialNeighbours[i].type;
                //Debug.Log(GetRightMiningFactor(type));
                Debug.Log(Juice.Sum);
                if (!materialNeighbours[i].LoadEmptyAfterTake(GetRightMiningFactor(type)) && (1 - Juice.Sum > GetRightMiningFactor(type)))
                {
                    Debug.Log("jo");
                    Debug.Log(materialNeighbours[i].type);
                    switch (materialNeighbours[i].type)
                    {
                        case MaterialManager.Type.black:
                            materialNeighbours[i].DecreaseLoad(miningFactorBlack);
                            Juice.black += miningFactorBlack;
                            break;
                        case MaterialManager.Type.blue:
                            materialNeighbours[i].DecreaseLoad(miningFactorBlue);
                            Juice.blue += miningFactorBlue;
                            break;
                        case MaterialManager.Type.green:
                            materialNeighbours[i].DecreaseLoad(miningFactorGreen);
                            Juice.green += miningFactorGreen;
                            break;
                        case MaterialManager.Type.red:
                            Debug.Log("im Red");
                            materialNeighbours[i].DecreaseLoad(miningFactorRed);
                            Juice.red += miningFactorRed;
                            break;
                        case MaterialManager.Type.yellow:
                            materialNeighbours[i].DecreaseLoad(miningFactorYellow);
                            Juice.yellow += miningFactorYellow;
                            break;
                    }
                }
            }
        }
        StartFlag = true;
    }*/
    public float GetRightMiningFactor(MaterialManager.Type type)
    {
        switch (type)
        {
            case MaterialManager.Type.black:
                return MiningFactorBlack;
            case MaterialManager.Type.blue:
                return MiningFactorBlue;
            case MaterialManager.Type.green:
                return MiningFactorGreen;
            case MaterialManager.Type.red:
                return MiningFactorRed;
            case MaterialManager.Type.yellow:
                return MiningFactorYellow;
        }
        return 0f;
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

    public Juice Juice
    {
        get
        {
            return juice;
        }

        set
        {
            juice = value;
        }
    }
}
