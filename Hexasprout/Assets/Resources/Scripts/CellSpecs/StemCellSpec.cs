using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StemCellSpec : MonoBehaviour {

    public FieldManager Field;
    public CellManager CellManager;
   
    private void Awake()
    {
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
                    CellManager.BuildVisualConnection(i);
                }
            }
        }
    }
    
    // Update is called once per frame
    public void OwnFixedUpdate()
    {

    }
}
