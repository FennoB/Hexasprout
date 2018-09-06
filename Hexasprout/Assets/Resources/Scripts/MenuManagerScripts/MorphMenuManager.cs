using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MorphMenuManager : MonoBehaviour
{
    public void OpenCustomizedImprovePanel(string title, GUI_Event e, Juice juice, float seconds, GUIManager gm)
    {
        gameObject.SetActive(true);

        //getting things in the Cache
        GetComponent<JobCache>().juice = juice;
        GetComponent<JobCache>().title = title;
        GetComponent<JobCache>().seconds = seconds;

        //showing things right on the panel
        transform.GetChild(2).gameObject.GetComponent<UnityEngine.UI.Text>().text = title;
        transform.GetChild(3).gameObject.GetComponent<ButtonScript>().ButtonID = e;
        transform.GetChild(8).gameObject.GetComponent<UnityEngine.UI.Text>().text = seconds.ToString();

        UnityEngine.UI.Image newCell = transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Image>();
        switch (e)
        {
            case GUI_Event.Morph2Heart:
                newCell.sprite = GetComponent<ImageSelectorImprovementMenu>().images[0];
                break;
            case GUI_Event.Morph2Leaf:
                newCell.sprite = GetComponent<ImageSelectorImprovementMenu>().images[1];
                break;
            case GUI_Event.Morph2Storage:
                newCell.sprite = GetComponent<ImageSelectorImprovementMenu>().images[2];
                break;
            case GUI_Event.Morph2Worker:
                newCell.sprite = GetComponent<ImageSelectorImprovementMenu>().images[3];
                break;
        }

        SetNecessaryMaterials(juice);

        gm.ResetSliderButtons();
        gm.ResetSelectedCells();
    }

    void SetNecessaryMaterials(Juice juice)
    {
        bool firstSlotUsed = false;
        bool secondSlotUsed = false;

        for (int i = 0; i < juice.Length; i++)
        {
            if (juice[i] != 0)
            {
                if (!firstSlotUsed)
                {
                    firstSlotUsed = true;
                    transform.GetChild(4).GetComponent<UnityEngine.UI.Image>().sprite = transform.GetChild(4).GetComponent<ImageSelector>().Sprites[i];
                    transform.GetChild(5).GetComponent<UnityEngine.UI.Text>().text = juice[i].ToString();

                }
                else
                {
                    secondSlotUsed = true;
                    transform.GetChild(6).GetComponent<UnityEngine.UI.Image>().sprite = transform.GetChild(6).GetComponent<ImageSelector>().Sprites[i];
                    transform.GetChild(7).GetComponent<UnityEngine.UI.Text>().text = juice[i].ToString();
                }
            }
        }
        if (!firstSlotUsed)
        {
            transform.GetChild(4).gameObject.SetActive(false);
            transform.GetChild(5).gameObject.SetActive(false);
        }
        if (!secondSlotUsed)
        {
            transform.GetChild(6).gameObject.SetActive(false);
            transform.GetChild(7).gameObject.SetActive(false);
        }
    }
}
