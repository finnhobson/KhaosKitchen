﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraController : MonoBehaviour
{
    private bool camAvailable;
    private WebCamTexture backCam;
    public RawImage panel;
    public GameObject colourPanel;
    public Text R, G, B;
    public bool red, orange, yellow, green, blue;
    public Player player;

    // Use this for initialization
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.Log("No camera detected");
            camAvailable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                backCam = new WebCamTexture(devices[i].name);
            }
        }

        if (backCam == null)
        {
            Debug.Log("Unable to find back camera");
            return;
        }

        backCam.Play();
        panel.texture = backCam;

        camAvailable = true;
    }

    
    // Update is called once per frame
    void Update()
    {
        if (camAvailable)
        {
            float avgRed = 0.0f;
            float avgGreen = 0.0f;
            float avgBlue = 0.0f;
            int pixelCount = 0;
            Color[] pixels = backCam.GetPixels();
            foreach (Color pixel in pixels)
            {
                avgRed += pixel.r;
                avgGreen += pixel.g;
                avgBlue += pixel.b;
                pixelCount++;
            }
            avgRed = avgRed / pixelCount;
            avgGreen = avgGreen / pixelCount;
            avgBlue = avgBlue / pixelCount;

            R.text = avgRed.ToString("F3");
            G.text = avgGreen.ToString("F3");
            B.text = avgBlue.ToString("F3");

            //Red
            if (avgRed > 0.6 && avgBlue < 0.3 && avgGreen < 0.3)
            {
                colourPanel.GetComponent<Image>().color = Color.red;
                player.cameraRed = true;
            }
            else player.cameraRed = false;

            //Dark Blue
            if (avgBlue > 0.6 && avgRed < 0.3 && avgGreen < 0.3)
            {
                colourPanel.GetComponent<Image>().color = Color.blue;
                player.cameraBlue = true;
            }
            else player.cameraBlue = false;

            //Green
            if (avgGreen > 0.6 && avgBlue < 0.3 && avgRed < 0.3)
            {
                colourPanel.GetComponent<Image>().color = Color.green;
                player.cameraGreen = true;
            }
            else player.cameraGreen = false;

            //Orange
            if (avgRed > 0.6 && avgGreen > 0.3 && avgGreen < 0.5 && avgBlue < 0.2)
            {
                colourPanel.GetComponent<Image>().color = new Color(1, 0.5f, 0, 1);
                player.cameraOrange = true;
            }
            else player.cameraOrange = false;

            //Yellow
            if (avgRed > 0.5 && avgGreen > 0.5 && avgBlue < 0.2)
            {
                colourPanel.GetComponent<Image>().color = Color.yellow;
                player.cameraYellow = true;
            }
            else player.cameraYellow = false;

        }
    }
}
