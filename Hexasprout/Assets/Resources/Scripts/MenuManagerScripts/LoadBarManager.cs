using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadBarManager : MonoBehaviour {

    public Animator a;
    [Range(0f, 0.999f)]
    public float progress;

    public void SetPicture(Sprite s)
    {
        if(s == null)
        {
            return;
        }

        transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = s;
    }

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        a.Play("LoadBar", 0, progress);
    }
}
