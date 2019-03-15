
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
    private float VolumeOneShot = 2f;
    
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
//        BackgroundSource.clip = GameOverClip;
        BackgroundSource.PlayOneShot(GameOverClip, VolumeOneShot);
//        BackgroundSource.PlayOneShot(GameOverClip, 2f);
    }

    void PlayBackgroundMusic()
    {
        BackgroundSource.Play();
    }

    public void PlayRoundBreak()
    {
        if(!isMusicPaused) PauseMusic();
        
        BackgroundSource.PlayOneShot(RoundBreaks[Switch], VolumeOneShot);
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
        BackgroundSource.PlayOneShot(Countdown[count], VolumeOneShot);
    }
}
