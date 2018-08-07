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
    Decompose
}


public class FieldManager : MonoBehaviour
{
    private FieldState state = FieldState.Invisible;
    //0 = Down left
    //1 = Down
    //... (circle)

    public GameObject[] neighbours;
    public int idx, idy;
    
    
    public GameObject cell = null;
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
                neighbours[conID] = m.gameObject;
                m.ConnectWithFieldAsNB(this, (conID + 3) % 6);  //Gegenverbindung anfordern
            }
        }
    }

    public void CheckState()
    {
        if (cell != null )
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
                    if (neighbours[i].GetComponent<FieldManager>().State == FieldState.HasCell)
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
                GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f);
                break;
            case FieldState.Visible:
                GetComponent<SpriteRenderer>().color = new Color(0.5f * warmth, 0.25f, 0.5f - 0.5f * warmth);
                break;
            case FieldState.HasCell:
            GetComponent<SpriteRenderer>().color = new Color(warmth, 0.5f, 1.0f - warmth);
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
    public GameObject[] GetNeighbours()
    {
        return neighbours;
    }
    public GameObject GetCell()
    {
        return cell;
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
}
