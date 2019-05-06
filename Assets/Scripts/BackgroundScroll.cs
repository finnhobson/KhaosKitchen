using UnityEngine;
using System.Collections;

public class BackgroundScroll : MonoBehaviour
{
    public float XscrollSpeedPerSec = 20;
    public float YscrollSpeedPerSec = 40;
    public float maxYsize = 1920f;
    public float maxXsize = 1080f;

    public float startY;
    public float startX;
    public float offsetY;
    public float offsetX;

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
