using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenuManager : MonoBehaviour {

    BuildManager bm;
    GUIManager gm;

    bool firstSlotUsed= false;
    bool secondSlotUsed = false;
    bool thirdSlotUsed = false;

    int juiceUsed1 = 0;
    int juiceUsed2 = 0;
    int juiceUsed3 = 0;

    // Use this for initialization
    void Start ()
    {
        gm = GameObject.FindGameObjectWithTag("GUI").GetComponent<GUIManager>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (gm.CellMenuTarget != null && gm.CellMenuTarget.Cell != null)
        {
            bm = gm.CellMenuTarget.Cell.GetComponent<BuildManager>();
            if (bm.buildFlag)
            {
                bm = gm.CellMenuTarget.Cell.GetComponent<BuildManager>();
                if (firstSlotUsed)
                {
                    transform.GetChild(2).GetComponent<Slider>().value = bm.cellBuildCache[juiceUsed1] / bm.goalJuice[juiceUsed1];
                }
                if (secondSlotUsed)
                {
                    transform.GetChild(5).GetComponent<Slider>().value = bm.cellBuildCache[juiceUsed2] / bm.goalJuice[juiceUsed2];
                }
                if (thirdSlotUsed)
                {
                    transform.GetChild(8).GetComponent<Slider>().value = bm.cellBuildCache[juiceUsed3] / bm.goalJuice[juiceUsed3];
                }
            }
        }
	}

    public void SetMenuUp(Juice juice, string title)
    {
        transform.GetChild(0).GetComponent<Text>().text = title;

        firstSlotUsed = false;
        secondSlotUsed = false;
        thirdSlotUsed = false;

        for (int i = 0; i < juice.Length; i++)
        {
            if (juice[i] != 0)
            {
                if (secondSlotUsed && !thirdSlotUsed)
                {
                    thirdSlotUsed = true;
                    transform.GetChild(7).GetComponent<Image>().sprite = GetComponent<ImageSelector>().Sprites[i];
                    transform.GetChild(8).GetComponent<Slider>().value = 0;
                    transform.GetChild(9).GetComponent<Text>().text = juice[i].ToString();

                    juiceUsed3 = i;
                }
                if (firstSlotUsed && !secondSlotUsed)
                {
                    secondSlotUsed = true;
                    transform.GetChild(4).GetComponent<Image>().sprite = GetComponent<ImageSelector>().Sprites[i];
                    transform.GetChild(5).GetComponent<Slider>().value = 0;
                    transform.GetChild(6).GetComponent<Text>().text = juice[i].ToString();

                    juiceUsed2 = i;
                }
                if (!firstSlotUsed)
                {
                    firstSlotUsed = true;
                    transform.GetChild(1).GetComponent<Image>().sprite = GetComponent<ImageSelector>().Sprites[i];
                    transform.GetChild(2).GetComponent<Slider>().value = 0;
                    transform.GetChild(3).GetComponent<Text>().text = juice[i].ToString();

                    juiceUsed1 = i;
                }
            }
        }
        if (!firstSlotUsed)
        {
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(false);
        }
        if (!secondSlotUsed)
        {
            transform.GetChild(4).gameObject.SetActive(false);
            transform.GetChild(5).gameObject.SetActive(false);
            transform.GetChild(6).gameObject.SetActive(false);
        }
        if (!thirdSlotUsed)
        {
            transform.GetChild(7).gameObject.SetActive(false);
            transform.GetChild(8).gameObject.SetActive(false);
            transform.GetChild(9).gameObject.SetActive(false);
        }
    }
}
