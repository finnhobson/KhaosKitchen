using UnityEngine;
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
        InitCamera();
    }

    void InitCamera()
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

    private void OnDisable()
    {
        backCam.Stop();
        camAvailable = false;
        red = false;
        blue = false;
        green = false;
        orange = false;
        yellow = false;
    }

    private void OnEnable()
    {
        InitCamera();
        red = false;
        blue = false;
        green = false;
        orange = false;
        yellow = false;
    }


    // Update is called once per frame
    void Update()
    {
        red = false;
        blue = false;
        green = false;
        orange = false;
        yellow = false;

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

            //R.text = avgRed.ToString("F3");
            //G.text = avgGreen.ToString("F3");
            //B.text = avgBlue.ToString("F3");

            //Red
            if (avgRed > 0.6 && avgBlue < 0.3 && avgGreen < 0.3) red = true;

            //Dark Blue
            if (avgBlue > 0.6 && avgRed < 0.3 && avgGreen < 0.3) blue = true;

            //Green
            if (avgGreen > 0.6 && avgBlue < 0.3 && avgRed < 0.3) green = true;

            //Orange
            if (avgRed > 0.6 && avgGreen > 0.3 && avgGreen < 0.5 && avgBlue < 0.2) orange = true;

            //Yellow
            if (avgRed > 0.5 && avgGreen > 0.5 && avgBlue < 0.2) yellow = true;
        }
    }
}
