using UnityEngine;
using System.IO;

public class ScreenshotMonoBehaviour : MonoBehaviour
{
    private string directoryPath = "Assets/Screenshots";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeScreenshot();
        }
    }

    private void TakeScreenshot()
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string uniqueFilePath = $"{directoryPath}/Screenshot_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
        ScreenCapture.CaptureScreenshot(uniqueFilePath);
        Debug.Log($"Screenshot saved to: {uniqueFilePath}");
    }
}
