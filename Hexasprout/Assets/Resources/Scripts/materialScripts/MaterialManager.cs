using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour {

    private int load;

    private void Awake()
    {
        load = (int)Random.Range(0f, 100f);
    }
    public int GetLoad()
    {
        return load;
    }
    public void DecreaseLoad(int take)
    {
        load = load - take;
    }
    public bool CheckLoadEmptyAfterTake(int take)
    {
        if (load - take < 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
