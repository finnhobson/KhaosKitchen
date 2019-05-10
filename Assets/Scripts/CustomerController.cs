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

    private int junction = -1;

    private GameController gameController;
    private AnimationController animationController;

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
        

            //MOVING INSIDE RESTAURANT

            //LEFT EXIT
            if (transform.localPosition.x > 17.8 && transform.localPosition.x < 18 && transform.localPosition.z > -28.1 && transform.localPosition.z < -27.9 && junction != 0)
            {
                junction = 0;
                rand = Random.Range(0, 3);
                if (rand == 0)
                {
                    x = 0;
                    z = Random.Range(0.0f, -maxSpeed);
                }
                if (rand == 1)
                {
                    z = 0;
                    x = Random.Range(0.0f, -maxSpeed);
                }
                if (rand == 2)
                {
                    x = 0;
                    z = Random.Range(0.0f, maxSpeed);
                }
                /*if (gameController.customerSatisfaction >= 25)
                {
                    
                }
                else if (animationController.customerNumber > 1)
                {
                    z = 0;
                    x = maxSpeed;
                    animationController.customerNumber--;
                }*/
            }
        

            //RIGHT EXIT
            if (transform.localPosition.x < -17.8 && transform.localPosition.x > -18 && transform.localPosition.z > -28.1 && transform.localPosition.z < -27.9 && junction != 1)
            {
                junction = 1;
                rand = Random.Range(0, 3);
                if (rand == 0)
                {
                    x = 0;
                    z = Random.Range(0.0f, -maxSpeed);
                }
                if (rand == 1)
                {
                    z = 0;
                    x = Random.Range(0.0f, maxSpeed);
                }
                if (rand == 2)
                {
                    x = 0;
                    z = Random.Range(0.0f, maxSpeed);
                }
                /*if (gameController.customerSatisfaction >= 25)
                {
                    
                }
                else if (animationController.customerNumber > 1)
                {
                    z = 0;
                    x = -maxSpeed;
                    animationController.customerNumber--;
                }*/
            }
        

            //TOP RIGHT
            if (transform.localPosition.x < -17.8 && transform.localPosition.x > -18 && transform.localPosition.z > -38.2 && transform.localPosition.z < -38 && junction != 2)
            {
                junction = 2;
                if (z != 0)
                {
                    z = 0;
                    x = Random.Range(0.03f, maxSpeed);
                }
                else if (x != 0)
                {
                    x = 0;
                    z = Random.Range(0.03f, maxSpeed);
                }
            }

            //TOP LEFT
            if (transform.localPosition.x > 17.8 && transform.localPosition.x < 18 && transform.localPosition.z > -38.2 && transform.localPosition.z < -38 && junction != 3)
            {
                junction = 3;
                //Moving in z direction
                if (z != 0)
                {
                    z = 0;
                    x = Random.Range(-0.03f, -maxSpeed);
                }
                //Moving in x direction
                else if (x != 0)
                {
                    x = 0;
                    z = Random.Range(0.03f, maxSpeed);
                }
            }

            //BOTTOM LEFT
            if (transform.localPosition.x > 17.8 && transform.localPosition.x < 18 && transform.localPosition.z > -20.2 && transform.localPosition.z < -20 && junction != 4)
            {
                junction = 4;
                if (z != 0)
                {
                    z = 0;
                    x = Random.Range(-0.03f, -maxSpeed);
                }
                else if (x != 0)
                {
                    x = 0;
                    z = Random.Range(-0.03f, -maxSpeed);
                }
            }

            //BOTTOM RIGHT
            if (transform.localPosition.x < -17.8 && transform.localPosition.x > -18 && transform.localPosition.z > -20.2 && transform.localPosition.z < -20 && junction != 5)
            {
                junction = 5;
                //Moving in z direction
                if (z != 0)
                {
                    z = 0;
                    x = Random.Range(0.03f, maxSpeed);
                }
                else if (x != 0)
                {
                    x = 0;
                    z = Random.Range(-0.03f, -maxSpeed);
                }
            }

            //BOTTOM RIGHT MIDDLE 
            if (transform.localPosition.x < -5.8 && transform.localPosition.x > -6 && transform.localPosition.z > -20.2 && transform.localPosition.z < -20 && junction != 6)
            {
                junction = 6;
                rand = Random.Range(0, 2);
                if (rand == 0)
                {
                    x = 0;
                    z = Random.Range(0.0f, -maxSpeed);
                }
                if (rand == 1)
                {
                    z = 0;
                    x = Random.Range(0.0f, maxSpeed);
                }
                if (rand == 2)
                {
                    z = 0;
                    x = Random.Range(0.0f, -maxSpeed);
                }

            }

            //BOTTOM LEFT MIDDLE 
            if (transform.localPosition.x < 5.8 && transform.localPosition.x > 6 && transform.localPosition.z > -20.2 && transform.localPosition.z < -20 && junction != 7)
            {
                junction = 7;
                rand = Random.Range(0, 3);
                if (rand == 0)
                {
                    x = 0;
                    z = Random.Range(0.0f, -maxSpeed);
                }
                if (rand == 1)
                {
                    z = 0;
                    x = Random.Range(0.0f, -maxSpeed);
                }
                if (rand == 2)
                {
                    z = 0;
                    x = Random.Range(0.0f, maxSpeed);
                }
            }

            //TOP LEFT MIDDLE 
            if (transform.localPosition.x < 5.8 && transform.localPosition.x > 6 && transform.localPosition.z > -38.2 && transform.localPosition.z < -38 && junction != 8)
            {
                junction = 8;
                rand = Random.Range(0, 3);
                if (rand == 0)
                {
                    x = 0;
                    z = Random.Range(0.0f, maxSpeed);
                }
                if (rand == 1)
                {
                    z = 0;
                    x = Random.Range(0.0f, -maxSpeed);
                }
                if (rand == 2)
                {
                    z = 0;
                    x = Random.Range(0.0f, maxSpeed);
                }
            }

            //TOP RIGHT MIDDLE 
            if (transform.localPosition.x < -5.8 && transform.localPosition.x > -6 && transform.localPosition.z > -38.2 && transform.localPosition.z < -38 && junction != 9)
            {
                junction = 9;
                rand = Random.Range(0, 3);
                if (rand == 0)
                {
                    x = 0;
                    z = Random.Range(0.0f, maxSpeed);
                }
                if (rand == 1)
                {
                    z = 0;
                    x = Random.Range(0.0f, -maxSpeed);
                }
                if (rand == 2)
                {
                    z = 0;
                    x = Random.Range(0.0f, maxSpeed);
                }
            }

            //RIGHT MIDDLE 
            if (transform.localPosition.x < -5.9 && transform.localPosition.x > -6 && transform.localPosition.z > -28.1 && transform.localPosition.z < -28 && junction != 10)
            {
                junction = 10;
                rand = Random.Range(0, 4);
                if (rand == 0)
                {
                    x = 0;
                    z = Random.Range(0.0f, maxSpeed);
                }
                if (rand == 1)
                {
                    x = 0;
                    z = Random.Range(0.0f, -maxSpeed);
                }
                if (rand == 2)
                {
                    z = 0;
                    x = Random.Range(0.0f, maxSpeed);
                }
                if (rand == 3)
                {
                    z = 0;
                    x = Random.Range(0.0f, -maxSpeed);
                }
            }

            //LEFT MIDDLE 
            if (transform.localPosition.x < 5.8 && transform.localPosition.x > 6 && transform.localPosition.z > -28.2 && transform.localPosition.z < -28 && junction != 11)
            {
                junction = 11;
                rand = Random.Range(0, 4);
                if (rand == 0)
                {
                    x = 0;
                    z = Random.Range(0.0f, maxSpeed);
                }
                if (rand == 1)
                {
                    x = 0;
                    z = Random.Range(0.0f, -maxSpeed);
                }
                if (rand == 2)
                {
                    z = 0;
                    x = Random.Range(0.0f, maxSpeed);
                }
                if (rand == 3)
                {
                    z = 0;
                    x = Random.Range(0.0f, -maxSpeed);
                }
            }

            transform.localPosition = new Vector3(transform.localPosition.x + x, transform.localPosition.y, transform.localPosition.z + z);
        }

        else
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

    }
}
