using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StemCellSpec : MonoBehaviour {

    public FieldManager Field;
    public CellManager CellManager;

    public FieldManager buildTarget;
    public string buildName;
    public int positionOfBuildTarget;
    public BuildManager BuildManager;

    private Animator[] animator;
   
    private void Awake()
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

    // EventHandler
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
                gm.AddSliderButton(GUI_Event.BtnTransmorph);
                break;
            case GUI_Event.BtnTransmorph:
                gm.ResetSliderButtons();
                if (CellManager.ConnectionCounter <= 2)
                {
                    gm.AddSliderButton(GUI_Event.BtnMorph2Heart);
                }
                if (CellManager.ConnectionCounter <= 1)
                {
                    gm.AddSliderButton(GUI_Event.BtnMorph2Leaf);
                    gm.AddSliderButton(GUI_Event.BtnMorph2Worker);
                }
                gm.AddSliderButton(GUI_Event.BtnMorph2Storage);
                gm.AddSliderButton(GUI_Event.BtnNavMain);
                break;
            case GUI_Event.Morph2Heart:
                gm.CloseCellMenu();
                gm.OpenLoadBar(GUI_Event.BtnMorph2Heart);
                OrderHeartMorph(gm);
                break;
            case GUI_Event.Morph2Leaf:
                gm.CloseCellMenu();
                gm.OpenLoadBar(GUI_Event.BtnMorph2Leaf);
                OrderLeafMorph(gm);
                break;
            case GUI_Event.Morph2Storage:
                gm.CloseCellMenu();
                gm.OpenLoadBar(GUI_Event.BtnMorph2Storage);
                OrderStorageMorph(gm);
                break;
            case GUI_Event.Morph2Worker:
                gm.CloseCellMenu();
                gm.OpenLoadBar(GUI_Event.BtnMorph2Worker);
                OrderWorkerMorph(gm);
                break;
        }
    }

    void Morph2Heart()
    {
        GameObject.Find("World").GetComponent<WorldGenerator>().Morph2Heart(this.gameObject.GetComponent<CellManager>());
    }

    void Morph2Leaf()
    {
        GameObject.Find("World").GetComponent<WorldGenerator>().Morph2(this.gameObject.GetComponent<CellManager>(), "LeafCell");
    }

    void Morph2Storage()
    {
        GameObject.Find("World").GetComponent<WorldGenerator>().Morph2(this.gameObject.GetComponent<CellManager>(), "StorageCell");
    }

    void Morph2Worker()
    {
        GameObject.Find("World").GetComponent<WorldGenerator>().Morph2(this.gameObject.GetComponent<CellManager>(), "WorkerCell");
    }

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
        switch(type)
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
            case "Morph to Heartcell":
                Morph2Heart();
                break;
            case "Morph to Leafcell":
                Morph2Leaf();
                break;
            case "Morph to Storagecell":
                Morph2Storage();
                break;
            case "Morph to Workercell":
                Morph2Worker();
                break;
        }
    }
    //Here are the buildmethods, for one build are always a order and a make method necessary

    void OrderHeartMorph(GUIManager gm)
    {
        CellManager.loadBarPicture = GUI_Event.BtnMorph2Heart;
        JobCache jobCache = gm.transform.GetChild(8).gameObject.GetComponent<JobCache>();
        buildName = jobCache.title;
        BuildManager.Build(jobCache.seconds, jobCache.juice, jobCache.title);
    }
    void OrderLeafMorph(GUIManager gm)
    {
        CellManager.loadBarPicture = GUI_Event.BtnMorph2Leaf;
        JobCache jobCache = gm.transform.GetChild(8).gameObject.GetComponent<JobCache>();
        buildName = jobCache.title;
        BuildManager.Build(jobCache.seconds, jobCache.juice, jobCache.title);
    }
    void OrderStorageMorph(GUIManager gm)
    {
        CellManager.loadBarPicture = GUI_Event.BtnMorph2Storage;
        JobCache jobCache = gm.transform.GetChild(8).gameObject.GetComponent<JobCache>();
        buildName = jobCache.title;
        BuildManager.Build(jobCache.seconds, jobCache.juice, jobCache.title);
    }
    void OrderWorkerMorph(GUIManager gm)
    {
        CellManager.loadBarPicture = GUI_Event.BtnMorph2Worker;
        JobCache jobCache = gm.transform.GetChild(8).gameObject.GetComponent<JobCache>();
        buildName = jobCache.title;
        BuildManager.Build(jobCache.seconds, jobCache.juice, jobCache.title);
    }

    // Builds a sprout and creates a new stemcell there
    void OrderNewCell()
    {
        // Sprout
        buildName = "Build Sprout";
        this.gameObject.GetComponent<BuildManager>().Build(seconds: 1f, juice: new Juice(0f, 0f, 0f, 0f, 0f, 0.25f), name: buildName);
        CellManager.loadBarPicture = GUI_Event.BtnDegenerate;
    }

    void OrderNewConnection()
    {
        buildName = "Make Connection";
        this.gameObject.GetComponent<BuildManager>().Build(2f, new Juice(0f, 0f, 0f, 0f, 0f, 0.2f), buildName);
        CellManager.loadBarPicture = GUI_Event.BtnDegenerate;
    }

    void MakeCell()
    {
        buildName = "Build Cell";

        //first parameter is time in seconds, second the required juice, third the Name of the Buildevent
        this.gameObject.GetComponent<BuildManager>().Build(2, new Juice(0f, 0f, 0f, 0f, 0f, 0.2f), buildName);
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

    public void OwnFixedUpdate()
    {
        // Stemcell shows the connections
        for (int i = 0; i < 6; i++)
        {
            if(CellManager.connections[i] != null && CellManager.connections[i].cellType != CellType.Stemcell && CellManager.connections[i].cellType != CellType.Storagecell)
            {
                // Show connection
                Animator a = animator[positionOfBuildTarget];
                a.Play("AnimationPipe" + i, 0, 1.0f);
            }
        }

        if (BuildManager.buildFlag)
        {
            if (buildName == "Build Sprout" || buildName == "Make Connection")
            {
                // Animation of build process
                Animator a = animator[positionOfBuildTarget];
                a.Play("AnimationPipe" + positionOfBuildTarget, 0, BuildManager.progress / 1.4f);
            }
            else if(buildName == "Build Cell")
            {
                float p = 0.5f + ((int)(BuildManager.progress * 5.0f) / 5.0f) / 1.5f;
                buildTarget.Cell.transform.localScale = new Vector3(p, p, p);
            }
        }
    }
}
