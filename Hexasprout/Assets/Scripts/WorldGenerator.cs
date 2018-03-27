using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public GameObject prefabField;
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
                GameObject newField = Instantiate(prefabField);
                newField.transform.SetParent(transform);
                newField.transform.position = new Vector2(-100.0f + (float) i * 1.5f + (float)(j % 2) * 0.75f, -50 + 0.25f * j);
                newField.GetComponent<FieldManager>().idx = i;
                newField.GetComponent<FieldManager>().idy = j;
                fields[i][j] = newField;
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
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}
