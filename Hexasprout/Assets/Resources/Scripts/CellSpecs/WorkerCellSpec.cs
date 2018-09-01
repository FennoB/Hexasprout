using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerCellSpec : MonoBehaviour {

    public float miningFactorBlue = 0.01f;
    public float miningFactorYellow = 0.01f;
    public float miningFactorBlack = 0.01f;
    public float miningFactorGreen = 0.01f;
    public float miningFactorRed = 0.01f;

    public int maxWorkerConnections = 1;
    public int counterWorkerConnections = 0;

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
        CellManager.ConnectionMax = 1;
    }

    private void Start()
    {
        Field = transform.parent.GetComponentInParent<FieldManager>();
    }

    // EventHandler
    public void EventHandler(GUI_Event e, GUIManager gm)
    {
        switch (e)
        {
            case GUI_Event.Grow:
                BuildWorkerConnection();
                break;
            case GUI_Event.Decompose:
                DecomposeWorkerConnection();
                break;
            case GUI_Event.OpenMenu:
                gm.AddSliderButton(GUI_Event.BtnDegenerate);
                break;
            case GUI_Event.BtnDegenerate:
                break;
            case GUI_Event.BtnSpecialize:
                SpecializeMenu(gm);
                break;
            case GUI_Event.WorkerCount:
                UnlockWorkerConnections(gm);
                break;
            case GUI_Event.WorkerSpeedBlack:
                gm.CloseCellMenu();
                CellManager.loadBarPicture = GUI_Event.BtnWorkerSpeedBlack;
                gm.OpenLoadBar(GUI_Event.BtnWorkerSpeedBlack);

                JobCache jobCache = gm.transform.GetChild(6).gameObject.GetComponent<JobCache>();
                //MiningFactorBlack += jobCache.dif;
                CellManager.BuildManager.Build(jobCache.seconds, jobCache.juice, jobCache.title);
                break;

            case GUI_Event.WorkerSpeedBlue:
                gm.CloseCellMenu();
                CellManager.loadBarPicture = GUI_Event.BtnWorkerSpeedBlue;
                gm.OpenLoadBar(GUI_Event.BtnWorkerSpeedBlue);
                jobCache = gm.transform.GetChild(6).gameObject.GetComponent<JobCache>();
                //MiningFactorBlue += jobCache.dif;
                CellManager.BuildManager.Build(jobCache.seconds, jobCache.juice, jobCache.title);
                break;

            case GUI_Event.WorkerSpeedGreen:
                gm.CloseCellMenu();
                CellManager.loadBarPicture = GUI_Event.BtnWorkerSpeedGreen;
                gm.OpenLoadBar(GUI_Event.BtnWorkerSpeedGreen);

                jobCache = gm.transform.GetChild(6).gameObject.GetComponent<JobCache>();
                //MiningFactorGreen += jobCache.dif;
                CellManager.BuildManager.Build(jobCache.seconds, jobCache.juice, jobCache.title);
                break;

            case GUI_Event.WorkerSpeedRed:
                gm.CloseCellMenu();
                CellManager.loadBarPicture = GUI_Event.BtnWorkerSpeedRed;
                gm.OpenLoadBar(GUI_Event.BtnWorkerSpeedRed);

                jobCache = gm.transform.GetChild(6).gameObject.GetComponent<JobCache>();
                //MiningFactorRed += jobCache.dif;
                CellManager.BuildManager.Build(jobCache.seconds, jobCache.juice, jobCache.title);
                break;

            case GUI_Event.WorkerSpeedYellow:
                gm.CloseCellMenu();
                CellManager.loadBarPicture = GUI_Event.BtnWorkerSpeedYellow;
                gm.OpenLoadBar(GUI_Event.BtnWorkerSpeedYellow);

                jobCache = gm.transform.GetChild(6).gameObject.GetComponent<JobCache>();
                //MiningFactorYellow += jobCache.dif;
                CellManager.BuildManager.Build(jobCache.seconds, jobCache.juice, jobCache.title);
                break;
            case GUI_Event.BuildReady:
                FinishBuild(CellManager.BuildManager.buildName);
                break;
        }
    }

    void FinishBuild(string buildName)
    {
        float dif = GameObject.FindGameObjectWithTag("GUI").transform.GetChild(6).gameObject.GetComponent<JobCache>().dif;
        switch (buildName)
        {
            case "Unlock WorkerConnections":
                maxWorkerConnections = 6;
                break;
            case "Mine Blue faster":
                miningFactorBlue += dif;
                break;
            case "Mine Green faster":
                miningFactorGreen += dif;
                break;
            case "Mine Yellow faster":
                miningFactorYellow += dif;
                break;
            case "Mine Black faster":
                miningFactorBlack += dif;
                break;
            case "Mine Red faster":
                miningFactorRed += dif;
                break;
        }
    }

    void UnlockWorkerConnections(GUIManager gm)
    {
        gm.CloseCellMenu();
        CellManager.loadBarPicture = GUI_Event.BtnWorkerCount;
        gm.OpenLoadBar(GUI_Event.BtnWorkerCount);

        JobCache jobCache = gm.transform.GetChild(6).gameObject.GetComponent<JobCache>();
        MaxWorkerConnections = (int)jobCache.dif;
        CellManager.BuildManager.Build(jobCache.seconds, jobCache.juice, jobCache.title);
    }

    public void SpecializeMenu(GUIManager gm)
    {
        gm.AddSliderButton(GUI_Event.BtnWorkerCount);
        gm.AddSliderButton(GUI_Event.BtnWorkerSpeedBlack);
        gm.AddSliderButton(GUI_Event.BtnWorkerSpeedBlue);
        gm.AddSliderButton(GUI_Event.BtnWorkerSpeedRed);
        gm.AddSliderButton(GUI_Event.BtnWorkerSpeedGreen);
        gm.AddSliderButton(GUI_Event.BtnWorkerSpeedYellow);
    }

    public void OwnFixedUpdate()
    {
        Juice = CellManager.juice;
        for (int i = 0; i < materialNeighbours.Length; i++)
        {
            if (materialNeighbours[i] != null)
            {
                MaterialManager.Type type = materialNeighbours[i].type;
                float miningFactor = 0.0f;
                Juice change = new Juice();

                switch (type)
                {
                    case MaterialManager.Type.black:
                        miningFactor = MiningFactorBlack;
                        change.black = 1;
                        break;
                    case MaterialManager.Type.blue:
                        miningFactor = MiningFactorBlue;
                        change.blue = 1;
                        break;
                    case MaterialManager.Type.green:
                        miningFactor = MiningFactorGreen;
                        change.green = 1;
                        break;
                    case MaterialManager.Type.red:
                        miningFactor = MiningFactorRed;
                        change.red = 1;
                        break;
                    case MaterialManager.Type.yellow:
                        miningFactor = MiningFactorYellow;
                        change.yellow = 1;
                        break;
                    case MaterialManager.Type.bluecharged:
                        miningFactor = miningFactorBlue;
                        change.blueCharged = 1;
                        break;
                }

                if (1 - Juice.Sum > miningFactor * Time.deltaTime)
                {
                    Juice += change * materialNeighbours[i].Take(miningFactor * Time.deltaTime);
                }

                CellManager.juice = Juice;
            }
        }
    }

    public bool ConMaxReached()
    {
        return counterWorkerConnections >= maxWorkerConnections;
    }

    void DecomposeWorkerConnection()
    {
        for (int i = 0; i < Field.neighbours.Length; i++)
        {
            if (Field.neighbours[i] != null && Field.neighbours[i].HasMaterial())
            {
                if (Field.neighbours[i].State == FieldState.SuperSelected)
                {
                    counterWorkerConnections--;
                    MaterialNeighbours[i] = null;
                    Destroy(Field.neighbours[i].transform.GetChild(1).gameObject);
                    transform.GetChild(2).GetChild(i).gameObject.SetActive(false);
                    Field.neighbours[i].baseBrightness = 0f;
                }
            }
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
                            MaterialNeighbours[i] = Field.neighbours[i].Material.GetComponent<MaterialManager>();
                            counterWorkerConnections++;
                            GameObject p = (GameObject)Resources.Load("Prefabs/Cells/workerhelper", typeof(GameObject));
                            GameObject g = Instantiate(p);
                            transform.GetChild(2).GetChild(i).gameObject.SetActive(true);
                            g.transform.SetParent(Field.neighbours[i].transform);
                            g.transform.localPosition = new Vector3(0, 0, -0.2f);
                            Field.neighbours[i].baseBrightness = 1f;
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
    public int MaxWorkerConnections
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
