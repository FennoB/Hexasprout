using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public int worldSizeX = 5;
    public int worldSizeY = 20;
    public GameObject[][] fields;
    public int probResource = 5;
    public int probRed = 10;
    public int probBlue = 33;
    public int probBlueCharged = 5;
    public int probGreen = 10;
    public int probYellow = 8;
    public int probBlack = 34;

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
                newField.transform.position = new Vector2(-100.0f + (float) i * 1.5f + (float)(j % 2) * 0.75f, -50 + 0.5f * j / 1.1428f);
                newField.GetComponent<FieldManager>().idx = i;
                newField.GetComponent<FieldManager>().idy = j;
                newField.GetComponent<FieldManager>().warmth = Random.value;
                fields[i][j] = newField;

                // Resource?
                if (Random.Range(0, 100) < 10)
                {
                    // Resource!
                    int resclass = Random.Range(0, 100);
                    int classBlack = probBlack;
                    int classBlue = classBlack + probBlue;
                    int classRed = classBlue + probRed;
                    int classGreen = classRed + probGreen;
                    int classYellow = classGreen + probYellow;
                    int classBluecharged = classYellow + probBlueCharged;
                    
                    if(resclass < classBlack)
                    {
                        CreateMaterial(newField.GetComponent<FieldManager>(), "black");
                    }
                    else if (resclass < classBlue)
                    {
                        CreateMaterial(newField.GetComponent<FieldManager>(), "blue");
                    }
                    else if (resclass < classRed)
                    {
                        CreateMaterial(newField.GetComponent<FieldManager>(), "red");
                    }
                    else if (resclass < classGreen)
                    {
                        CreateMaterial(newField.GetComponent<FieldManager>(), "green");
                    }
                    else if (resclass < classYellow)
                    {
                        CreateMaterial(newField.GetComponent<FieldManager>(), "yellow");
                    }
                    else if (resclass < classBluecharged)
                    {
                        CreateMaterial(newField.GetComponent<FieldManager>(), "bluecharged");
                    }
                }
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
        CreateStorageCell(fields[2][2].GetComponent<FieldManager>());
        CreateWorkerCell(fields[2][1].GetComponent<FieldManager>());
        //Hard code Material
        CreateMaterial(fields[2][3].GetComponent<FieldManager>(), "red");
        
    }

    public void CreateMaterial(FieldManager fm, string type)
    {
        if (fm != null && fm.Cell == null && fm.material == null)
        {
            GameObject p = (GameObject)Resources.Load("Prefabs/Materials/" + type, typeof(GameObject));
            GameObject g = Instantiate(p);
            g.GetComponent<Transform>().SetParent(fm.gameObject.GetComponent<Transform>());
            g.GetComponent<Transform>().localPosition = new Vector3(0, 0, -0.14f);
            fm.Material = g;
        }
        else
        {
            // throw Exception
        }
    }

    public void CreateWorkerCell(FieldManager fm)
    {
        if (fm != null || fm.Cell == null)
        {
            GameObject p = (GameObject)Resources.Load("Prefabs/Cells/WorkerCell", typeof(GameObject));
            GameObject g = Instantiate(p);
            g.GetComponent<Transform>().SetParent(fm.gameObject.GetComponent<Transform>());
            g.GetComponent<Transform>().localPosition = new Vector3(0, 0, -0.14f);
            fm.Cell = g;
        }
        else
        {
            // throw Exception
        }
    }

    public bool CreateStemCell(FieldManager fm)
    {
        // Fieldmanager valid?
        if(fm == null || fm.Cell != null || fm.Material != null)
        {
            // Nope
            return false;
        }

        // Wheee
        // Load Prefab
        GameObject p = (GameObject)Resources.Load("Prefabs/Cells/StemCell", typeof(GameObject));

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
        fm.Cell = g;
        return true;
    }

    public bool CreateStorageCell(FieldManager fm)
    {
        // Fieldmanager valid?
        if (fm == null || fm.Cell != null || fm.Material != null)
        {
            // Nope
            return false;
        }

        // Wheee
        // Load Prefab
        GameObject p = (GameObject)Resources.Load("Prefabs/Cells/StorageCell", typeof(GameObject));

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
        fm.Cell = g;
        return true;
    }


    // Fixed update is called at fixed timestep
    private void FixedUpdate()
    {
        GameObject[] cells = GameObject.FindGameObjectsWithTag("Cell");

        foreach (GameObject c in cells)
        {
            CellManager cm = c.GetComponent<CellManager>();
            cm.OwnFixedUpdate();
            c.GetComponent<BuildManager>().OwnFixedUpdate();
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
