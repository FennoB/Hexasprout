using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour {

    public int colorClass = 0;      //0 is grey
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector3 scale = FindObjectOfType<WorldManager>().transform.localScale;

        Vector3 velocity = GetComponent<Rigidbody>().velocity;

        Vector3 pos = transform.position;

        if (pos.x > scale.x / 2 - 0.5f)
        {
            pos.x = scale.x / 2 - 0.5f;
            velocity = new Vector3(-velocity.x, velocity.y, velocity.z) * 1.3f;
        }
        else if (pos.x < - scale.x / 2 + 0.5f)
        {
            pos.x = -scale.x / 2 + 0.5f;
            velocity = new Vector3(-velocity.x, velocity.y, velocity.z) * 1.3f;
        }
        if (pos.y > scale.y / 2 - 0.5f)
        {
            pos.y = scale.y / 2 - 0.5f;
            velocity = new Vector3(velocity.x, -velocity.y, velocity.z) * 1.3f;
        }
        else if (pos.y < - scale.y / 2 + 0.5f)
        {
            pos.y = -scale.y / 2 + 0.5f;
            velocity = new Vector3(velocity.x, -velocity.y, velocity.z) * 1.3f;
        }


        transform.position = pos;
        GetComponent<Rigidbody>().velocity = velocity;
    }
}
