﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Juice
{
    public float red;
    public float green;
    public float blue;
    public float blueCharged;
    public float yellow;
    public float black;

    public Juice()
    {
        red = 0;
        green = 0;
        blue = 0;
        blueCharged = 0;
        yellow = 0;
        black = 0;
    }

    public Juice(float red, float green, float blue, float blueCharged, float yellow, float black)
    {
        this.red = red;
        this.green = green;
        this.blue = blue;
        this.blueCharged = blueCharged;
        this.yellow = yellow;
        this.black = black;
    }

    public void SetAllTo(float value)
    {
        red = value;
        green = value;
        blue = value;
        blueCharged = value;
        yellow = value;
        black = value;
    }

    public static Juice operator+(Juice a, Juice b)
    {
        Juice result = new Juice
        {
            red = a.red + b.red,
            green = a.green + b.green,
            blue = a.blue + b.blue,
            blueCharged = a.blueCharged + b.blueCharged,
            yellow = a.yellow + b.yellow,
            black = a.black + b.black
        };
        return result;
    }

    public static Juice operator*(Juice a, float scale)
    {
        Juice result = new Juice
        {
            red = a.red * scale,
            green = a.green * scale,
            blue = a.blue * scale,
            blueCharged = a.blueCharged * scale,
            yellow = a.yellow * scale,
            black = a.black * scale
        };
        return result;
    }
    

    public static Juice operator*(float scale, Juice a)
    {
        return a * scale;
    }

    public static Juice operator-(Juice a, Juice b)
    {
        return (b * -1.0f) + a;
    }

    public static Juice operator/(Juice a, float div)
    {
        return a * (1.0f / div);
    }
    public static bool operator >(Juice a, Juice b)
    {
        if (a.red > b.red && a.green > b.green && a.blue > b.blue && a.blueCharged > b.blueCharged && a.yellow > b.yellow && a.black > b.black)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool operator <(Juice a, Juice b)
    {
        if (a.red < b.red || a.green < b.green || a.blue < b.blue || a.blueCharged < b.blueCharged || a.yellow < b.yellow || a.black < b.black)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public float Sum
    {
        get
        {
            return red + green + blue + blueCharged + yellow + black;
        }
    }

    public Juice Copy
    {
        get
        {
            Juice result = new Juice();
            result.red = red;
            result.green = green;
            result.blue = blue;
            result.blueCharged = blueCharged;
            result.yellow = yellow;
            result.black = black;
            return result;
        }
    }

    public Juice PositivesOnly
    {
        get
        {
            Juice result = Copy;

            if (red < 0)
            {
                result.red = 0;
            }
            if (green < 0)
            {
                result.green = 0;
            }
            if (blue < 0)
            {
                result.blue = 0;
            }
            if (blueCharged < 0)
            {
                result.blueCharged = 0;
            }
            if (yellow < 0)
            {
                result.yellow = 0;
            }
            if (black < 0)
            {
                result.black = 0;
            }
            return result;
        }
    }

    public Juice AbsoluteValues
    {
        get
        {
            Juice result = Copy;

            if (red < 0)
            {
                result.red = -red;
            }
            if (green < 0)
            {
                result.green = -green;
            }
            if (blue < 0)
            {
                result.blue = -blue;
            }
            if (blueCharged < 0)
            {
                result.blueCharged = -blueCharged;
            }
            if (yellow < 0)
            {
                result.yellow = -yellow;
            }
            if (black < 0)
            {
                result.black = -black;
            }
            return result;
        }
    }
}

public enum CellType
{
    None = -1,
    Stemcell,
    Leafcell,
    Workercell,
    Heartcell,
    Storagecell,
    Breedcell
}

public class CellManager : MonoBehaviour
{
    public float energy = 1.0f;        // Energy battery
    public float energyMax = 1.0f;     // Battery size
    public float energyUse = 0.2f;     // Usage per minute (Death after 5 minutes without supply)
    public float energyConvert = 0.5f; // Convertion of chemical to cellenegy per minute (2 minutes to reload battery)
    public bool alive = true;
    public float diffusionFactor = 0.5f;    // Diffusion speed. Behaviour undefined when > 1.0f
    public CellType cellType = CellType.Stemcell;        // Celltypes: stem=0, leaf=1, worker=2, heart=3, storage=4, breed=5
    public FieldManager Field;

    public Juice juice;
    public Juice diffusionDelta;
    public CellManager[] connections;

    public int tempid;      // My ID in the most recently generated Heartmap

	// Use this for initialization
	void Awake ()
    {
        // Reset Substance Deltas
        diffusionDelta = new Juice();

        connections = new CellManager[6];
        for(int i = 0; i < 6; i++)
        {
            connections[i] = null;
        }

        juice = new Juice
        {
            blueCharged = 0.5f,
            black = 0.5f
        };  
    }
     

    private void Start()
    {
        Field = this.gameObject.transform.parent.GetComponentInParent<FieldManager>();
    }

    // Update is called once per frame
    public void OwnFixedUpdate()
    {
        if (alive)
        {
            AbsorbEnergy();
            UseEnergy();

            switch(cellType)
            {
                case CellType.Stemcell:
                    GetComponent<StemCellSpec>().OwnFixedUpdate();
                    break;
                case CellType.Leafcell:
                    GetComponent<LeafCellSpec>().OwnFixedUpdate();
                    break;
                case CellType.Workercell:
                    GetComponent<WorkerCellSpec>().OwnFixedUpdate();
                    break;
                case CellType.Storagecell:
                    GetComponent<StorageCellSpec>().OwnFixedUpdate();
                    break;
                case CellType.Heartcell:
                    GetComponent<HeartCellSpec>().OwnFixedUpdate();
                    break;
                case CellType.Breedcell:
                    GetComponent<BreedCellSpec>().OwnFixedUpdate();
                    break;
            }
        }
    }

    // Handle events
    public void EventHandler(GUI_Event e, GUIManager gm)
    {
        // Own stuff
        switch (e)
        {
            // Core Events:
            case GUI_Event.OpenMenu:
                OpenMenu(gm);
                break;
            case GUI_Event.CloseMenu:
                CloseMenu(gm);
                break;
            case GUI_Event.Decompose:
                DecomposeConnection();
                break;

            // Button Events:
            case GUI_Event.BtnSpecialize:
                OpenSpecializeMenu(gm);
                break;

            case GUI_Event.BtnDestroy:
                OpenDestroyMenu(gm);
                break;
            
            // Back to main
            case GUI_Event.BtnNavMain:
                gm.ResetSliderButtons();
                OpenMenu(gm);
                break;
            case GUI_Event.BtnMorph2Heart:
                Morph2Heart();
                break;
        }

        // Cell specs
        switch (cellType)
        {
            case CellType.Stemcell:
                GetComponent<StemCellSpec>().EventHandler(e, gm);
                
                break;
            case CellType.Leafcell:
                GetComponent<LeafCellSpec>().EventHandler(e, gm);
                break;
            case CellType.Workercell:
                GetComponent<WorkerCellSpec>().EventHandler(e, gm);
                break;
            case CellType.Heartcell:
                GetComponent<HeartCellSpec>().EventHandler(e, gm);
                break;
            case CellType.Storagecell:
                GetComponent<StorageCellSpec>().EventHandler(e, gm);
                break;
            case CellType.Breedcell:
                GetComponent<BreedCellSpec>().EventHandler(e, gm);
                break;
        }
    }

    void Morph2Heart()
    {
        GameObject.Find("World").GetComponent<WorldGenerator>().Morph2Heart(this);

    }
    // Adds buttons to the Slider menu
    void OpenMenu(GUIManager gm)
    {
        // Main menu: 
        gm.AddSliderButton(GUI_Event.BtnDestroy);
        gm.AddSliderButton(GUI_Event.BtnSpecialize);
    }

    // Does something when cellmenu closes (maybe later)
    void CloseMenu(GUIManager gm)
    {
        // Nothing! Wheee!
    }

    // Specialize slider menu
    void OpenSpecializeMenu(GUIManager gm)
    {
        gm.ResetSliderButtons();
        gm.AddSliderButton(GUI_Event.BtnEnergycap);
        gm.AddSliderButton(GUI_Event.BtnEnergyuse);
        gm.AddSliderButton(GUI_Event.BtnNavMain);
    }

    // Destroy Dialog
    void OpenDestroyMenu(GUIManager gm)
    {
        // Nothing yet
    }

    // Absorb energy from blood
    void AbsorbEnergy()
    {
        float delta = energyConvert * (Time.deltaTime / 60.0f);
        if(energyMax - energy < energyConvert * (Time.deltaTime / 60.0f))
        {
            delta = energyMax - energy;
        }
        if(juice.blueCharged < delta)
        {
            delta = juice.blueCharged;
        }

        juice.blueCharged -= delta;
        juice.blue += delta;
        energy += delta;
    }

    // Decompose
    void DecomposeConnection()
    {
        for (int i = 0; i < Field.neighbours.Length; i++)
        {
            if (Field.neighbours[i] != null)
            {
                if (Field.neighbours[i].State == FieldState.SuperSelected && !Field.neighbours[i].HasMaterial())
                {
                    Field.Cell.GetComponent<CellManager>().DecomposeWith(Field.neighbours[i].Cell.GetComponent<CellManager>(), i);
                }
            }
        }
    }

    // Use energy for living
    void UseEnergy()
    {
        energy -= energyUse * (Time.deltaTime / 60.0f);     // Usage = energyUse * deltaTime in minutes
        if(energy <= 0 || alive == false)   // alive == false --> externally set to false -> Death() has to be called too!
        {
            // NJJjaaaaarrgrgrghghuhuhuuu!!!!!1........
            Death();
        }
    }

    // Connect
    public void ConnectWith(CellManager cm, int conID)
    {
        // Already connected?
        if (connections[conID] == null)
        {
            //No. Connect!
            //I know you
            connections[conID] = cm;

            //You know me
            cm.ConnectWith(this, (conID + 3) % 6);
        }
    }

    // Decompose a Connnection to a neighbour
    public void DecomposeWith(CellManager cm, int conID)
    {
        // is there a connection established?
        if (connections[conID] != null)
        {
            //No. Connect!
            //I know you
            connections[conID] = null;

            //You know me
            cm.DecomposeWith(this, (conID + 3) % 6);
        }
    }

    // Death
    void Death()
    {
        energy = 0;
        alive = false;
        GameObject child = transform.GetChild(0).gameObject;
        child.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 1.0f);
    }

    // Juice Diffusion Calculation
    public void DiffusionCalc()
    {
        // Calculation of new Substance Deltas 0, 1, 2        
        for (int i = 0; i < 3; i++)
        {
            //Calculation only for the first three connections. Reason:
            //The diffusion delta for each connection only needs to be calculated from one side
             
            if (connections[i] != null)
            {
                Juice buf = connections[i].GetComponent<CellManager>().juice;
                Juice otherDifDelta = connections[i].GetComponent<CellManager>().diffusionDelta;
                Juice delta = (juice - buf) * diffusionFactor * 0.1666f;


                //Add deltas to diffusionDeltas
                diffusionDelta -= delta;
                connections[i].GetComponent<CellManager>().diffusionDelta += delta;
            }
        }
        
    }

    //Juice Diffusion Apply
    public void DiffusionApply()
    {
        //deltaTime sollte niemals > 1.0f sein, sonst sind schwingeffekte möglich
        juice += Time.deltaTime * diffusionDelta;

        // Reset Substance Deltas
        diffusionDelta = new Juice();

    }
}
