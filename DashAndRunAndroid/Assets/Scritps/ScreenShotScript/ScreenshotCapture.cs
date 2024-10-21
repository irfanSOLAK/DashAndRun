using UnityEngine;

public class ScreenshotCapture : MonoBehaviour
{
    private void Update()
    {
        // Check for 'E' key press
        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeScreenshot();
        }
    }

    private void TakeScreenshot()
    {
        string path = $"Screenshot_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
        ScreenCapture.CaptureScreenshot(path);
        Debug.Log($"Screenshot saved to: {path}");
    }

    private void OnDestroy()
    {
        Debug.Log("Screenshot mode deactivated.");
    }
}
