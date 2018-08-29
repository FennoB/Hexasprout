using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum GUI_Event
{
    // the actual events which get triggered, when a user wants to improve the stat of a cell
    EnergyCap = -15,
    EnergyUse,
    LeafSpeed,
    WorkerCount,
    WorkerSpeedRed,
    WorkerSpeedGreen,
    WorkerSpeedYellow,
    WorkerSpeedBlack,
    WorkerSpeedBlue,

    // Cell gets destroyed
    DestroyCell,
    // A Build, initiated by a cell is ready
    BuildReady,
    // Grow to a superselected cell
    Grow,
    // decompose connection to a superselected cell 
    Decompose,
    // close menu of the selected cell
    CloseMenu,
    // open the menu for the selected cell
    OpenMenu,
    // Button that player want to destroy one cell got pressed
    BtnDestroy,
    // player want to see possible Transformations, for exemple if the cell has more than 
    BtnTransmorph,
    // the player wants to improve stats of the cell
    BtnSpecialize,
    // degenerate back to stemcell
    BtnDegenerate,
    //reduces the energy a cell uses 
    BtnEnergyuse,
    // increases the amount of energy a cell can hold
    BtnEnergycap,
    // different possibilitys to specialize to
    BtnMorph2Leaf,
    BtnMorph2Storage,
    BtnMorph2Heart,
    BtnMorph2Worker,
    // go back in the menu
    BtnNavMain,
    // the Menu for handling the storagecell functionality
    BtnStoreMenu,
    // increasing speed the leaf converts energy
    BtnLeafSpeed,
    // the number of possible working connections
    BtnWorkerCount,
    // the speed a workercell can mine special material 
    BtnWorkerSpeedRed,
    BtnWorkerSpeedBlue,
    BtnWorkerSpeedGreen,
    BtnWorkerSpeedBlack,
    BtnWorkerSpeedYellow,
    // the menu for the heartcell functionality
    BtnHeartMenu,
    length
}

public class GUIManager : MonoBehaviour
{
    // This is used for ViewDrag
    private Vector3 hit_position = Vector3.zero;
    private Vector3 current_position = Vector3.zero;
    private Vector3 camera_position = Vector3.zero;
    private bool world_ui_blocked;
    public GameObject world;

    // This is used for CellMenu
    public bool CellMenuOpen = false;
    public FieldManager CellMenuTarget;

    // This is used for managing touch
    private Touch touch;
    private int tapUpCounter = 0;       // For single Click
    private int tapCounter = 0;         // For double Click
    private float zoomstartdis = 0.0f;  // Distance of the touches when zoom starts
    private Vector2 zoompos;            // Center of the touches when zooming

    // Slider Button List
    public GameObject[] SliderButtons;
    [Range(0.0f, 0.9999f)]
    public float sliderPosition = 0.0f;     // 0.0f - 0.999f Slider goes in circles
    private int SliderCounter = 0;          // Counts the active Buttons on the slider
    private Vector2 SliderTouch;            // Touch position when user starts sliding
    private bool Sliding = false;           // True when user is sliding

    public float SliderPosition
    {
        get
        {
            return sliderPosition;
        }

        set
        {
            sliderPosition = value;
        }
    }

    // Start
    void Start()
    {
        SliderTouch = new Vector2();
        SliderButtons = new GameObject[(int)GUI_Event.length];

        for (int i = 0; i < (int)GUI_Event.length; i++)
        {
            GameObject button = transform.GetChild(0).GetChild(i).gameObject;
            SliderButtons[(int)button.GetComponent<ButtonScript>().ButtonID] = button;
            
        }

        ResetSliderButtons();
    }

