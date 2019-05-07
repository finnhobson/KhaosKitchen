using UnityEngine;
using System.Collections;

public class BackgroundScroll : MonoBehaviour
{
    private float XscrollSpeedPerSec = 20;
    private float YscrollSpeedPerSec = 40;
    //public float maxYsize = 2023f;
    //public float maxXsize = 1174f;

    private float maxYsize = 1011.5f;
    private float maxXsize = 587f;

    private float startY;
    private float startX;
    private float offsetY;
    private float offsetX;

    void Start ()
    {
        startY = transform.position.y;
        startX = transform.position.x;
        offsetY = 0f;
        offsetX = 0f;

    }

    void MoveBackground()
    {
        offsetY += YscrollSpeedPerSec*Time.deltaTime;
        offsetX += XscrollSpeedPerSec*Time.deltaTime;


        if (offsetY >= maxYsize)
        {
            offsetY = 0f;
        }
         if (offsetX >= maxXsize)
        {
            offsetX = 0f;
        }
        transform.position = new Vector2(startX + offsetX, startY + offsetY);

    }

    void Update ()
    {
        //if(updateCount%10 == 0){
            MoveBackground();
        //}
        //updateCount++;


    }
}
