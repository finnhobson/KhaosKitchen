using UnityEngine;
using System.Collections;

public class BackgroundScroll : MonoBehaviour
{
    public float scrollSpeedPerSec = 20;
    public float maxYsize = 1920f;
    public float startY;
    public float offset;

    private int updateCount;

    void Start ()
    {
        startY = transform.position.y;
        offset = 0f;
        updateCount = 0;
    }

    void MoveBackground()
    {
        offset += scrollSpeedPerSec*Time.deltaTime;

        if (offset >= maxYsize)
        {
            offset = 0f;
        }
        transform.position = new Vector2(transform.position.x, startY + offset);

    }

    void Update ()
    {
        //if(updateCount%10 == 0){
            MoveBackground();
        //}
        //updateCount++;


    }
}
