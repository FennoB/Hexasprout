using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum GUI_Event
{
    Menu = -1,
    BtnDestroy,
    BtnTransmorph,
    BtnSpecialize,
    BtnDegenerate,
    BtnEnergyuse,
    BtnEnergycap,
    BtnMorph2Leaf,
    BtnMorph2Storage,
    BtnMorph2Heart,
    BtnMorph2Worker,
    BtnStorageSpeed,
    BtnStore,
    BtnLeafSpeed,
    BtnWorkerCount,
    BtnWorkerSpeedRed,
    BtnWorkerSpeedBlue,
    BtnWorkerSpeedGreen,
    BtnWorkerSpeedBlack,
    BtnWorkerSpeedYellow,
    BtnHeartSpeed,
    BtnHeartSwitch,
    BtnHeartSwitchDir,
    length
}

public class GUIManager : MonoBehaviour
{
    // This is used for ViewDrag
    private Vector3 hit_position = Vector3.zero;
    private Vector3 current_position = Vector3.zero;
    private Vector3 camera_position = Vector3.zero;
    private bool view_drag_blocked;

    // This is used for CellMenu
    private bool CellMenuOpen = false;
    private FieldManager CellMenuTarget;

    // This is used for managing touch
    private Touch touch;
    private int tapUpCounter = 0;       // For single Click
    private int tapCounter = 0;         // For double Click
    private float zoomstartdis = 0.0f;  // Distance of the touches when zoom starts
    private Vector2 zoompos;            // Center of the touches when zooming

    // Slider Button List
    private GameObject[] SliderButtons;

    // Start
    void Start()
    {
        SliderButtons = new GameObject[(int)GUI_Event.length];

        for(int i = 0; i < (int)GUI_Event.length; i++)
        {
            GameObject button = transform.GetChild(0).GetChild(i).gameObject;
            SliderButtons[(int)button.GetComponent<ButtonScript>().ButtonID] = button;
        }
    }

    // Called every frame
    void Update()
    {
        // Handle Touch stuff
        TouchHandling();
    }

    // Touch stuff
    void TouchHandling()
    {
        // Does the user touch the screen?
        int state = -1;
        if (Input.touchCount == 1)
        {
            // Yes. Get first touch
            touch = Input.GetTouch(0);

            // Every phase for a state
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    state = 0;
                    break;

                case TouchPhase.Moved:
                    state = 1;
                    break;

                case TouchPhase.Ended:
                    state = 2;
                    break;

                case TouchPhase.Canceled:
                    state = 2;
                    break;
            }
        }
        else if(Input.touchCount >= 2)
        {
            state = -2;
            hit_position = Input.mousePosition;
            camera_position = transform.position;
            ApplyZoom();
        }
        
        // User just began touch
        if (state == 0 || (Input.GetMouseButtonDown(0) && state == -1))
        {
            // For ViewDrag:
            view_drag_blocked = false;
            hit_position = Input.mousePosition;
            camera_position = transform.position;

            // Single Click?
            tapUpCounter = 0;
            StartCoroutine("SingleClicked");
        }

        // Finger moved over the screen
        if (state == 1 || (Input.GetMouseButton(0) && state == -1))
        {
            // Drag camera
            current_position = Input.mousePosition;
            LeftMouseDrag();
        }

