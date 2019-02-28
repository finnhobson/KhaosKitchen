using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefController : MonoBehaviour
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

    // Use this for initialization
    void Start()
    {

        x = Random.Range(-maxSpeed, maxSpeed);
        z = Random.Range(-maxSpeed, maxSpeed);
        angle = Mathf.Atan2(x, z) * (180 / 3.141592f);
        transform.localRotation = Quaternion.Euler(0, angle, 0);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

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


        if (time > 1.0f)
        {
            x = Random.Range(-maxSpeed, maxSpeed);
            z = Random.Range(-maxSpeed, maxSpeed);
            angle = Mathf.Atan2(x, z) * (180 / 3.141592f);
            transform.localRotation = Quaternion.Euler(0, angle, 0);
            time = 0.0f;
        }

        transform.localPosition = new Vector3(transform.localPosition.x + x, transform.localPosition.y, transform.localPosition.z + z);
    }
}
