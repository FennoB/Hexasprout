using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public int worldSizeX = 5;
    public int worldSizeY = 20;
    public GameObject[][] fields;

	// Use this for initialization
	void Start ()
    {
        fields = new GameObject[worldSizeX][];
        for(int i = 0; i < worldSizeX; i++)
        {
            fields[i] = new GameObject[worldSizeY];
        }

        for (int i = 0; i < worldSizeX; i++)
        {
            for (int j = 0; j < worldSizeY; j++)
            {
                GameObject newField = Instantiate((GameObject)Resources.Load("Prefabs/Field", typeof(GameObject)));
                newField.transform.SetParent(transform);
                newField.transform.position = new Vector2(-100.0f + (float) i * 1.5f + (float)(j % 2) * 0.75f, -50 + 0.25f * j);
                newField.GetComponent<FieldManager>().idx = i;
                newField.GetComponent<FieldManager>().idy = j;
                newField.GetComponent<FieldManager>().warmth = Random.value;
                fields[i][j] = newField;

                //// Resource?
                //if(Random.Range(0, 100) < 5)
                //{
                //    // Resource!
                //    int resclass = Random.Range(0, 5);
                //    if (resclass)
                //}
            }
        }

        for (int i = 0; i < worldSizeX; i++)
        {
            for (int j = 0; j < worldSizeY; j++)
            {
                //Get FieldManager
                FieldManager fm = fields[i][j].GetComponent<FieldManager>();

                //Neighbour FieldManager for connection
                FieldManager fmNB;

                //Up left
                int nbx = i - (1 - (j % 2));
                int nby = j - 1;

                //Check if in world
                if(nbx >= 0 && nby >= 0)
                {
                    //Get nb Fieldmanager for connection
                    fmNB = fields[nbx][nby].GetComponent<FieldManager>();
                    fm.ConnectWithFieldAsNB(fmNB, 0);
                }

                //Up
                nbx = i;
                nby = j - 2;

                //Check if in world
                if (nbx >= 0 && nby >= 0)
                {
                    //Get nb Fieldmanager for connection
                    fmNB = fields[nbx][nby].GetComponent<FieldManager>();
                    fm.ConnectWithFieldAsNB(fmNB, 1);
                }

                //Up right
                nbx = i + (j % 2);
                nby = j - 1;

                //Check if in world
                if (nbx < worldSizeX && nby >= 0)
                {
                    //Get nb Fieldmanager for connection
                    fmNB = fields[nbx][nby].GetComponent<FieldManager>();
                    fm.ConnectWithFieldAsNB(fmNB, 2);
                }
            }
        }

        // Hard code dummy cell
        CreateStemCell(fields[2][2].GetComponent<FieldManager>());
        
    }
    
    public bool CreateStemCell(FieldManager fm)
    {
        // Fieldmanager valid?
        if(fm == null || fm.cell != null)
        {
            // Nope
            return false;
        }

        // Wheee
        // Load Prefab
        GameObject p = (GameObject)Resources.Load("Prefabs/StemCell", typeof(GameObject));

        // Prefab valid?
        if (p == null)
        {
            // Nope
            return false;
        }

        // Wheee
        // Intantiate a Gameobject of this Prefab
        GameObject g = Instantiate(p);

        // Gameobject valid?
        if (g == null)
        {
            // Nope
            return false;
        }

        // Wheee
        // Link with field
        g.GetComponent<Transform>().SetParent(fm.gameObject.GetComponent<Transform>());
        g.GetComponent<Transform>().localPosition = new Vector3(0, 0, -0.14f);
        fm.cell = g;
        return true;
    }

    // Fixed update is called at fixed timestep
    private void FixedUpdate()
    {
        GameObject[] cells = GameObject.FindGameObjectsWithTag("Cell");

        foreach (GameObject c in cells)
        {
            if(c.GetComponent<LeafCellSpec>() != null)
            {
                c.GetComponent<LeafCellSpec>().Absorb();
            }

            CellManager cm = c.GetComponent<CellManager>();
            cm.OwnFixedUpdate();
        }

        foreach (GameObject c in cells)
        {
            CellManager cm = c.GetComponent<CellManager>();
            cm.DiffusionCalc();
        }

        foreach (GameObject c in cells)
        {
            CellManager cm = c.GetComponent<CellManager>();
            cm.DiffusionApply();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public GameObject[][] GetFields()
    {
        return fields;
    }
}
