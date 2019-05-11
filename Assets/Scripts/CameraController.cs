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
        if (camAvailable) backCam.Stop();
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
            int pixelCount = 0;
            int redCount = 0;
            int orangeCount = 0;
            int yellowCount = 0;
            int greenCount = 0;
            int blueCount = 0;

            Color[] pixels = backCam.GetPixels();
            foreach (Color pixel in pixels)
            {
                if (pixel.r > 0.7 && pixel.g < 0.3 && pixel.b < 0.3) redCount++;
                if (pixel.r > 0.7 && pixel.g > 0.3 && pixel.g < 0.5 && pixel.b < 0.3) orangeCount++;
                if (pixel.r > 0.7 && pixel.g > 0.7 && pixel.b < 0.2) yellowCount++;
                if (pixel.r < 0.5 && pixel.g > 0.6 && pixel.b < 0.3) greenCount++;
                if (pixel.r < 0.3 && pixel.g < 0.5 && pixel.b > 0.6) blueCount++;
                pixelCount++;
            }

            if (redCount / pixelCount > 0.3) red = true;
            if (orangeCount / pixelCount > 0.3) orange = true;
            if (yellowCount / pixelCount > 0.3) yellow = true;
            if (greenCount / pixelCount > 0.3) green = true;
            if (blueCount / pixelCount > 0.3) blue = true;
        }
    }
}
