
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MusicPlayer : MonoBehaviour
{
    
    /*
     * Remember to add variance to button sounds.
     */

    //GameObjects
    private AudioSource BackgroundSource;
    public AudioClip BackgroundMusic;
    public AudioClip GameOverClip;
    public AudioClip[] RoundBreaks;
    public AudioClip[] Countdown;
    float m_MySliderValue;
    
    //Primitives
    private int NumberOfRoundBreaks;
    private int Switch = 0;
    
    //Booleans
    private bool isRoundPaused = false;
    private bool isMusicPaused = false;
    
    private void Awake()
    {
        BackgroundSource = GetComponent<AudioSource>();
        BackgroundSource.clip = BackgroundMusic;
    }

    private void Start()
    {
        NumberOfRoundBreaks = RoundBreaks.Length;
        m_MySliderValue = 0.5f;
    }

    private void Update()
    {        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BackgroundSource.Pause();
        }

        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            BackgroundSource.UnPause();
        }
    }

    public void PauseMusic()
    {
        isMusicPaused = true;
        BackgroundSource.Stop();
    }

    public void UnPauseMusic()
    {
        isMusicPaused = false;
        BackgroundSource.UnPause();
    }

    public void PlayGameOver()
    {
        BackgroundSource.Stop();
        BackgroundSource.clip = GameOverClip;
        BackgroundSource.Play();
//        BackgroundSource.PlayOneShot(GameOverClip, 2f);
    }

    void PlayBackgroundMusic()
    {
        BackgroundSource.Play();
    }

    public void PlayRoundBreak()
    {
        if(!isMusicPaused) PauseMusic();
        
        BackgroundSource.PlayOneShot(RoundBreaks[Switch], 2f);
        Switch = (Switch + 1) % NumberOfRoundBreaks;
    }

    public void StartRoundMusic()
    {
        isRoundPaused = false;
        isMusicPaused = false;
        PlayBackgroundMusic();
    }

    public void PlayCountDown(int count)
    {
        BackgroundSource.PlayOneShot(Countdown[count], 2f);
    }
    
    void OnGUI()
    {
        //Create a horizontal Slider that controls volume levels. Its highest value is 1 and lowest is 0
        m_MySliderValue = GUI.HorizontalSlider(new Rect(25, 25, 200, 60), m_MySliderValue, 0.0F, 1.0F);
        //Makes the volume of the Audio match the Slider value
        BackgroundSource.volume = m_MySliderValue;
    }
}
