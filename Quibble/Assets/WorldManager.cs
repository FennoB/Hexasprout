using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {

    public GameObject blockPrefab;

	// Use this for initialization
	void Start ()
    {
        GetComponent<MeshRenderer>().material.color = Color.black;

        //Zwanzig Blocks erstellen
        for (int i = 0; i < 20; i++)
        {
            GameObject block = GameObject.Instantiate(blockPrefab);
            block.transform.position = new Vector3(Random.Range(-transform.localScale.x / 2.0f + 0.5f, transform.localScale.x / 2.0f - 0.5f), Random.Range(-transform.localScale.y / 2.0f + 0.5f, transform.localScale.y / 2.0f - 0.5f), -2.0f);
            Material materialColored = new Material(Shader.Find("Diffuse"));
            int colclass = Random.Range(0, 8);
            if(colclass == 0)
            {
                materialColored.color = new Color(0.25f, 0.25f, 0.25f);
            }
            else
            {
                materialColored.color = new Color((int)(colclass / 4), 0.5f * (float)((colclass - colclass % 2) % 4), colclass % 2);
            }

            block.GetComponent<BlockManager>().colorClass = colclass;
            block.GetComponent<MeshRenderer>().material = materialColored;
            block.GetComponent<Rigidbody>().velocity = new Vector3(Random.value, Random.value, 0);
            block.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void reload()
    {
        GameObject[] list = GameObject.FindGameObjectsWithTag("block");
        foreach(GameObject block in list)
        {
            block.transform.position = new Vector3(Random.Range(-transform.localScale.x / 2.0f + 0.5f, transform.localScale.x / 2.0f - 0.5f), Random.Range(-transform.localScale.y / 2.0f + 0.5f, transform.localScale.y / 2.0f - 0.5f), -2.0f);
            Material materialColored = new Material(Shader.Find("Diffuse"));
            int colclass = Random.Range(0, 8);
            if (colclass == 0)
            {
                materialColored.color = new Color(0.25f, 0.25f, 0.25f);
            }
            else
            {
                materialColored.color = new Color((int)(colclass / 4), 0.5f * (float)((colclass - colclass % 2) % 4), colclass % 2);
            }

            block.GetComponent<BlockManager>().colorClass = colclass;
            block.GetComponent<MeshRenderer>().material = materialColored;
            block.GetComponent<Rigidbody>().velocity = new Vector3(Random.value, Random.value, 0);
            block.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }
}
