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

        CellManager.ConnectionMax = 6;
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
            case GUI_Event.BtnMorph2Heart:
                break;
            case GUI_Event.BtnMorph2Leaf:
                break;
            case GUI_Event.BtnMorph2Storage:
                break;
            case GUI_Event.BtnMorph2Worker:
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
