using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour {

    public enum Type {black, red, yellow, green, blue, bluecharged}

    public float load;
    public Type type;

    private void Awake()
    {
        load = (int)Random.Range(0f, 100f);
    }
    public float GetLoad()
    {
        return load;
    }
    public float Take(float take)
    {
        if(take > load)
        {
            load = 0;
            return load;
        }
        else
        {
            load -= take;
            return take;
        }
    }
    public void SetType(Type type)
    {
        this.type = type;
    }
}
