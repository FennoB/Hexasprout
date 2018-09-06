using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImprovementMenuManager : MonoBehaviour
{

    public void OpenCustomizedImprovePanel(float value, float dif, string title, GUI_Event e, Juice juice, float seconds, GUIManager gm)
    {
        gameObject.SetActive(true);

        //getting things in the Cache
        gameObject.GetComponent<JobCache>().juice = juice;
        gameObject.GetComponent<JobCache>().dif = dif;
        gameObject.GetComponent<JobCache>().title = title;
        gameObject.GetComponent<JobCache>().seconds = seconds;

        //showing things right on the panel
        gameObject.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = value.ToString();
        gameObject.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Text>().text = (value + dif).ToString();
        gameObject.transform.GetChild(2).gameObject.GetComponent<UnityEngine.UI.Text>().text = title;
        gameObject.transform.GetChild(3).gameObject.GetComponent<ButtonScript>().ButtonID = e;
        gameObject.transform.GetChild(8).gameObject.GetComponent<UnityEngine.UI.Text>().text = seconds.ToString();

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
