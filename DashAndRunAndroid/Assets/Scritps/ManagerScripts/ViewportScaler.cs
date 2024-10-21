using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ViewportScaler : MonoBehaviour,IManagerModule
{
    // https://gamedesigntheory.blogspot.com/2010/09/controlling-aspect-ratio-in-unity.html

    [SerializeField] private Vector2 targetAspect = new Vector2(1920f, 1080);
    [SerializeField] private Color backgroundColor = new Color(0f, 0f, 0f);
    private float targetRatio;

    public int ManagerExecutionOrder => 20;
    public void OnModuleAwake()
    {
        SceneManager.activeSceneChanged += AdjustCameraViewport;
    }

    private void AdjustCameraViewport(Scene current, Scene next )
    {
        targetRatio = (float)targetAspect.x / (float)targetAspect.y;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        Camera mainCamera = Camera.main;

        Rect rect = CalculateViewportRect(windowAspect, targetRatio);

        SetCameraRect(mainCamera, rect);
        CreateBackGroundCam();
    }

    private Rect CalculateViewportRect(float windowAspect, float targetRatio)
    {
        if (windowAspect < targetRatio)
        {
            float scaleHeight = windowAspect / targetRatio;
            return new Rect(0f, (1f - scaleHeight) / 2f, 1f, scaleHeight);
        }
        else
        {
            float scaleWidth = 1f / (windowAspect / targetRatio);
            return new Rect((1f - scaleWidth) / 2f, 0f, scaleWidth, 1f);
        }
    }

    private void SetCameraRect(Camera camera, Rect rect)
    {
        camera.rect = rect;
    }

    private void CreateBackGroundCam()
    {
        Camera background = new GameObject().AddComponent<Camera>();
        background.backgroundColor = backgroundColor;
        background.cullingMask = 0;
        background.depth = -100;
        background.farClipPlane = 1;
        background.useOcclusionCulling = false;
        background.allowHDR = false;
        background.clearFlags = CameraClearFlags.Color;
        background.name = "Background Camera";
    }
}