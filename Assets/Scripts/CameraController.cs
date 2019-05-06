using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraController : MonoBehaviour
{
    private bool camAvailable;
    private WebCamTexture backCam;
    public RawImage panel;
    public GameObject cameraButton;

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

    /*
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

            if (avgRed > 0.8 && avgBlue < 0.5 && avgGreen < 0.5)
            {
                //print red
                cameraButton.GetComponent<Image>().color = Color.red;
            }

            if (avgBlue > 0.8 && avgRed < 0.5 && avgGreen < 0.5)
            {
                //print blue
                cameraButton.GetComponent<Image>().color = Color.blue;
            }

            if (avgGreen > 0.8 && avgBlue < 0.5 && avgRed < 0.5)
            {
                //print green
                cameraButton.GetComponent<Image>().color = Color.green;
            }
        }
    }*/
}
