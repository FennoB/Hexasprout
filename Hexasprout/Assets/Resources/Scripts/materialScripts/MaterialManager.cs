﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : MonoBehaviour {

    public enum Type {black, red, yellow, green, blue}

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
    public void DecreaseLoad(float take)
    {
        load = load - take * Time.deltaTime;
    }
    public bool LoadEmptyAfterTake(float take)
    {
        if (load - take < 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void SetType(Type type)
    {
        this.type = type;
    }
}
