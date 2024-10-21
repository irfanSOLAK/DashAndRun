using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [Header("Falling Platform Settings")]
    [SerializeField] private float fallDelay = 0.3f; // Oyuncu dokunduktan sonra d��me gecikmesi
    [SerializeField] private float fallSpeed = 5f; // Platformun d��me h�z�
    [SerializeField] private float resetTime = 3f; // Platformun tekrar yerle�me s�resi
    [SerializeField] bool autoReturn; // Platformun tekrar yerle�mesini istersen


    private bool isFalling = false;
    private Rigidbody2D rb;
    private Vector3 originalPosition;

    private GameObject platform;
    private Animator animator;
    private BoxCollider2D boxCollider;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true; // Ba�lang��ta hareket etmemesi i�in kinematic
        originalPosition = transform.position;

        platform = transform.GetChild(0).gameObject;
        animator = platform.GetComponent<Animator>();
        boxCollider = platform.GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Oyuncu platforma dokundu�unda
        if (collision.CompareTag("PlayerGround"))
        {
            Invoke(nameof(StartFalling), fallDelay);
        }
    }

    private void StartFalling()
    {
        isFalling = true;
        animator.SetBool("IsFalling", isFalling);
        GameBehaviour.Instance.Audio.PlaySound("PlatformFall");
        boxCollider.isTrigger = true;
        rb.isKinematic = false; // Platformun hareket edebilir olmas�n� sa�la
        rb.velocity = Vector2.down * fallSpeed;
        Invoke(nameof(ResetPlatform), resetTime);
    }

    private void ResetPlatform()
    {
        if (!autoReturn) return;
        transform.position = originalPosition;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true; // Tekrar hareketsiz hale getir
        boxCollider.isTrigger = false;
        isFalling = false;
        animator.SetBool("IsFalling", isFalling);
    }
}
