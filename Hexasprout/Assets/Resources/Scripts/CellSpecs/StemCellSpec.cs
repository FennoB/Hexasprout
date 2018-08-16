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
        BuildManager = this.gameObject.GetComponentInParent<BuildManager>();
        CellManager = this.gameObject.GetComponentInParent<CellManager>();
        animator = new Animator[6];

        for (int i = 0; i < 6; i++)
        {
            animator[i] = transform.GetChild(2).GetChild(i).gameObject.GetComponent<Animator>();
        }
}

    private void Start()
    {
        Field = this.gameObject.transform.parent.GetComponentInParent<FieldManager>();
    }

    public void ActivateOptionScript()
    {
        this.gameObject.transform.GetChild(3).GetComponent<Canvas>().enabled = true;
    }
    public void DeactivateOptionScript()
    {
        this.gameObject.transform.GetChild(3).GetComponent<Canvas>().enabled = false;
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
        }
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
            case "Build Cell":
                MakeCell();
                break;
            case "Build Connection":
                MakeConnection();
                break;
        }
    }
    //Here are the buildmethods, for one build are always a order and a make method necessary

    void OrderNewCell()
    {
        buildName = "Build Cell";
        //first parameter is time in seconds, second the required juice, third the Name of the Buildevent
        this.gameObject.GetComponent<BuildManager>().Build(10, new Juice(0f,0f,0f,0f,0f,1f), buildName);
    }

    void OrderNewConnection()
    {
        buildName = "Build Connection";
        this.gameObject.GetComponent<BuildManager>().Build(10, new Juice(0f, 0f, 0f, 0f, 0f, 0.2f), buildName);
    }

    void MakeCell()
    {
        GameObject.Find("World").GetComponent<WorldGenerator>().CreateStemCell(buildTarget);
        MakeConnection();
    }

    void MakeConnection()
    {
        Field.Cell.GetComponent<CellManager>().ConnectWith(buildTarget.Cell.GetComponent<CellManager>(), positionOfBuildTarget);
        animator[positionOfBuildTarget].SetTrigger("Start");

        buildTarget = null;
        positionOfBuildTarget = 0;
    }
    public void OwnFixedUpdate(){}
}
