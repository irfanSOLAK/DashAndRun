using System.Collections;
using UnityEngine;

[ExecutionOrder(19)]
public class CameraFollow : MonoBehaviour, IExecutionOrder
{
    // çok önemli not eðer target in rigidbody2d si interpolate yi interpolate seçmen gerekiyor
    // yoksa karakter sanki titriyormuþ gibi görünüyor.

    #region Serialized Fields
    [SerializeField] private Transform target;          // Takip edilecek hedef (karakter)
    [SerializeField] private Vector3 offset = new Vector3(5f, 1.5f, -10f);            // Kameranýn hedefe göre ofseti
    [SerializeField] private float pathDampening = 0.125f; // Kameranýn yumuþak hareket hýzý
    [SerializeField] private float zoomDampening = 0.125f; // Kameranýn yumuþak hareket hýzý
    [SerializeField] private float zoomSpeed = 2f;      // Yakýnlaþtýrma/uzaklaþtýrma hýzý
    [SerializeField] private Vector2 zoomLimits = new Vector2(5f, 6f); // Yakýnlaþtýrma sýnýrlarý
    [SerializeField] private float shakeIntensity = 0.1f; // Kamera sarsýlma yoðunluðu
    [SerializeField] private float bounceIntensity = 0.01f; // Zýplama sýrasýnda kameranýn zýplama etkisi
    #endregion

    #region Private Fields
    private Camera cam;                             // Kamera bileþeni
    private Vector3 desiredPosition;                // Kameranýn hedeflenen pozisyonu
    private Vector3 smoothedPosition;               // Yumuþatýlmýþ pozisyon
    private Vector3 targetPreviousPosition;         // Hedefin önceki pozisyonu
    private Vector3 velocity = Vector3.zero;        // Yumuþatma hesaplamalarý için hýz
    private Vector3 shakeOffset = Vector3.zero;     // Kamera sarsýlma etkisi için offset
    private float currentZoomVelocity = 0f;         // Zoom yumuþatmasý için hýz
    #endregion

    #region Unity Callbacks

    public void ManagedAwake()
    {
        GameObject sceneController = GameObject.FindWithTag("SceneController");
        Vector3 camStartPos = sceneController.GetComponent<SpawnPoints>().SpawnTransform.position;
        transform.position = new Vector3(camStartPos.x, camStartPos.y, transform.position.z);
        sceneController.GetComponent<ScriptManager>().RelatedScripts.Add(this);
    }

    void Start()
    {
        Initialize();
    }

    void LateUpdate()
    {
        FollowTarget();
        AdjustZoom();
        ApplyBounceEffect();
    }


    #endregion

    #region Initialization
    private void Initialize()
    {
        if (!target)
        {
            target = GameObject.FindWithTag("Player").transform;
        }

        cam = GetComponent<Camera>();
        targetPreviousPosition = target.position;
    }
    #endregion

    #region CameraEffects
    private void FollowTarget()
    {
        // Hedef pozisyonunu hesapla
        desiredPosition = target.position + offset;
        // Kameranýn hedef pozisyona yumuþak geçiþini saðla
        smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, pathDampening);
        transform.position = smoothedPosition;
    }

    private void AdjustZoom()
    {
        float playerSpeed = Mathf.Abs(target.GetComponent<Rigidbody2D>().velocity.x);
        float targetOrthoSize = Mathf.Clamp(zoomLimits.x + playerSpeed * zoomSpeed, zoomLimits.x, zoomLimits.y);

        // Zoom seviyesini yumuþat
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetOrthoSize, ref currentZoomVelocity, zoomDampening);
    }

    private void ApplyBounceEffect()
    {
        if (target.position.y > targetPreviousPosition.y) // Karakter yukarý zýplýyorsa
        {
            transform.position += Vector3.up * bounceIntensity;
        }
        // Güncellenmiþ pozisyonu sakla
        targetPreviousPosition = target.position;
    }
    #endregion

    private IEnumerator ShakeCameraCoroutine(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            ShakeCamera();
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Shake süresi sona erdiðinde kamerayý orijinal pozisyona döndür
        transform.position = smoothedPosition;
    }

    private void ShakeCamera()
    {
        // Kamera sarsýlmasýný uygula
        shakeOffset = Random.insideUnitSphere * shakeIntensity;
        shakeOffset.z = 0; // Z ekseninde hareket etmemesi için
        transform.position = smoothedPosition + shakeOffset;
    }
}
