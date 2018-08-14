using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StemCellSpec : MonoBehaviour {

    public FieldManager Field;
    public CellManager CellManager;

    private Animator[] animator;
   
    private void Awake()
    {
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
                BuildConnection();
                break;
        }
    }

    void BuildConnection()
    {
        for (int i = 0; i < Field.neighbours.Length; i++)
        {
            if (Field.neighbours[i] != null)
            {
                if (Field.neighbours[i].State == FieldState.SuperSelected && !Field.neighbours[i].HasMaterial())
                {                
                    if (Field.neighbours[i].Cell == null)
                    {
                        GameObject.Find("World").GetComponent<WorldGenerator>().CreateStemCell(Field.neighbours[i]);
                    }
                    Field.Cell.GetComponent<CellManager>().ConnectWith(Field.neighbours[i].Cell.GetComponent<CellManager>(), i);
                    animator[i].SetTrigger("Start");
                }
            }
        }
    }
    
    // Update is called once per frame
    public void OwnFixedUpdate()
    {

    }
}
