using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ViewDrag : MonoBehaviour
{
    private Vector3 hit_position = Vector3.zero;
    private Vector3 current_position = Vector3.zero;
    private Vector3 camera_position = Vector3.zero;
    private int tapCounter = 0;
    //float z = 0.0f;
    //float doubleClickTimer;
    //bool doubleClick;

    // Use this for initialization
    /*void Start()
    {
        doubleClick = false;
        doubleClickTimer = Time.unscaledTime;
    }*/

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
        if (Input.GetMouseButtonUp(0))
        {
            tapCounter++;
            if (tapCounter == 1)
            {
                StartCoroutine("DoubleClicked");
            }
        }
            /*if (doubleClick)
            {
                if (Time.unscaledTime - doubleClickTimer < 0.4f)
                {
                    // Double clicked!
                    doubleClick = false;

                    hit = Physics2D.Raycast(point, Vector2.zero);
                    if (hit.collider != null && hit.collider.gameObject.tag == "Field")
                    {
                        FieldManager fm = hit.collider.gameObject.GetComponent<FieldManager>();
                        if (fm.cell == null)
                        {
                            // Cell creation
                            GameObject g = Instantiate((GameObject)Resources.Load("Prefabs/StemCell", typeof(GameObject)));
                            g.GetComponent<Transform>().SetParent(fm.gameObject.GetComponent<Transform>());
                            g.GetComponent<Transform>().localPosition = new Vector3(0, 0, -0.14f);
                            fm.gameObject.GetComponent<FieldManager>().cell = g;

                            // Neighbour Cells
                            for (int i = 0; i < 6; i++)
                            {
                                if (fm.neighbours[i] != null)
                                {
                                    if (fm.neighbours[i].GetComponent<FieldManager>().cell != null)
                                    {
                                        g.GetComponent<CellManager>().ConnectWith
                                            (fm.neighbours[i].GetComponent<FieldManager>().cell.GetComponent<CellManager>(), i);
                                    }
                                }
                            }

                        }
                        else
                        {
                            hit.collider.gameObject.GetComponent<FieldManager>().warmth = Random.value;
                        }
                    }
                }
                else
                {
                    // Next time?
                    doubleClickTimer = Time.unscaledTime;
                }
            }
            else
            {
                doubleClick = true;
                doubleClickTimer = Time.unscaledTime;
            }
        }*/
    }

    IEnumerator DoubleClicked ()
    {
        yield return new WaitForSeconds(0.5f);
        if (tapCounter > 1)
        {
            Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(point, Vector2.zero);


            // if a field is hit by double click 
            if (hit.collider != null && hit.collider.gameObject.tag == "Field")
            {
                FieldManager fm = hit.collider.gameObject.GetComponent<FieldManager>();

                //if selected field is neighbourselected, it means that player want to build new cell there
                if (fm.cell == null && fm.GetNeighbourSelected())
                {
                    CreateCell(fm);

                    ResetSelectStateOfCells(GameObject.Find("World").GetComponent<WorldGenerator>().GetFields());
                    /*FieldManager selected = GetSelectedCell(fm);
                    if (selected != null)
                    {
                        selected.SetSelected(false);
                        for (int i = 0; i < 6; i++)
                        {
                            if (fm.GetNeighbours()[i] != null)
                            {
                                selected.GetNeighbours()[i].gameObject.GetComponent<FieldManager>().SetNeighbourSelected(false);
                            }
                        }

                    }*/
                }
                //means that player want to build connection between neighbourselected cell and selected cell
                //TODO buid connection
                if (fm.cell != null && fm.GetNeighbourSelected())
                {
                    ResetSelectStateOfCells(GameObject.Find("World").GetComponent<WorldGenerator>().GetFields());
                    /*FieldManager selected = GetSelectedCell(fm);
                    if (selected != null)
                    {
                        selected.SetSelected(false);
                        for (int i = 0; i < 6; i++)
                        {
                            if (fm.GetNeighbours()[i] != null)
                            {
                                selected.GetNeighbours()[i].gameObject.GetComponent<FieldManager>().SetNeighbourSelected(false);
                            }
                        }
                    }*/
                }
                //first tap, select a cell
                if (fm.cell != null && !fm.GetNeighbourSelected())
                {
                    ResetSelectStateOfCells(GameObject.Find("World").GetComponent<WorldGenerator>().GetFields());
                    fm.SetSelected(true);
                    for (int i = 0; i < 6; i++)
                    {
                        if (fm.GetNeighbours()[i] != null)
                        {
                            fm.GetNeighbours()[i].gameObject.GetComponent<FieldManager>().SetNeighbourSelected(true);
                        }
                    }
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

        // Neighbour Cells
        for (int i = 0; i < 6; i++)
        {
            if (fm.neighbours[i] != null)
            {
                if (fm.neighbours[i].GetComponent<FieldManager>().cell != null)
                {
                    g.GetComponent<CellManager>().ConnectWith
                        (fm.neighbours[i].GetComponent<FieldManager>().cell.GetComponent<CellManager>(), i);
                }
            }
        }
    }

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
