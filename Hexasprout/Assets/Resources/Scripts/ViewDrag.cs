using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewDrag : MonoBehaviour
{
    Vector3 hit_position = Vector3.zero;
    Vector3 current_position = Vector3.zero;
    Vector3 camera_position = Vector3.zero;
    float z = 0.0f;
    float doubleClickTimer;
    bool doubleClick;

    // Use this for initialization
    void Start()
    {
        doubleClick = false;
        doubleClickTimer = Time.unscaledTime;
    }

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
        RaycastHit2D hit;

        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonUp(0))
        {
            if (doubleClick)
            {
                if (Time.unscaledTime - doubleClickTimer < 0.4f)
                {
                    // Double clicked!
                    doubleClick = false;

                    hit = Physics2D.Raycast(point, Vector2.zero);
                    if (hit.collider.gameObject.tag == "Field")
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
                                        g.GetComponent<CellManager>().ConnectWith(fm.neighbours[i].GetComponent<FieldManager>().cell.GetComponent<CellManager>(), i);
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
