using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TouchManager : MonoBehaviour
{
    private Vector3 hit_position = Vector3.zero;
    private Vector3 current_position = Vector3.zero;
    private Vector3 camera_position = Vector3.zero;
    private GameObject selectedCell;
    private Touch touch;
    private int tapUpCounter = 0;
    private int tapCounter = 0;

    private int doubleClickCounter = 0;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            hit_position = Input.mousePosition;
            camera_position = transform.position;
        }


        if (Input.GetMouseButton(0))
        {
            current_position = Input.mousePosition;
            LeftMouseDrag();
        }

        TouchHandling();
    }

    void TouchHandling()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            //if (Input.GetMouseButtonDown(0))
            if (touch.phase == TouchPhase.Began)
            {
                tapUpCounter = 0;
                StartCoroutine("SingleClicked");
                //Debug.Log("Begin");
                //Debug.Log(tapUpCounter);
            }
            //if (Input.GetMouseButtonUp(0))
            if (touch.phase == TouchPhase.Ended)
            {
                //Debug.Log("End");
                tapUpCounter++;
                tapCounter++;
                if (tapCounter == 1)
                {

                    StartCoroutine("DoubleClicked");
                }
            }
        }
    }

    IEnumerator SingleClicked()
    {
        yield return new WaitForSeconds(0.1f);
        if (tapUpCounter == 1)
        {
            Debug.Log("Single Click");
            Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(point, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject.tag == "Field")
            {
                FieldManager fm = hit.collider.gameObject.GetComponent<FieldManager>();
                if (fm.GetCell() != null)
                {
                    fm.GetCell().GetComponent<StemCellSpec>().ActivateOptionScript();
                    selectedCell = fm.GetCell();
                }
                else
                {
                    if (selectedCell != null)
                    {
                        selectedCell.GetComponent<StemCellSpec>().DeactivateOptionScript();
                    }
                }
            }
        }
    }

    //Doubleclicked is in a Coroutine for handling the Clickevent over time
    IEnumerator DoubleClicked ()
    {
        yield return new WaitForSeconds(0.5f);
        if (tapCounter > 1)
        {
            //the field hit by the user
            Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(point, Vector2.zero);


            // if a field is hit by double click 
            if (hit.collider != null && hit.collider.gameObject.tag == "Field")
            {
                FieldManager fm = hit.collider.gameObject.GetComponent<FieldManager>();

                //first tap, select a cell
                if (fm.cell != null && !fm.GetNeighbourSelected() && doubleClickCounter == 0)
                {
                    //reseting old selected cells and set the new ones
                    ResetSelectStateOfCells(GameObject.Find("World").GetComponent<WorldGenerator>().GetFields());
                    fm.SetSelected(true);
                    for (int i = 0; i < 6; i++)
                    {
                        if (fm.GetNeighbours()[i] != null)
                        {
                            fm.GetNeighbours()[i].gameObject.GetComponent<FieldManager>().SetNeighbourSelected(true);
                        }
                    }
                    doubleClickCounter = 1;
                }

                //if selected field is neighbourselected, it means that player want to build new cell there
                if (fm.cell == null && fm.GetNeighbourSelected() && doubleClickCounter == 1)
                {
                    CreateCell(fm);
                    FieldManager selected = GetSelectedCell(fm);
                    MakeCellConnection(selected, fm);

                    ResetSelectStateOfCells(GameObject.Find("World").GetComponent<WorldGenerator>().GetFields());
                    doubleClickCounter = 0;
                }
                //means that player want to build connection between neighbourselected cell and selected cell
                if (fm.cell != null && fm.GetNeighbourSelected() && doubleClickCounter == 1)
                {
                    FieldManager selected = GetSelectedCell(fm);
                    MakeCellConnection(selected, fm);

                    ResetSelectStateOfCells(GameObject.Find("World").GetComponent<WorldGenerator>().GetFields());
                    doubleClickCounter = 0;
                }
            }
        }
        tapCounter = 0; 
    }
    //get selected cell if your're on an neighbourselected cell
    FieldManager GetSelectedCell(FieldManager fm)
    {
        FieldManager selected = null;
        for (int i = 0; i < 6; i++)
        {
            if (fm.GetNeighbours()[i] != null)
            {
                if (fm.GetNeighbours()[i].GetComponent<FieldManager>().GetSelected())
                {
                    selected = fm.GetNeighbours()[i].GetComponent<FieldManager>();
                    break;
                }
            }
        }
        return selected;
    }

    void CreateCell(FieldManager fm)
    {
        // Cell creation
        GameObject g = Instantiate((GameObject)Resources.Load("Prefabs/StemCell", typeof(GameObject)));
        g.GetComponent<Transform>().SetParent(fm.gameObject.GetComponent<Transform>());
        g.GetComponent<Transform>().localPosition = new Vector3(0, 0, -0.14f);
        fm.gameObject.GetComponent<FieldManager>().cell = g;
    }
    /**
     *This method makes a connection between two cells by only know the FieldManagers of the cells 
     * The Connection get established from second, the Animation made from first
     * 
     */
    public void MakeCellConnection(FieldManager first, FieldManager second)
    {
        //case connection to down left
        //the neighbourfield has another id, if the selected field is in an uneven row than it has in a even one
        if (first.GetIdy() % 2 == 0)
        {   
            if (first.GetIdx() - 1 == second.GetIdx() && first.GetIdy() - 1 == second.GetIdy())
            {   
                first.GetCell().GetComponent<CellManager>().ConnectWith(second.GetCell().GetComponent<CellManager>(), 0);
                first.GetCell().GetComponent<CellManager>().SetDownLeftAnimation();
            }
        }
        if (first.GetIdy() % 2 == 1)
        {
            if (first.GetIdx() == second.GetIdx() && first.GetIdy() - 1 == second.GetIdy())
            {
                first.GetCell().GetComponent<CellManager>().ConnectWith(second.GetCell().GetComponent<CellManager>(), 0);
                first.GetCell().GetComponent<CellManager>().SetDownLeftAnimation();
            }
        }
        //case connection down
        if (first.GetIdx() == second.GetIdx() && first.GetIdy() - 2 == second.GetIdy())
        {
            first.GetCell().GetComponent<CellManager>().ConnectWith(second.GetCell().GetComponent<CellManager>(), 1);
            first.GetCell().GetComponent<CellManager>().SetDownAnimation();
        }
        //case connection down right
        if (first.GetIdy() % 2 == 0)
        {
            if (first.GetIdx() == second.GetIdx() && first.GetIdy() - 1 == second.GetIdy())
            {
                first.GetCell().GetComponent<CellManager>().ConnectWith(second.GetCell().GetComponent<CellManager>(), 2);
                first.GetCell().GetComponent<CellManager>().SetDownRightAnimation();
            }
        }
        if (first.GetIdy() % 2 == 1)
        {
            if (first.GetIdx() + 1 == second.GetIdx() && first.GetIdy() - 1 == second.GetIdy())
            {
                first.GetCell().GetComponent<CellManager>().ConnectWith(second.GetCell().GetComponent<CellManager>(), 2);
                first.GetCell().GetComponent<CellManager>().SetDownRightAnimation();
            }
        }

        //case connection up right
        if (first.GetIdy() % 2 == 0)
        {
            if (first.GetIdx() == second.GetIdx() && first.GetIdy() + 1 == second.GetIdy())
            {
                first.GetCell().GetComponent<CellManager>().ConnectWith(second.GetCell().GetComponent<CellManager>(), 3);
                first.GetCell().GetComponent<CellManager>().SetUpRightAnimation();
            }
        }
        if (first.GetIdy() % 2 == 1)
        {
            if (first.GetIdx() + 1 == second.GetIdx() && first.GetIdy() + 1 == second.GetIdy())
            {
                first.GetCell().GetComponent<CellManager>().ConnectWith(second.GetCell().GetComponent<CellManager>(), 3);
                first.GetCell().GetComponent<CellManager>().SetUpRightAnimation();
            }
        }
        //case connection up
        if (first.GetIdx() == second.GetIdx() && first.GetIdy() + 2 == second.GetIdy())
        {
            first.GetCell().GetComponent<CellManager>().ConnectWith(second.GetCell().GetComponent<CellManager>(), 4);
            first.GetCell().GetComponent<CellManager>().SetUpAnimation();
        }
        //case connection up left
        if (first.GetIdy() % 2 == 0)
        {
            if (first.GetIdx() - 1 == second.GetIdx() && first.GetIdy() + 1 == second.GetIdy())
            {
                first.GetCell().GetComponent<CellManager>().ConnectWith(second.GetCell().GetComponent<CellManager>(), 5);
                first.GetCell().GetComponent<CellManager>().SetUpLeftAnimation();
            }
        }
        if (first.GetIdy() % 2 == 1)
        {
            if (first.GetIdx() == second.GetIdx() && first.GetIdy() + 1 == second.GetIdy())
            {
                first.GetCell().GetComponent<CellManager>().ConnectWith(second.GetCell().GetComponent<CellManager>(), 5);
                first.GetCell().GetComponent<CellManager>().SetUpLeftAnimation();
            }
        }
    }

    /*
     *In this method we are iterating throug all cells, in order to deselect all 
     * 
     */
    public void ResetSelectStateOfCells(GameObject[][] fields)
    {
        for (int i = 0; i < fields.Length; i++)
        {
            for (int j = 0; j < fields[i].Length; j++)
            {
                fields[i][j].GetComponent<FieldManager>().SetSelected(false);
                fields[i][j].GetComponent<FieldManager>().SetNeighbourSelected(false);
            }
        }
    }

    void LeftMouseDrag()
    {
        // From the Unity3D docs: "The z position is in world units from the camera."  In my case I'm using the y-axis as height
        // with my camera facing back down the y-axis.  You can ignore this when the camera is orthograhic.
        current_position.z = hit_position.z = camera_position.y;

        // Get direction of movement.  (Note: Don't normalize, the magnitude of change is going to be Vector3.Distance(current_position-hit_position)
        // anyways.  
        Vector3 direction = Camera.main.ScreenToWorldPoint(current_position) - Camera.main.ScreenToWorldPoint(hit_position);

        // Invert direction to that terrain appears to move with the mouse.
        //direction = direction * -1;

        Vector3 position = camera_position + direction;

        transform.position = position;
    }
}
