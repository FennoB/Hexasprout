using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    public int state = 0;
    public GameObject[] neighbours;
    public int idx, idy;

    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update ()
    {

        CheckState();
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
            GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f);
        }
        else if (state == 1)
        {
            GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
        }
        else if(state == 0) 
        {
            GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f);
        }
    }

    public int GetState()
    {
        return state;
    }
}
