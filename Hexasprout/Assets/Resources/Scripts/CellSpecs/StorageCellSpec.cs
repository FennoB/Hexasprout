using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageCellSpec : MonoBehaviour
{
    public Juice content;
    public Juice capacities;
    public Juice target;
    public float maxSpeedPerSecond = 0.1f;
    public FieldManager Field;

    public CellManager CellManager;

    public FieldManager buildTarget;
    public string buildName;
    public int positionOfBuildTarget;
    public BuildManager BuildManager;

    private Animator[] animator;

    // Use this for initialization
    void Awake ()
    {
        BuildManager = this.gameObject.GetComponent<BuildManager>();
        CellManager = this.gameObject.GetComponent<CellManager>();
        animator = new Animator[6];

        for (int i = 0; i < 6; i++)
        {
            animator[i] = transform.GetChild(2).GetChild(i).gameObject.GetComponent<Animator>();
        }

        CellManager.ConnectionMax = 6;
    }

    private void Start()
    {
        Field = transform.parent.GetComponent<FieldManager>();
    }

    // Update is called once per frame
    public void OwnFixedUpdate()
    {
        if (BuildManager.buildFlag)
        {
            if (buildName == "Build Sprout" || buildName == "Make Connection")
            {
                // Animation of build process
                Animator a = animator[positionOfBuildTarget];
                a.Play("AnimationPipe" + positionOfBuildTarget, 0, BuildManager.progress / 1.4f);
            }
            else if (buildName == "Build Cell")
            {
                float p = 0.5f + ((int)(BuildManager.progress * 5.0f) / 5.0f) / 1.5f;
                buildTarget.Cell.transform.localScale = new Vector3(p, p, p);
            }
        }

        StorageHandling();
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
        GetComponent<CellManager>().juice = juice;
    }

    public void EventHandler(GUI_Event e, GUIManager gm)
    {
        switch (e)
        {
            case GUI_Event.Grow:
                StartBuild();
                break;

            case GUI_Event.BuildReady:
                FinishBuild(buildName);
                break;

            case GUI_Event.OpenMenu:
                gm.AddSliderButton(GUI_Event.BtnStoreMenu);
                gm.AddSliderButton(GUI_Event.BtnDegenerate);
                break;
        }
    }
    

    //- COPIES FROM STEMCELLSPEC! -//
    void StartBuild()
    {
        for (int i = 0; i < Field.neighbours.Length; i++)
        {
            if (Field.neighbours[i] != null)
            {
                if (Field.neighbours[i].State == FieldState.SuperSelected && !Field.neighbours[i].HasMaterial())
                {
                    buildTarget = Field.neighbours[i];
                    positionOfBuildTarget = i;

                    if (Field.neighbours[i].Cell == null)
                    {
                        OrderNewCell();
                    }
                    else
                    {
                        OrderNewConnection();
                    }
                }
            }
        }
    }
    void FinishBuild(string type)
    {
        switch (type)
        {
            case "Build Sprout":
                MakeCell();
                break;
            case "Make Connection":
                MakeConnection();
                break;
            case "Build Cell":
                FinishCell();
                break;
        }
    }
    void OrderNewCell()
    {
        // Sprout
        buildName = "Build Sprout";
        this.gameObject.GetComponent<BuildManager>().Build(10, new Juice(0f, 0f, 0f, 0f, 0f, 0.02f), buildName);
        CellManager.loadBarPicture = GUI_Event.BtnDegenerate;
    }
    void OrderNewConnection()
    {
        buildName = "Make Connection";
        this.gameObject.GetComponent<BuildManager>().Build(10, new Juice(0f, 0f, 0f, 0f, 0f, 0.02f), buildName);
        CellManager.loadBarPicture = GUI_Event.BtnDegenerate;
    }
    void MakeCell()
    {
        buildName = "Build Cell";

        //first parameter is time in seconds, second the required juice, third the Name of the Buildevent
        this.gameObject.GetComponent<BuildManager>().Build(10, new Juice(0f, 0f, 0f, 0f, 0f, 0.02f), buildName);
        GameObject.Find("World").GetComponent<WorldGenerator>().CreateStemCell(buildTarget);

        GameObject p = (GameObject)Resources.Load("Prefabs/Connections/buildegg", typeof(GameObject));
        GameObject g = Instantiate(p);
        g.transform.SetParent(buildTarget.transform);
        g.transform.localPosition = new Vector3(0, 0, -0.2f);
        buildTarget.Cell.GetComponent<CellManager>().alive = false;
        buildTarget.Cell.transform.localScale = new Vector3(0, 0, 0);
    }
    void FinishCell()
    {
        buildTarget.Cell.transform.localScale = new Vector3(1, 1, 1);
        buildTarget.Cell.GetComponent<CellManager>().alive = true;
        Destroy(buildTarget.transform.GetChild(1).gameObject);
        buildName = "---";
        MakeConnection();
    }
    void MakeConnection()
    {
        Field.Cell.GetComponent<CellManager>().ConnectWith(buildTarget.Cell.GetComponent<CellManager>(), positionOfBuildTarget);
        buildName = "---";
        buildTarget = null;
        positionOfBuildTarget = 0;
    }
    //- END COPIES -//
}
