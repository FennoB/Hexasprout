using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CellManager : MonoBehaviour
{
    public float energy = 1.0f;        // Energy battery
    public float energyMax = 1.0f;     // Battery size
    public float energyUse = 0.2f;     // Usage per minute (Death after 5 minutes without supply)
    public bool alive = true;

	// Use this for initialization
	void Start ()
    {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (alive)
        {
            AbsorbEnergy();
            UseEnergy();
        }
    }

    // Absorb energy from blood
    void AbsorbEnergy()
    {
        //TODO
    }

    // Use energy for living
    void UseEnergy()
    {
        energy -= energyUse * (Time.deltaTime / 60.0f);     // Usage = energyUse * deltaTime in minutes
        if(energy <= 0)
        {
            Death();
        }
    }

    // Death
    void Death()
    {
        alive = false;
    }
}
