using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FieldState 
{
    Invisible = 0,
    Visible,
    HasCell, 
    Selected,
    Grow,
    Decompose,
    SuperSelected
}


public class FieldManager : MonoBehaviour
{
    public FieldState state = FieldState.Invisible;
    //0 = Down left
    //1 = Down
    //... (circle)

    public FieldManager[] neighbours;
    public int idx, idy;


    private GameObject cell = null;
    public GameObject material = null;

    public float warmth;

    


    //public bool occupied = false;
    //private bool selected = false;//true if field is selected by double click
    //private bool neighbourSelected = false;// true if a neighbourfield is selected

    // Use this for initialization
    void Start ()
    {
    }

	// Update is called once per frame
    void Update()
    {
        if (State <= FieldState.HasCell)
        { 
            CheckState();
        }
        SetApperance();
	}

    public void ConnectWithFieldAsNB(FieldManager m, int conID)
    {
        if(conID >= 0 && conID <= 5 && m != null)
        {
            if(neighbours[conID] == null)
            {
                neighbours[conID] = m;
                m.ConnectWithFieldAsNB(this, (conID + 3) % 6);  //Gegenverbindung anfordern
            }
        }
    }

    public void CheckState()
    {
        if (Cell != null )
        {
            State = FieldState.HasCell;
        }

        if (State != FieldState.HasCell)
        {
            State = FieldState.Invisible;
            for (int i = 0; i < neighbours.Length; i++)
            {
                if (neighbours[i] != null)
                {
                    if (neighbours[i].GetComponent<FieldManager>().Cell != null)
                    {
                        State = FieldState.Visible;
                    }
                }
            }
        }
    }

    public void SetApperance()
    {
        switch (State)
        {
            case FieldState.Invisible:
                GetComponent<SpriteRenderer>().color = new Color(0.37f * warmth, 0.17f, 0.37f - 0.37f * warmth);
                if (HasMaterial())
                {
                    material.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f);
                }
                break;
            case FieldState.Visible:
                GetComponent<SpriteRenderer>().color = new Color(0.05f + 0.5f * warmth, 0.3f, 0.55f - 0.5f * warmth);
                if (HasMaterial())
                {
                    material.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f);
                }
                break;
            case FieldState.HasCell:
                GetComponent<SpriteRenderer>().color = new Color(warmth, 0.5f, 1.0f - warmth);
                if (HasMaterial())
                {
                    material.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
                }
                break;
            case FieldState.Selected:
                GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 1f);
                break;
            case FieldState.Grow:
                GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0f);
                break;
            case FieldState.Decompose:
                GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f);
                break;

        }
    }

    public bool HasMaterial()
    {
        if (Material != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public FieldManager[] GetNeighbours()
    {
        return neighbours;
    }
    public int GetIdx()
    {
        return idx;
    }
    public int GetIdy()
    {
        return idy;
    }
    public FieldState State
    {
        get
        {
            return state;
        }

        set
        {
            state = value;
        }
    }

    public GameObject Cell
    {
        get
        {
            return cell;
        }

        set
        {
            cell = value;
        }
    }

    public GameObject Material
    {
        get
        {
            return material;
        }

        set
        {
            material = value;
        }
    }
}
