using System.Collections;
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
    public float bluePotency;

    public Juice()
    {
        red = 0;
        green = 0;
        blue = 0;
        blueCharged = 0;
        yellow = 0;
        black = 0;
        bluePotency = 2.0f;
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
    public CellType cellType = CellType.None;        // Celltypes: stem=0, leaf=1, worker=2, heart=3, storage=4, breed=5

    public Juice juice;
    public Juice diffusionDelta;
    public GameObject[] connections;
    
    //Here are the animators for the different directions of the connections of a cell
    public Animator animConnectionUp;
    public Animator animConnectionDown;
    public Animator animConnectionRightUp;
    public Animator animConnectionRightDown;
    public Animator animConnectionLeftUp;
    public Animator animConnectionLeftDown;

    public int tempid;      // My ID in the most recently generated Heartmap

	// Use this for initialization
	void Awake ()
    {
        connections = new GameObject[6];
        for(int i = 0; i < 6; i++)
        {
            connections[i] = null;
        }

        juice.blueCharged = 1.0f;
        
        //at the moment are the Animations only defined for stemcells, the selected child is the gameObject which is animated
        if (this.gameObject.name.Equals("StemCell(Clone)"))
        {
            animConnectionUp = this.gameObject.transform.GetChild(2).GetChild(0).GetComponent<Animator>();
            animConnectionDown = this.gameObject.transform.GetChild(2).GetChild(1).GetComponent<Animator>();
            animConnectionLeftDown = this.gameObject.transform.GetChild(2).GetChild(2).GetChild(0).GetComponent<Animator>();
            animConnectionLeftUp = this.gameObject.transform.GetChild(2).GetChild(3).GetChild(0).GetComponent<Animator>();
            animConnectionRightDown = this.gameObject.transform.GetChild(2).GetChild(4).GetChild(0).GetComponent<Animator>();
            animConnectionRightUp = this.gameObject.transform.GetChild(2).GetChild(5).GetChild(0).GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    public void OwnFixedUpdate()
    {
        if (alive)
        {
            AbsorbEnergy();
            UseEnergy();
        }
    }

    // Handle events
    public void EventHandler(GUI_Event e, GUIManager gm)
    {
        // Own stuff

        // Cell specs
        switch(cellType)
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

    //Here are methods for triggering the different animations
    public void SetUpAnimation()
    {
        animConnectionUp.SetTrigger("BuildUp");
    }
    public void SetDownAnimation()
    {
        animConnectionDown.SetTrigger("BuildDown");
    }
    public void SetDownLeftAnimation()
    {
        animConnectionLeftDown.SetTrigger("BuildLeftDown");
    }
    public void SetUpLeftAnimation()
    {
        animConnectionLeftUp.SetTrigger("BuildLeftUp");
    }
    public void SetDownRightAnimation()
    {
        animConnectionRightDown.SetTrigger("BuildRightDown");
    }
    public void SetUpRightAnimation()
    {
        animConnectionRightUp.SetTrigger("BuildRightUp");
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
            connections[conID] = cm.gameObject;

            //You know me
            cm.ConnectWith(this, (conID + 3) % 6);
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
        // Reset Substance Deltas
        diffusionDelta = new Juice();

        // Calculation of new Substance Deltas 0, 1, 2        
        for (int i = 0; i < 3; i++)
        {
            //Calculation only for the first three connections. Reason:
            //The diffusion delta for each connection only needs to be calculated from one side
             
            if (connections[i] != null)
            {
                Juice buf = connections[i].GetComponent<CellManager>().juice;
                Juice otherDifDelta = connections[i].GetComponent<CellManager>().diffusionDelta;
                Juice delta = new Juice()
                {
                    //Calculate deltas
                    red         = (juice.red - buf.red)                 * diffusionFactor * 0.1666f,
                    green       = (juice.green - buf.green)             * diffusionFactor * 0.1666f,
                    blue        = (juice.blue - buf.blue)               * diffusionFactor * 0.1666f,
                    blueCharged = (juice.blueCharged - buf.blueCharged) * diffusionFactor * 0.1666f,
                    yellow      = (juice.yellow - buf.yellow)           * diffusionFactor * 0.1666f,
                    black       = (juice.black - buf.black)             * diffusionFactor * 0.1666f
                };

                //Add deltas to diffusionDeltas
                diffusionDelta.red          -= delta.red;
                diffusionDelta.green        -= delta.green;
                diffusionDelta.blue         -= delta.blue;
                diffusionDelta.blueCharged  -= delta.blueCharged;
                diffusionDelta.yellow       -= delta.yellow;
                diffusionDelta.black        -= delta.black;

                otherDifDelta.red           += delta.red;
                otherDifDelta.green         += delta.green;
                otherDifDelta.blue          += delta.blue;
                otherDifDelta.blueCharged   += delta.blueCharged;
                otherDifDelta.yellow        += delta.yellow;
                otherDifDelta.black         += delta.black;
            }
        }
        
    }

    //Juice Diffusion Apply
    public void DiffusionApply()
    {
        //deltaTime sollte niemals > 1.0f sein, sonst sind schwingeffekte möglich
        juice.red           += Time.deltaTime * diffusionDelta.red;
        juice.green         += Time.deltaTime * diffusionDelta.green;
        juice.blue          += Time.deltaTime * diffusionDelta.blue;
        juice.blueCharged   += Time.deltaTime * diffusionDelta.blueCharged;
        juice.yellow        += Time.deltaTime * diffusionDelta.yellow;
        juice.black         += Time.deltaTime * diffusionDelta.black;
    }
}
