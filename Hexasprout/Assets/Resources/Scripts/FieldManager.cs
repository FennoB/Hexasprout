using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    public int state = 0;
    //0 = Down left
    //1 = Down
    //... (circle)

    public GameObject[] neighbours;
    public int idx, idy;
    public GameObject cell;
    public GameObject resource;
    public float warmth;

    private bool selected = false;//true if field is selected by double click
    private bool neighbourSelected = false;// true if a neighbourfield is selected

    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update ()
    {
        CheckState();
        SetApperance();
        SetSelectedApperance();
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
        if (cell != null)
        {
            state = 2;
        }

        if (state != 2)
        {
            state = 0;
            for (int i = 0; i < neighbours.Length; i++)
            {
                if (neighbours[i] != null)
                {
                    if (neighbours[i].GetComponent<FieldManager>().GetState() == 2)
                    {
                        state = 1;
                    }
                }
            }
        }
    }

    public void SetApperance()
    {
        if (state == 2)
        {
            GetComponent<SpriteRenderer>().color = new Color(warmth, 0.5f, 1.0f - warmth);
        }
        else if (state == 1)
        {
            GetComponent<SpriteRenderer>().color = new Color(0.5f * warmth, 0.25f, 0.5f - 0.5f * warmth);
        }
        else if(state == 0) 
        {
            GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f);
        }
    }
    /**
     *field is more green if it is selected
     *field is more yellow if it is a neighbour of a selected field
     */
    public void SetSelectedApperance()
    {
        if (selected)
        {
            GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0f);
        }
        if (neighbourSelected)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 0f);
        }
    }

    public void SetSelected(bool selected)
    {
        this.selected = selected;
    }
    public void SetNeighbourSelected(bool neighbourSelected)
    {
        this.neighbourSelected = neighbourSelected;
    }
    public int GetState()
    {
        return state;
    }
    public bool GetSelected()
    {
        return selected;
    }
    public bool GetNeighbourSelected()
    {
        return neighbourSelected;
    }
    public GameObject[] GetNeighbours()
    {
        return neighbours;
    }
}
