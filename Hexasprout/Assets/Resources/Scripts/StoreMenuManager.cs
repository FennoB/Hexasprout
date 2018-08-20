using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Makes it easier for to acess the store menu
public class StoreMenuManager : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        GameObject targetCell = GetComponentInParent<GUIManager>().CellMenuTarget.Cell;
		if(targetCell.GetComponent<CellManager>().cellType == CellType.Storagecell)
        {
            // This is a storage cell!
            // Read values and adjust menu
            StorageCellSpec s = targetCell.GetComponent<StorageCellSpec>();

            // This giga super duper loopy fluu can generate all the Text contents we need :o
            // If you want to understand it, here we go:
            // This is for every mineral in the juice
            for (int j = 0; j < 6; j++)
            {
                // This is for every text we have to change for this mineral
                for (int i = 0; i < 4; i++)
                {
                    float value = 0.0f;

                    // Every i corresponds to a text UI Element. Here we get the value:
                    switch (i)
                    {
                        case 0:
                            value = s.capacities[j];
                            break;
                        case 1:
                            value = s.target[j];
                            break;
                        case 2:
                            value = s.content[j];
                            break;
                        case 3:
                            value = s.target[j] - s.content[j];
                            break;
                    }

                    // And now, with the child structure, we set the text to the value
                    transform.GetChild(j).GetChild(i).gameObject.GetComponent<UnityEngine.UI.Text>().text = value.ToString();
                }

                // Now Adjust the Slider value:
                transform.GetChild(j).gameObject.GetComponent<UnityEngine.UI.Slider>().value = s.target[j] / s.capacities[j];
            }

            // Next Level: (TODO)
            transform.GetChild(6).gameObject.GetComponent<UnityEngine.UI.Text>().text = "LIMIT\n\n\n" + "7.0";
        }
	}

    // Get new values from the sliders
    public void ChangeValueRed(float v)
    {
        StorageCellSpec s = GetComponentInParent<GUIManager>().CellMenuTarget.Cell.GetComponent<StorageCellSpec>();
        s.target.red = v * s.capacities.red;
    }

    public void ChangeValueGreen(float v)
    {
        StorageCellSpec s = GetComponentInParent<GUIManager>().CellMenuTarget.Cell.GetComponent<StorageCellSpec>();
        s.target.green = v * s.capacities.green;
    }

    public void ChangeValueBlue(float v)
    {
        StorageCellSpec s = GetComponentInParent<GUIManager>().CellMenuTarget.Cell.GetComponent<StorageCellSpec>();
        s.target.blue = v * s.capacities.blue;
    }

    public void ChangeValueBlueCharged(float v)
    {
        StorageCellSpec s = GetComponentInParent<GUIManager>().CellMenuTarget.Cell.GetComponent<StorageCellSpec>();
        s.target.blueCharged = v * s.capacities.blueCharged;
    }

    public void ChangeValueYellow(float v)
    {
        StorageCellSpec s = GetComponentInParent<GUIManager>().CellMenuTarget.Cell.GetComponent<StorageCellSpec>();
        s.target.yellow = v * s.capacities.yellow;
    }

    public void ChangeValueBlack(float v)
    {
        StorageCellSpec s = GetComponentInParent<GUIManager>().CellMenuTarget.Cell.GetComponent<StorageCellSpec>();
        s.target.black = v * s.capacities.black;
    }
}
