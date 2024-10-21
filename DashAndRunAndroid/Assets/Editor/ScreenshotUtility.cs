using UnityEditor;
using UnityEngine;

public class ScreenshotUtilityEditor : EditorWindow
{
    private static ScreenshotCapture captureInstance;

    // Add menu item in GameObject/My Settings
    [MenuItem("GameObject/My Settings/Enable Screenshot Mode")]
    public static void EnableScreenshotMode()
    {
        if (!Application.isPlaying)
        {
            Debug.LogError("Screenshot mode can only be enabled in Play Mode.");
            return;
        }

        if (captureInstance == null)
        {
            GameObject manager = GameObject.Find("GameBehaviour");
            manager.AddComponent<ScreenshotCapture>();
            Debug.Log("Screenshot mode activated! Press 'E' in Play Mode to take a screenshot.");
        }
    }
}
