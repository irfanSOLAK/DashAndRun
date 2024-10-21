using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] float moveSpeed;                                    // Character's horizontal movement speed
    [SerializeField] Transform sideGroundCheck;                          // Transform for ground check (child gameobject)
    [SerializeField] LayerMask wallLayer;                              // Layer mask for ground detection
    #endregion

    #region Private Fields
    private float horizontalInput;       // Horizontal movement input
    private bool isFacingRight = true;   // Character's facing direction
    private Rigidbody2D rb;              // Rigidbody2D component
    private float sideGroundCheckRadius;
    private Joystick joystick;
    #endregion

    #region Prop
    private CharacterState currentState;

    public CharacterState CurrentState
    {
        get => currentState;
        set
        {
            if (currentState != value)
            {
                currentState = value;
                GameBehaviour.Instance.Notifications.PostNotification(currentState);
            }
        }
    }

    #endregion

    #region Unity Callbacks
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sideGroundCheckRadius = sideGroundCheck.GetComponent<CircleCollider2D>().radius;
        joystick = GameObject.FindWithTag("InputCanvas").GetComponentInChildren<Joystick>();
    }

    void Update()
    {
        // Giriþleri al
        HandleInput();
    }

    void FixedUpdate()
    {
        // Karakterin hareketini yönet
        Move();
        SetCurrentState();
        CheckWallCollisions();
    }
    #endregion

    #region Movement and Actions
    private void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal") + joystick.Horizontal;
        Flip();
    }

    private void Flip()
    {
        // Karakterin yönünü kontrol et ve döndür
        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void Move()
    {
        // Yatay hareketi uygula
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
    }
    #endregion

    #region Wall Check

    private void CheckWallCollisions()
    {
        // Duvarda olup olmadýðýný kontrol et
        bool isTouchingSideWall = Physics2D.OverlapCircle(sideGroundCheck.position, sideGroundCheckRadius, wallLayer);

        if (isTouchingSideWall)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    #endregion


    private void SetCurrentState()
    {
        var state = GetState();
        if (state == CurrentState) return;
        CurrentState = state;
    }

    private CharacterState GetState()
    {
        // Yükseklik kontrolü ile zýplama ve düþme durumlarýný belirle
        if (Mathf.Abs(rb.velocity.y) > 2f)
        {
            return rb.velocity.y > 0.5f
                ? (GetComponent<CharacterJump>().IsGrounded ? CharacterState.Jump : CharacterState.DoubleJump)
                : CharacterState.Fall;
        }

        // Yatay hýz kontrolü ile koþma ve boþta olma durumlarýný belirle
        return Mathf.Abs(rb.velocity.x) > 0.5f
            ? CharacterState.Run
            : CharacterState.Idle;
    }
}
