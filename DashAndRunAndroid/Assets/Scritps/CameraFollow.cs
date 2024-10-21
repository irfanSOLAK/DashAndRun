using System.Collections;
using UnityEngine;

[ExecutionOrder(19)]
public class CameraFollow : MonoBehaviour, IExecutionOrder
{
    // �ok �nemli not e�er target in rigidbody2d si interpolate yi interpolate se�men gerekiyor
    // yoksa karakter sanki titriyormu� gibi g�r�n�yor.

    #region Serialized Fields
    [SerializeField] private Transform target;          // Takip edilecek hedef (karakter)
    [SerializeField] private Vector3 offset = new Vector3(5f, 1.5f, -10f);            // Kameran�n hedefe g�re ofseti
    [SerializeField] private float pathDampening = 0.125f; // Kameran�n yumu�ak hareket h�z�
    [SerializeField] private float zoomDampening = 0.125f; // Kameran�n yumu�ak hareket h�z�
    [SerializeField] private float zoomSpeed = 2f;      // Yak�nla�t�rma/uzakla�t�rma h�z�
    [SerializeField] private Vector2 zoomLimits = new Vector2(5f, 6f); // Yak�nla�t�rma s�n�rlar�
    [SerializeField] private float shakeIntensity = 0.1f; // Kamera sars�lma yo�unlu�u
    [SerializeField] private float bounceIntensity = 0.01f; // Z�plama s�ras�nda kameran�n z�plama etkisi
    #endregion

    #region Private Fields
    private Camera cam;                             // Kamera bile�eni
    private Vector3 desiredPosition;                // Kameran�n hedeflenen pozisyonu
    private Vector3 smoothedPosition;               // Yumu�at�lm�� pozisyon
    private Vector3 targetPreviousPosition;         // Hedefin �nceki pozisyonu
    private Vector3 velocity = Vector3.zero;        // Yumu�atma hesaplamalar� i�in h�z
    private Vector3 shakeOffset = Vector3.zero;     // Kamera sars�lma etkisi i�in offset
    private float currentZoomVelocity = 0f;         // Zoom yumu�atmas� i�in h�z
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
        // Kameran�n hedef pozisyona yumu�ak ge�i�ini sa�la
        smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, pathDampening);
        transform.position = smoothedPosition;
    }

    private void AdjustZoom()
    {
        float playerSpeed = Mathf.Abs(target.GetComponent<Rigidbody2D>().velocity.x);
        float targetOrthoSize = Mathf.Clamp(zoomLimits.x + playerSpeed * zoomSpeed, zoomLimits.x, zoomLimits.y);

        // Zoom seviyesini yumu�at
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetOrthoSize, ref currentZoomVelocity, zoomDampening);
    }

    private void ApplyBounceEffect()
    {
        if (target.position.y > targetPreviousPosition.y) // Karakter yukar� z�pl�yorsa
        {
            transform.position += Vector3.up * bounceIntensity;
        }
        // G�ncellenmi� pozisyonu sakla
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

        // Shake s�resi sona erdi�inde kameray� orijinal pozisyona d�nd�r
        transform.position = smoothedPosition;
    }

    private void ShakeCamera()
    {
        // Kamera sars�lmas�n� uygula
        shakeOffset = Random.insideUnitSphere * shakeIntensity;
        shakeOffset.z = 0; // Z ekseninde hareket etmemesi i�in
        transform.position = smoothedPosition + shakeOffset;
    }
}
