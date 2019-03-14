
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ServerAudio : MonoBehaviour
{
    
    //Remember to add variance to button sounds

    private AudioSource BackgroundSource;

    private bool isMusicPaused;
    
    // Start is called before the first frame update
    void Awake()
    {
        BackgroundSource = GetComponent<AudioSource>();
        isMusicPaused = false; 
        BackgroundSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
//            isMusicPaused = false;
            BackgroundSource.Pause();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            BackgroundSource.UnPause();
        }
    }

    void PauseMusic()
    {
        isMusicPaused = true;
    }

    void UnPauseMusic()
    {
        isMusicPaused = false;
    }
}
