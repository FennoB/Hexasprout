using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuibbleManager : MonoBehaviour {

    public float speed = 0.5f;
    bool deathMenu = false;
    public float yRotation = 0;
    public float xRotation = 0;
    public GameObject deathtext = null;
    public GameObject scoretext = null;

    int colorClass = 0;

    float score = 0;

    // The ID of the touch that began the scroll.
    int ScrollTouchID = -1;
    // The position of that initial touch
    Vector2 ScrollTouchOrigin;



    // Use this for initialization
    void Start ()
    {
        deathMenu = false;
        yRotation = 0;
        xRotation = 0;
        Input.gyro.enabled = true;

        Material materialColored = new Material(Shader.Find("Diffuse"));

        int colclass = 0; // Random.Range(1, 7);
        if (colclass == 0 || true)
        {
            materialColored.color = new Color(1, 0, 0);
        }
        else
        {
            materialColored.color = new Color((int)(colclass / 4), 0.5f * (float)((colclass - colclass % 2) % 4), colclass % 2);
        }

        colorClass = colclass;
        GetComponent<MeshRenderer>().material = materialColored;
    }

    void OnCollisionEnter(Collision col)
    {
        BlockManager bm = col.gameObject.GetComponent<BlockManager>();
        
        if (bm.colorClass == colorClass || colorClass == 0 || bm.colorClass == 0)
        {
            score += 1000;

            Material materialColored = new Material(Shader.Find("Diffuse"));

            int colclass = Random.Range(0, 8);

            if (bm.colorClass == 0)
            {
                score += 2000;
                colclass = 0;
            }

            if (colclass == 0 || true)
            {
                materialColored.color = new Color(1, 0, 0);
            }
            else
            {
                materialColored.color = new Color((int)(colclass / 4), 0.5f * (float)((colclass - colclass % 2) % 4), colclass % 2);
            }

            colorClass = colclass;
            GetComponent<MeshRenderer>().material = materialColored;

        }
        else
        {
            // Report death
            deathMenu = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!deathMenu)
        {
            score += Time.deltaTime * 100;
            deathtext.SetActive(false);
            scoretext.GetComponent<UnityEngine.UI.Text>().text = "Score: " + (int)score;

            foreach (Touch T in Input.touches)
            {
                //Note down the touch ID and position when the touch begins...
                if (T.phase == TouchPhase.Began)
                {
                    if (ScrollTouchID == -1)
                    {
                        ScrollTouchID = T.fingerId;
                        ScrollTouchOrigin = T.position;
                    }
                }
                //Forget it when the touch ends
                if ((T.phase == TouchPhase.Ended) || (T.phase == TouchPhase.Canceled))
                {
                    ScrollTouchID = -1;
                }
                if (T.phase == TouchPhase.Moved)
                {
                    //If the finger has moved and it's the finger that started the touch, move the camera along the Y axis.
                    if (T.fingerId == ScrollTouchID)
                    {
                        xRotation += T.deltaPosition.x;
                        yRotation += T.deltaPosition.y;
                    }
                }
            }

            Vector3 movement = new Vector3(xRotation, yRotation, 0.0f);
            GetComponent<Rigidbody>().velocity = movement * speed;
        }
        else
        {
            deathtext.SetActive(true);
            deathtext.GetComponent<UnityEngine.UI.Text>().text = " G A M E   O V E R !";

            foreach (Touch T in Input.touches)
            {
                //Forget it when the touch ends
                if ((T.phase == TouchPhase.Ended) || (T.phase == TouchPhase.Canceled))
                {
                    score = 0;
                    deathMenu = false;

                    FindObjectOfType<WorldManager>().reload();

                    transform.position = new Vector3(0, 0, -2);
                    GetComponent<Rigidbody>().velocity = Vector3.zero;

                }
            }
        }

        Vector3 scale = FindObjectOfType<WorldManager>().transform.localScale;

        Vector3 velocity = GetComponent<Rigidbody>().velocity;

        Vector3 pos = transform.position;

        if (pos.x > scale.x / 2 - 0.5f)
        {
            pos.x = scale.x / 2 - 0.5f;
            velocity = new Vector3(-velocity.x, velocity.y, velocity.z) * 1.3f;
        }
        else if (pos.x < -scale.x / 2 + 0.5f)
        {
            pos.x = -scale.x / 2 + 0.5f;
            velocity = new Vector3(-velocity.x, velocity.y, velocity.z) * 1.3f;
        }
        if (pos.y > scale.y / 2 - 0.5f)
        {
            pos.y = scale.y / 2 - 0.5f;
            velocity = new Vector3(velocity.x, -velocity.y, velocity.z) * 1.3f;
        }
        else if (pos.y < -scale.y / 2 + 0.5f)
        {
            pos.y = -scale.y / 2 + 0.5f;
            velocity = new Vector3(velocity.x, -velocity.y, velocity.z) * 1.3f;
        }

        xRotation = velocity.x / speed;
        yRotation = velocity.y / speed;

        transform.position = pos;
        GetComponent<Rigidbody>().velocity = velocity;
    }
}