    // Called every frame
    void Update()
    {
        // Blocking cause menus open?
        if (transform.GetChild(3).gameObject.activeSelf)
        {
            return;
        }

        // Handle Touch stuff
        TouchHandling();
        RegenerateSlider();
        MoveSlider();
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
        else if (Input.touchCount >= 2)
        {
            state = -2;
            hit_position = Input.mousePosition;
            camera_position = world.transform.position;
            ApplyZoom();
        }

        // User just began touch
        if (state == 0 || (Input.GetMouseButtonDown(0) && state == -1))
        {
            // For ViewDrag:
            hit_position = Input.mousePosition;
            camera_position = world.transform.position;
            world_ui_blocked = false;

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
        if ((state == 2 || (Input.GetMouseButtonUp(0) && state == -1)) && !world_ui_blocked)
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
        // Cell menu open, this means another cell is already selected
        if (CellMenuOpen)
        {
            if (fm.State == FieldState.Grow)
            {
                fm.State = FieldState.SuperSelected;
                CellMenuTarget.Cell.GetComponent<CellManager>().EventHandler(GUI_Event.Grow, this);
            }
            if (fm.State == FieldState.Decompose)
            {
                fm.State = FieldState.SuperSelected;
                CellMenuTarget.Cell.GetComponent<CellManager>().EventHandler(GUI_Event.Decompose, this);
            }
            CloseCellMenu();
        }
        else
        {
            // Tapped on a cell?
            if (fm.Cell != null && fm.Cell.GetComponent<CellManager>().alive)
            {
                // Yes. Open Cell menu
                OpenCellMenu(fm);
            }
            else
            {
                // Just to reset everything
                CloseCellMenu();
            }
        }
    }

    // Open selection menu
    void OpenSelectionMenu(FieldManager fm)
    {
        fm.State = FieldState.Selected;
        //and visualize connection to neighbours

        CellManager cm = fm.Cell.GetComponent<CellManager>();

        // Neighbour states
        for (int i = 0; i < fm.GetNeighbours().Length; i++)
        {
            if (fm.GetNeighbours()[i] != null)
            {
                // Decompose?
                if (cm.connections[i] != null)
                {
                    fm.GetNeighbours()[i].State = FieldState.Decompose;
                }

                // Material?
                else if (fm.GetNeighbours()[i].Material == null)
                {
                    // Connection possible?

                    //* Only stemcells and storagecells can build connections
                    if (cm.cellType == CellType.Stemcell || cm.cellType == CellType.Storagecell)
                    /*/
                    //All cells can build connections if max not reached (later)
                    if(!cm.ConMaxReached())
                    //*/
                    {
                        // From this side: yes
                        if
                        (
                            fm.GetNeighbours()[i].Cell == null || !fm.GetNeighbours()[i].GetComponentInChildren<CellManager>().ConMaxReached()
                        )
                        {
                            // From the other side: Yes. Grow then
                            fm.GetNeighbours()[i].State = FieldState.Grow;
                        }
                    }
                }
                else
                {
                    // Yes. Workercell?
                    if (cm.cellType == CellType.Workercell)
                    {
                        // Already connected?
                        if (cm.GetComponent<WorkerCellSpec>().MaterialNeighbours[i] == null)
                        {
                            // No. Able to connect?
                            if (!cm.GetComponent<WorkerCellSpec>().ConMaxReached())
                            {
                                // Yes
                                fm.GetNeighbours()[i].State = FieldState.Grow;
                            }
                        }
                        else
                        {
                            // Yes. Decompose then
                            fm.GetNeighbours()[i].GetComponent<FieldManager>().State = FieldState.Decompose;
                        }
                    }
                }
            }
        }
    }

    // Cell Selection Menu Closing
    void ResetSelectedCells()
    {
        if (CellMenuTarget != null)
        {
            for (int i = 0; i < CellMenuTarget.GetNeighbours().Length; i++)
            {
                if (CellMenuTarget.GetNeighbours()[i] != null)
                {
                    if (CellMenuTarget.GetNeighbours()[i].State == FieldState.Decompose ||
                       CellMenuTarget.GetNeighbours()[i].State == FieldState.Grow ||
                       CellMenuTarget.GetNeighbours()[i].State == FieldState.Selected ||
                       CellMenuTarget.GetNeighbours()[i].State == FieldState.SuperSelected)
                    {
                        if (CellMenuTarget.GetNeighbours()[i].Cell == null)
                        {
                            CellMenuTarget.GetNeighbours()[i].State = FieldState.Visible;
                        }
                        else
                        {
                            CellMenuTarget.GetNeighbours()[i].State = FieldState.HasCell;
                        }
                    }
                }
            }
            CellMenuTarget.State = FieldState.HasCell;
        }

    }

    // Handles other button events
    public void EventHandler(GUI_Event e)
    {
        // Blocking cause menus open?
        if (transform.GetChild(3).gameObject.activeSelf || transform.GetChild(4).gameObject.activeSelf)
        {
            return;
        }

        // Give event to menu cell
        if (CellMenuOpen)
        {
            world_ui_blocked = true;

            if (e == GUI_Event.BtnStoreMenu)
            {
                transform.GetChild(3).gameObject.SetActive(true);
                ResetSliderButtons();
                ResetSelectedCells();
            }

            if (e == GUI_Event.BtnHeartMenu)
            {
                transform.GetChild(4).gameObject.SetActive(true);
                ResetSliderButtons();
                ResetSelectedCells();
            }
            if (e == GUI_Event.BtnDestroy)
            {
                transform.GetChild(5).gameObject.SetActive(true);
                ResetSliderButtons();
                ResetSelectedCells();
            }
            //Here are the Improvement Events handled, it seems to me that this position is a good place for this, because
            //its pretty centralized, 
            if (e == GUI_Event.BtnEnergycap)
            {
                GameObject ImprovePanel = transform.GetChild(6).gameObject;
                ImprovePanel.SetActive(true);
               
                ImprovePanel.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = CellMenuTarget.Cell.GetComponent<CellManager>().energyMax.ToString();
                ImprovePanel.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Text>().text = (CellMenuTarget.Cell.GetComponent<CellManager>().energyMax * 1.1f).ToString();
                ImprovePanel.transform.GetChild(2).gameObject.GetComponent<UnityEngine.UI.Text>().text = "Increase Capacity";
            }
            // this is necessary for the case, that the user destroys a cell, because then theres no reference anymore
            if (CellMenuTarget != null)
            {
                CellMenuTarget.Cell.GetComponent<CellManager>().EventHandler(e, this);
            }
        }
    }

    // Called to open cell menus
    public void OpenCellMenu(FieldManager target)
    {
        CloseCellMenu();

        CellMenuTarget = target;
        CellMenuOpen = true;
        ResetSliderButtons();
        transform.GetChild(0).gameObject.SetActive(true);

        //set cell as the by the player selected one
        OpenSelectionMenu(target);
        EventHandler(GUI_Event.OpenMenu);
    }

    // Calles to close cell menus
    public void CloseCellMenu()
    {
        // Close Menus

        transform.GetChild(3).gameObject.SetActive(false);
        transform.GetChild(4).gameObject.SetActive(false);
        transform.GetChild(5).gameObject.SetActive(false);
        transform.GetChild(6).gameObject.SetActive(false);

        if (CellMenuOpen)
        {
            // Close Slider menu
            transform.GetChild(2).gameObject.SetActive(false);
            ResetSliderButtons();
            EventHandler(GUI_Event.CloseMenu);

            // Close Selection menu
            ResetSelectedCells();

            CellMenuOpen = false;
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    // LoadBar
    public LoadBarManager OpenLoadBar(Sprite s)
    {
        CellMenuOpen = true;
        GameObject loadbar = transform.GetChild(2).gameObject;
        loadbar.SetActive(true);
        loadbar.GetComponent<LoadBarManager>().SetPicture(s);
        return loadbar.GetComponent<LoadBarManager>();
    }
    public LoadBarManager OpenLoadBar(GUI_Event buttonImg)
    {
        return OpenLoadBar(SliderButtons[(int)buttonImg].GetComponent<UnityEngine.UI.Image>().sprite);
    }
    public LoadBarManager SetLoadBar(float progress)
    {
        GameObject loadbar = transform.GetChild(2).gameObject;
        loadbar.GetComponent<LoadBarManager>().progress = progress;
        return loadbar.GetComponent<LoadBarManager>();
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
    IEnumerator DoubleClicked()
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

    // Used to drag camera with moving finger on screen
    void LeftMouseDrag()
    {
        if (world_ui_blocked)
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

        world.transform.position = position;
    }

    // Zoom 
    void ApplyZoom()
    {
        if (Input.touchCount == 2)
        {
            world_ui_blocked = true;

            if (Input.touches[1].phase == TouchPhase.Began)
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

            if (Input.touches[1].phase == TouchPhase.Moved)
            {
                // 1. Move world with new zoom pos
                Vector2 newzoompos = (Input.touches[0].position + Input.touches[1].position) / 2.0f;
                Vector2 worlddelta = Camera.main.ScreenToWorldPoint(newzoompos) - Camera.main.ScreenToWorldPoint(zoompos);
                world.transform.position += new Vector3(worlddelta.x, worlddelta.y, 0);
                zoompos = (Input.touches[0].position + Input.touches[1].position) / 2.0f;

                // 2. Calculate zoom factor
                float zoomfactor = Vector2.Distance(Input.touches[0].position, Input.touches[1].position) / zoomstartdis;

                // 3. Move newzoompos with world to the center of the screen
                Vector2 screencenter = Camera.main.transform.position;
                Vector2 worldzoompos = Camera.main.ScreenToWorldPoint(newzoompos);
                Vector2 zoomdelta = screencenter - worldzoompos;
                world.transform.position += new Vector3(zoomdelta.x, zoomdelta.y, 0);

                // 4. Apply zoom
                Camera.main.orthographicSize /= zoomfactor;
                if (Camera.main.orthographicSize > 5)
                {
                    Camera.main.orthographicSize = 5;
                }
                else if (Camera.main.orthographicSize < 0.5f)
                {
                    Camera.main.orthographicSize = 0.5f;
                }

                zoomstartdis = Vector2.Distance(Input.touches[0].position, Input.touches[1].position);

                // 5. Move transformed newzoompos with world to newzoompos from before
                worldzoompos = Camera.main.ScreenToWorldPoint(newzoompos);
                zoomdelta = screencenter - worldzoompos;
                world.transform.position -= new Vector3(zoomdelta.x, zoomdelta.y, 0);

                // Done!
            }
        }
    }

    // Prepare the Slider menu
    public void ResetSliderButtons()
    {
        foreach (GameObject button in SliderButtons)
        {
            button.SetActive(false);
        }
    }

    // Use this function to add Buttons to the slider menu
    public void AddSliderButton(GUI_Event e)
    {
        if (e >= 0)
        {
            SliderButtons[(int)e].SetActive(true);
        }
    }

    // Slider visual 
    void RegenerateSlider()
    {
        int counter = 0;    // Counts the Buttons that are activated
        List<GameObject> list = new List<GameObject>();
        foreach (GameObject button in SliderButtons)
        {
            if (button.activeSelf)
            {
                counter++;
                list.Add(button);
            }
        }

        SliderCounter = counter;

        // Determine Sliding
        if (counter > 5)
        {
            // Sliding
            transform.GetChild(1).gameObject.SetActive(true);

            float pos = SliderPosition * counter;
            float stepextra = pos - (int)pos;
            for (int i = 0; i < counter; i++)
            {
                float newposx = 0;
                int index = (i - ((int)pos - 2) + counter) % counter;
                newposx = (128f + 16f) * (-index + 2 + stepextra);

                if (index == 0)
                {
                    list[i].GetComponent<UnityEngine.UI.Image>().color = new Color(1f, 1f, 1f, 1f - stepextra);
                }
                else if (index == 5)
                {
                    list[i].GetComponent<UnityEngine.UI.Image>().color = new Color(1f, 1f, 1f, stepextra);
                }
                else if (index > 5)
                {
                    list[i].GetComponent<UnityEngine.UI.Image>().color = new Color(1f, 1f, 1f, 0f);
                }
                else
                {
                    list[i].GetComponent<UnityEngine.UI.Image>().color = new Color(1f, 1f, 1f, 1f);
                }

                if (list[i].GetComponent<UnityEngine.UI.Image>().color.a > 0.5f)
                {
                    list[i].GetComponent<UnityEngine.UI.Button>().interactable = true;
                }
                else
                {
                    list[i].GetComponent<UnityEngine.UI.Button>().interactable = false;
                }

                list[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(newposx, list[i].GetComponent<RectTransform>().anchoredPosition.y);
            }
        }
        else
        {
            // No fading, Just show the buttons
            transform.GetChild(1).gameObject.SetActive(false);

            for (int i = 0; i < counter; i++)
            {
                float newposx = (120f + 16f) * (i - counter * 0.5f + 0.5f);
                list[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(newposx, list[i].GetComponent<RectTransform>().anchoredPosition.y);
            }
        }
    }

    // Slider move
    void MoveSlider()
    {
        if (Input.touchCount == 1)
        {
            Touch t = Input.touches[0];
            if (t.phase == TouchPhase.Began)
            {
                if (t.position.x > -255 && t.position.y < 255 && t.position.y > transform.GetChild(0).position.y - 32 && t.position.y < transform.GetChild(0).position.y + 32)
                {
                    world_ui_blocked = true;
                    SliderTouch = t.position;
                    Sliding = true;
                }
            }
            if (t.phase == TouchPhase.Moved && Sliding && SliderCounter > 0)
            {
                Vector3 delta = t.position - SliderTouch;
                float slideDelta = 3 * delta.x / 255f;
                float slidePosDelta = slideDelta / SliderCounter;
                SliderPosition += slidePosDelta;
                SliderTouch = t.position;
                while (SliderPosition < 0)
                {
                    SliderPosition += 1f;
                }
                while (SliderPosition >= 1)
                {
                    SliderPosition -= 1f;
                }
            }
            if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
            {
                world_ui_blocked = false;
                Sliding = false;
            }
        }
    }
}
