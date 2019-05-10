using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerController : MonoBehaviour
{
    public float maxSpeed;

    public float xMax;
    public float zMax;
    public float xMin;
    public float zMin;

    private float x;
    private float z;
    private float time;
    private float angle;

    private GameController gameController;

    public GameObject shirt;
    public List<GameObject> hatParts;

    // Use this for initialization
    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        if (transform.localPosition.x > xMax + 5)
        {
            x = -maxSpeed;
            z = 0;
            transform.localRotation = Quaternion.Euler(0, 270, 0);
            time = 0.0f;
        }

        if (transform.localPosition.x < xMin - 5)
        {
            x = maxSpeed;
            z = 0;
            transform.localRotation = Quaternion.Euler(0, 90, 0);
            time = 0.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        int rand;

        if (!gameController.isRoundPaused && gameController.isGameStarted)
        {

            if (x == 0)
            {
                if (z > 0)
                {
                    transform.localRotation = Quaternion.Euler(0, 0, 0); 
                }
                else if (z < 0)
                {
                    transform.localRotation = Quaternion.Euler(0, 180, 0);
                }

            }
            if (z == 0)
            {
                if (x > 0)
                {
                    transform.localRotation = Quaternion.Euler(0, 90, 0);
                }
                else if (x < 0)
                {
                    transform.localRotation = Quaternion.Euler(0, 270, 0);
                }

            }

            //WALKING DOWN PATH
            /*if (transform.localPosition.x > xMax)
            {
                x = -maxSpeed;
                transform.localRotation = Quaternion.Euler(0, 270, 0);
                time = 0.0f;
            }

            if (transform.localPosition.x < -xMax)
            {
                x = maxSpeed;
                transform.localRotation = Quaternion.Euler(0, 90, 0);
                time = 0.0f;
            }*/

            //MOVING INSIDE RESTAURANT
            if (transform.localPosition.x > 17.8 && transform.localPosition.x < 18 && transform.localPosition.z == -28)
            {
                rand = Random.Range(0, 3);
                if (rand == 0)
                {
                    x = 0;
                    z = Random.Range(0.0f, -maxSpeed);
                   // transform.localRotation = Quaternion.Euler(0, 180, 0);
                }
                if (rand == 1)
                {
                    z = 0;
                    x = Random.Range(0.0f, -maxSpeed);
                   // transform.localRotation = Quaternion.Euler(0, 270, 0);
                }
                if (rand == 2)
                {
                    x = 0;
                    z = Random.Range(0.0f, maxSpeed);
                    //transform.localRotation = Quaternion.Euler(0, 0, 0);
                } 
            }

            if (transform.localPosition.x < -17.8 && transform.localPosition.x > -18 && transform.localPosition.z == -28)
            {
                rand = Random.Range(0, 3);
                if (rand == 0)
                {
                    x = 0;
                    z = Random.Range(0.0f, -maxSpeed);
                   // transform.localRotation = Quaternion.Euler(0, 180, 0);
                }
                if (rand == 1)
                {
                    z = 0;
                    x = Random.Range(0.0f, maxSpeed);
                   // transform.localRotation = Quaternion.Euler(0, 90, 0);
                }
                if (rand == 2)
                {
                    x = 0;
                    z = Random.Range(0.0f, maxSpeed);
                  //  transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }

            //TOP RIGHT
            if (transform.localPosition.x < -17.8 && transform.localPosition.x > -18 && transform.localPosition.z > -38.2 && transform.localPosition.z < -38)
            {
                if (z != 0)
                {
                    z = 0;
                    x = Random.Range(0.03f, maxSpeed);
                    //transform.localRotation = Quaternion.Euler(0, 90, 0);
                }
                else if (x != 0)
                {
                    x = 0;
                    z = Random.Range(0.03f, maxSpeed);
                  // transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }

            //TOP LEFT
            if (transform.localPosition.x > 17.8 && transform.localPosition.x < 18 && transform.localPosition.z > -38.2 && transform.localPosition.z < -38)
            {
                //Moving in z direction
                if (z != 0)
                {
                    z = 0;
                    x = Random.Range(-0.03f, -maxSpeed);
                   // transform.localRotation = Quaternion.Euler(0, 270, 0);
                }
                //Moving in x direction
                else if (x != 0)
                {
                    x = 0;
                    z = Random.Range(0.03f, maxSpeed);
                   // transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }

            //BOTTOM LEFT
            if (transform.localPosition.x > 17.8 && transform.localPosition.x < 18 && transform.localPosition.z > -20.2 && transform.localPosition.z < -20)
            {
                if (z != 0)
                {
                    z = 0;
                    x = Random.Range(-0.03f, -maxSpeed);
                   // transform.localRotation = Quaternion.Euler(0, 270, 0);
                }
                else if (x != 0)
                {
                    x = 0;
                    z = Random.Range(-0.03f, -maxSpeed);
                   // transform.localRotation = Quaternion.Euler(0, 180, 0);
                }
            }

            //BOTTOM RIGHT
            if (transform.localPosition.x < -17.8 && transform.localPosition.x > -18 && transform.localPosition.z > -20.2 && transform.localPosition.z < -20)
            {
                //Moving in z direction
                if (z != 0)
                {
                    z = 0;
                    x = Random.Range(0.03f, maxSpeed);
                   // transform.localRotation = Quaternion.Euler(0, 90, 0);
                }
                else if (x != 0)
                {
                    x = 0;
                    z = Random.Range(-0.03f, -maxSpeed);
                  // transform.localRotation = Quaternion.Euler(0, 180, 0);
                }
            }

            //BOTTOM RIGHT MIDDLE 
            if (transform.localPosition.x < -5.8 && transform.localPosition.x > -6 && transform.localPosition.z > -20.2 && transform.localPosition.z < -20)
            {
                rand = Random.Range(0, 2);
                if (rand == 0)
                {
                    x = 0;
                    z = Random.Range(0.0f, -maxSpeed);
                    // transform.localRotation = Quaternion.Euler(0, 180, 0);
                }
                if (rand == 1)
                {
                    z = 0;
                    x = Random.Range(0.0f, maxSpeed);
                    // transform.localRotation = Quaternion.Euler(0, 90, 0);
                }

            }
            //BOTTOM LEFT MIDDLE 
            if (transform.localPosition.x < 5.8 && transform.localPosition.x > 6 && transform.localPosition.z > -20.2 && transform.localPosition.z < -20)
            {
                rand = Random.Range(0, 2);
                if (rand == 0)
                {
                    x = 0;
                    z = Random.Range(0.0f, -maxSpeed);
                    // transform.localRotation = Quaternion.Euler(0, 180, 0);
                }
                if (rand == 1)
                {
                    z = 0;
                    x = Random.Range(0.0f, -maxSpeed);
                    // transform.localRotation = Quaternion.Euler(0, 90, 0);
                }


            }
            //TOP LEFT MIDDLE 
            if (transform.localPosition.x < 5.8 && transform.localPosition.x > 6 && transform.localPosition.z > -38.2 && transform.localPosition.z < -38)
            {
                rand = Random.Range(0, 2);
                if (rand == 0)
                {
                    x = 0;
                    z = Random.Range(0.0f, maxSpeed);
                    // transform.localRotation = Quaternion.Euler(0, 180, 0);
                }
                if (rand == 1)
                {
                    z = 0;
                    x = Random.Range(0.0f, -maxSpeed);
                    // transform.localRotation = Quaternion.Euler(0, 90, 0);
                }
            }
            //TOP RIGHT MIDDLE 
            if (transform.localPosition.x < -5.8 && transform.localPosition.x > -6 && transform.localPosition.z > -38.2 && transform.localPosition.z < -38)
            {
                rand = Random.Range(0, 2);
                if (rand == 0)
                {
                    x = 0;
                    z = Random.Range(0.0f, maxSpeed);
                    // transform.localRotation = Quaternion.Euler(0, 180, 0);
                }
                if (rand == 1)
                {
                    z = 0;
                    x = Random.Range(0.0f, -maxSpeed);
                    // transform.localRotation = Quaternion.Euler(0, 90, 0);
                }
            }
            //RIGHT MIDDLE 
            if (transform.localPosition.x < -5.8 && transform.localPosition.x > -6 && transform.localPosition.z > -28.2 && transform.localPosition.z < -28)
            {
                rand = Random.Range(0, 4);
                if (rand == 0)
                {
                    x = 0;
                    z = Random.Range(0.0f, maxSpeed);
                    // transform.localRotation = Quaternion.Euler(0, 180, 0);
                }
                if (rand == 1)
                {
                    x = 0;
                    z = Random.Range(0.0f, -maxSpeed);
                    // transform.localRotation = Quaternion.Euler(0, 90, 0);
                }
                if (rand == 2)
                {
                    z = 0;
                    x = Random.Range(0.0f, maxSpeed);
                    // transform.localRotation = Quaternion.Euler(0, 180, 0);
                }
                if (rand == 3)
                {
                    z = 0;
                    x = Random.Range(0.0f, -maxSpeed);
                    // transform.localRotation = Quaternion.Euler(0, 90, 0);
                }
            }
           
            //LEFT MIDDLE 
             if (transform.localPosition.x < 5.8 && transform.localPosition.x > 6 && transform.localPosition.z > -28.2 && transform.localPosition.z < -28)
            {
                rand = Random.Range(0, 4);
                if (rand == 0)
                {
                    x = 0;
                    z = Random.Range(0.0f, maxSpeed);
                    // transform.localRotation = Quaternion.Euler(0, 180, 0);
                }
                if (rand == 1)
                {
                    x = 0;
                    z = Random.Range(0.0f, -maxSpeed);
                    // transform.localRotation = Quaternion.Euler(0, 90, 0);
                }
                if (rand == 2)
                {
                    z = 0;
                    x = Random.Range(0.0f, maxSpeed);
                    // transform.localRotation = Quaternion.Euler(0, 180, 0);
                }
                if (rand == 3)
                {
                    z = 0;
                    x = Random.Range(0.0f, -maxSpeed);
                    // transform.localRotation = Quaternion.Euler(0, 90, 0);
                }
            }
            /*if (transform.localPosition.x > xMax)
            {
                x = Random.Range(-maxSpeed, 0.0f);
                angle = Mathf.Atan2(x, z) * (180 / 3.141592f);

                time = 0.0f;
            }

            if (transform.localPosition.x < xMin)
            {
                x = Random.Range(0.0f, maxSpeed);
                angle = Mathf.Atan2(x, z) * (180 / 3.141592f);
                transform.localRotation = Quaternion.Euler(0, angle, 0);
                time = 0.0f;
            }

            if (transform.localPosition.z > zMax)
            {
                z = Random.Range(-maxSpeed, 0.0f);
                angle = Mathf.Atan2(x, z) * (180 / 3.141592f);
                transform.localRotation = Quaternion.Euler(0, angle, 0);
                time = 0.0f;
            }

            if (transform.localPosition.z < zMin)
            {
                z = Random.Range(0.0f, maxSpeed);
                angle = Mathf.Atan2(x, z) * (180 / 3.141592f);
                transform.localRotation = Quaternion.Euler(0, angle, 0);
                time = 0.0f;
            }


            if (time > 2.0f)
            {
                x = Random.Range(-maxSpeed, maxSpeed);
                z = Random.Range(-maxSpeed, maxSpeed);
                angle = Mathf.Atan2(x, z) * (180 / 3.141592f);
                transform.localRotation = Quaternion.Euler(0, angle, 0);
                time = 0.0f;
            }*/

            transform.localPosition = new Vector3(transform.localPosition.x + x, transform.localPosition.y, transform.localPosition.z + z);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

    }
}
