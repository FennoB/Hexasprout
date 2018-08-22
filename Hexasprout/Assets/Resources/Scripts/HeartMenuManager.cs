using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartMenuManager : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        GUIManager gm = GetComponentInParent<GUIManager>();
        if(gm.CellMenuTarget.Cell.GetComponent<CellManager>().cellType == CellType.Heartcell)
        {
            HeartCellSpec hsp = gm.CellMenuTarget.Cell.GetComponent<HeartCellSpec>();
            
            // Get values
            transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Slider>().value = hsp.heartspeed / hsp.maxspeed;
            transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Text>().text = hsp.maxspeed.ToString();
            transform.GetChild(2).gameObject.GetComponent<UnityEngine.UI.Text>().text = hsp.heartspeed.ToString();
            transform.GetChild(3).gameObject.GetComponent<UnityEngine.UI.Text>().text = "LIMIT\n\n\n" + (hsp.heartspeed * 1.2).ToString();

            transform.GetChild(6).rotation = Quaternion.identity;
            int dir = (hsp.pumpdirection + 3) % 6;
            transform.GetChild(6).Rotate(0, 0, 60 * (dir - 1));

            transform.GetChild(6).gameObject.GetComponent<Animator>().SetBool("pump", hsp.pumping);
        }
    }

    void ChangeValue(float v)
    {
        HeartCellSpec hsp = GetComponentInParent<GUIManager>().CellMenuTarget.Cell.GetComponent<HeartCellSpec>();
        hsp.heartspeed = v * hsp.maxspeed;
    }
}
