using UnityEngine;
using UnityEngine.UI;

public class LevelLoaderUIPositionSync : MonoBehaviour
{
    [SerializeField] private RectTransform uiElement;  // UI öðesi (RectTransform)
    [SerializeField] private Transform targetTransform;  // Hedef oyun nesnesi
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform canvasRectTransform;

    private readonly string sceneControllerTag = "SceneController";
    private readonly string effectsTag = "CharacterEffects";

    private void OnEnable()
    {
        canvas.worldCamera = Camera.main;
        FindTargetTransform();
        Vector2 uiPosition = GetUIPosition();
        SetUIPosition(uiPosition);
    }

    private void FindTargetTransform()
    {
        GameObject targetObject = GameObject.FindWithTag(effectsTag) ?? GameObject.FindWithTag(sceneControllerTag);

        if (targetObject != null)
        {
            targetTransform = targetObject.CompareTag(effectsTag)
                ? targetObject.transform
                : targetObject.GetComponent<SpawnPoints>().SpawnTransform;
        }
    }

    private Vector2 GetUIPosition()
    {
        Vector3 worldPosition = targetTransform.position;
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            screenPosition,
            canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null, // hem render mode overlay hem kamera için ayarlandý
            out Vector2 uiPosition
        );

        return uiPosition;
    }

    private void SetUIPosition(Vector2 uiPosition)
    {
        CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();

        if (canvasScaler != null && canvasScaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
        {
            Vector2 canvasSize = canvasRectTransform.sizeDelta;
            float scaleFactor = Mathf.Min(canvasSize.x / canvasScaler.referenceResolution.x, canvasSize.y / canvasScaler.referenceResolution.y);
            uiElement.anchoredPosition = uiPosition / scaleFactor;
        }
        else
        {
            uiElement.anchoredPosition = uiPosition;
        }
    }
}