        // Touch stops
        if (state == 2 || (Input.GetMouseButtonUp(0) && state == -1))
        {
            // Add tap
            tapUpCounter++;
            tapCounter++;

            // Ready for Double Click?
            if (tapCounter == 1)
            {
                StartCoroutine("DoubleClicked");
            }
        }
    }
    
    // Called when user tapped on a field
    void TappedOnField(FieldManager fm)
    {
        // Cell menu open?
        if (CellMenuOpen)
        {
            // Is this in the selection?
            if (fm.GetNeighbourSelected())
            {
                // Tapped on a cell?
                if (fm.GetCell() == null)
                {
                    // Nope
                    CreateCell(fm);
                }

                // Connect
                MakeCellConnection(CellMenuTarget, fm);
            }

            CloseCellMenu();
        }
        else
        {
            // Tapped on a cell?
            if (fm.GetCell() != null)
            {
                // Yes. Open Cell menu
                OpenCellMenu(fm);
            }
        }
    }

    // Handles other button events
    public void EventHandler(GUI_Event e)
    {
        // Give event to menu cell
        if(CellMenuOpen)
        {
            CellMenuTarget.cell.GetComponent<CellManager>().EventHandler(e, this);
        }
    }

    // Called to open cell menus
    public void OpenCellMenu(FieldManager target)
    {
        CloseCellMenu();

        // Open Slider menu
        target.GetCell().GetComponent<StemCellSpec>().ActivateOptionScript();

        // Set Field to Selection Mode
        target.SetSelected(true);

        // Set Neighbours to Selection Mode
        for (int i = 0; i < 6; i++)
        {
            if (target.GetNeighbours()[i] != null)
            {
                target.GetNeighbours()[i].GetComponent<FieldManager>().SetNeighbourSelected(true);
            }
        }

        CellMenuTarget = target;
        CellMenuOpen = true;
    }

    // Calles to close cell menus
    public void CloseCellMenu()
    {
        if (CellMenuOpen)
        {
            // Close Slider menu
            CellMenuTarget.GetCell().GetComponent<StemCellSpec>().DeactivateOptionScript();

            // Close Selection menu
            ResetSelectStateOfCells(GameObject.Find("World").GetComponent<WorldGenerator>().GetFields());
            CellMenuOpen = false;
        }
    }

    // Is called when Single tap ocurrs
    IEnumerator SingleClicked()
    {
        // Wait 0.1 Seconds
        yield return new WaitForSeconds(0.2f);

        // Check if tap
        if (tapUpCounter == 1)
        {
            Debug.Log("Click");
            // Get world pos from tap pos
            Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Raycast
            RaycastHit2D hit = Physics2D.Raycast(point, Vector2.zero);

            // Tapped on a field?
            if (hit.collider != null && hit.collider.gameObject.tag == "Field")
            {
                FieldManager fm = hit.collider.gameObject.GetComponent<FieldManager>();
                TappedOnField(fm);
            }
        }
    }

    // Doubleclicked is in a Coroutine for handling the Clickevent over time
    IEnumerator DoubleClicked ()
    {
        yield return new WaitForSeconds(0.5f);
        if (tapCounter > 1)
        {
        //    //the field hit by the user
        //    Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    RaycastHit2D hit = Physics2D.Raycast(point, Vector2.zero);
        }
        tapCounter = 0; 
    }

    // Called to place a new stemcell on a certain field
    void CreateCell(FieldManager fm)
    {
        // Cell creation
        GameObject.Find("World").GetComponent<WorldGenerator>().CreateStemCell(fm);
    }

    // A cell connection is established and the animation starts
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

    // Goes through all field an disables all selection modes
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

    // Used to drag camera with moving finger on screen
    void LeftMouseDrag()
    {
        if(view_drag_blocked)
        {
            return;
        }

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

    // Zoom 
    void ApplyZoom()
    {
        if(Input.touchCount == 2)
        {
            view_drag_blocked = true;

            if(Input.touches[1].phase == TouchPhase.Began)
            {
                // Calculate center
                zoompos = (Input.touches[0].position + Input.touches[1].position) / 2.0f;

                // Calculate distance at start
                zoomstartdis = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);
            }

            // This one will be a little hart to grasp
            // Its very tricky to get the effect that the point
            // where the zoom starts is the center of the zoom effect.
            // The trick is to move the world in a way that the world position
            // of the zoompoint (newzoompos -> worldzoompos) is in the center of the
            // screen. Then the zoom is applied and the world position of
            // the zoompoint has changed slightly. Now we can transform the world back
            // with this change beeing applied permanently.

            if(Input.touches[1].phase == TouchPhase.Moved)
            {
                // 1. Move world with new zoom pos
                Vector2 newzoompos = (Input.touches[0].position + Input.touches[1].position) / 2.0f;
                Vector2 worlddelta = Camera.main.ScreenToWorldPoint(newzoompos) - Camera.main.ScreenToWorldPoint(zoompos);
                transform.position += new Vector3(worlddelta.x, worlddelta.y, 0);
                zoompos = (Input.touches[0].position + Input.touches[1].position) / 2.0f;

                // 2. Calculate zoom factor
                float zoomfactor = Vector2.Distance(Input.touches[0].position, Input.touches[1].position) / zoomstartdis;

                // 3. Move newzoompos with world to the center of the screen
                Vector2 screencenter = Camera.main.transform.position;
                Vector2 worldzoompos = Camera.main.ScreenToWorldPoint(newzoompos);
                Vector2 zoomdelta = screencenter - worldzoompos;
                transform.position += new Vector3(zoomdelta.x, zoomdelta.y, 0);

                // 4. Apply zoom
                Camera.main.orthographicSize /= zoomfactor;
                if (Camera.main.orthographicSize > 5)
                {
                    Camera.main.orthographicSize = 5;
                }
                else if(Camera.main.orthographicSize < 0.5f)
                {
                    Camera.main.orthographicSize = 0.5f;
                }

                zoomstartdis = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);

                // 5. Move transformed newzoompos with world to newzoompos from before
                worldzoompos = Camera.main.ScreenToWorldPoint(newzoompos);
                zoomdelta = screencenter - worldzoompos;
                transform.position -= new Vector3(zoomdelta.x, zoomdelta.y, 0);

                // Done!
            }
        }
    }
}
