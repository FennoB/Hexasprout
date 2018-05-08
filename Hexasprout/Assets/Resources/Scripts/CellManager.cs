using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CellManager : MonoBehaviour
{
    public float energy = 1.0f;        // Energy battery
    public float energyMax = 1.0f;     // Battery size
    public float energyUse = 0.2f;     // Usage per minute (Death after 5 minutes without supply)
    public bool alive = true;
    public GameObject[] connections;

	// Use this for initialization
	void Start ()
    {
        connections = new GameObject[6];
        for(int i = 0; i < 6; i++)
        {
            connections[i] = null;
        }
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

    // Connect
    void ConnectWith(CellManager cm, int conID)
    {
        // Already connected?
        if (connections[conID] == null)
        {
            //No. Connect!
            //I know you
            connections[conID] = cm.gameObject;

            //You know me
            cm.ConnectWith(this, (conID + 3) % 6);
        }
    }

    // Death
    void Death()
    {
        energy = 0;
        alive = false;
    }
}
