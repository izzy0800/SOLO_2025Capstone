using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HighRes : MonoBehaviour
{
    public Camera captureCamera;
    public int imageWidth = 5055;
    public int imageHeight = 7110;
    public string fileName = "HighResScreenshot";

    // Optional: Set to 100 for maximum quality
    public int jpegQuality = 100;

    void Update()
    {
        // Press K to capture
        if (Input.GetKeyDown(KeyCode.K))
        {
            CaptureScreenshot();
        }
    }

    public void CaptureScreenshot()
    {
        // Create a RenderTexture at your exact resolution
        RenderTexture rt = new RenderTexture(imageWidth, imageHeight, 24);

        // Temporarily set the camera to render to this texture
        captureCamera.targetTexture = rt;

        // Create a Texture2D to store the pixels
        Texture2D screenshot = new Texture2D(imageWidth, imageHeight, TextureFormat.RGB24, false);

        // Render the camera's view
        captureCamera.Render();

        // Read the pixels from the RenderTexture
        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, imageWidth, imageHeight), 0, 0);
        screenshot.Apply();

        // Clean up
        captureCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // Save as PNG (lossless, high quality)
        byte[] bytes = screenshot.EncodeToPNG();

        // Save to your project folder
        string path = Application.dataPath + "/../" + fileName + ".png";
        File.WriteAllBytes(path, bytes);

        Debug.Log("Screenshot saved to: " + path);

        Destroy(screenshot);
    }
}
