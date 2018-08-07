using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour {

    public enum Type {black, red, yellow, green, blue}

    private int load;
    public Type type;

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
    public void SetType(Type type)
    {
        this.type = type;
    }
}
