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

        if (!gameController.isRoundPaused && gameController.isGameStarted)
        {
            if (transform.localPosition.x > xMax + 2)
            {
                x = -maxSpeed;
                transform.localRotation = Quaternion.Euler(0, 270, 0);
                time = 0.0f;
            }

            if (transform.localPosition.x < xMin - 2)
            {
                x = maxSpeed;
                transform.localRotation = Quaternion.Euler(0, 90, 0);
                time = 0.0f;
            }

            if (transform.localPosition.x > xMax)
            {
                x = Random.Range(-maxSpeed, 0.0f);
                angle = Mathf.Atan2(x, z) * (180 / 3.141592f);
                transform.localRotation = Quaternion.Euler(0, angle, 0);
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
            }

            transform.localPosition = new Vector3(transform.localPosition.x + x, transform.localPosition.y, transform.localPosition.z + z);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

    }
}
