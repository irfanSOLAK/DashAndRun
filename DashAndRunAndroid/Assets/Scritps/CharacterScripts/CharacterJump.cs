using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterJump : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] float jumpForce;                                    // Jump force
    [SerializeField] [Range(0f, 1f)] float jumpBoostMultiplier;          // Character's jump boost %
    [SerializeField] [Range(0f, 1f)] float fallBoostMultiplier;          // Character's fall boost %
    [SerializeField] int maxJumps;                                       // Maximum number of jumps (double jump)
    [SerializeField] Transform groundCheck;                              // Transform for ground check (child gameobject)
    [SerializeField] LayerMask groundLayer;                              // Layer mask for ground detection
    [SerializeField] LayerMask noJumpLayer;                              // Layer mask for ground detection
    #endregion

    #region Private Fields
    private float verticalInput;         // Dikey hareket giriþi
    private int jumpsLeft;               // Remaining jumps         
    private Rigidbody2D rb;              // Rigidbody2D component
    private float groundCheckRadius;
    private float jumpBoostValue;
    private float fallBoostValue;
    private bool canJump = true;
    private Joystick joystick;
    private InputCanvas inputCanvas;
    #endregion

    #region Prop
    private bool isGrounded; // Flag indicating ground contact

    public bool IsGrounded
    {
        get => isGrounded;
        set => isGrounded=value;
    }

    #endregion

    #region Unity Callbacks
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpsLeft = maxJumps;             // Baþlangýçta zýplama sayýsýný ayarla
        groundCheckRadius = groundCheck.GetComponent<CircleCollider2D>().radius;
        inputCanvas = GameObject.FindWithTag("InputCanvas").GetComponent<InputCanvas>();
        joystick = GameObject.FindWithTag("InputCanvas").GetComponentInChildren<Joystick>();
        jumpBoostValue = jumpForce * jumpBoostMultiplier;
        fallBoostValue = jumpForce * fallBoostMultiplier;
    }

    void Update()
    {
        HandleInput();
    }
    void FixedUpdate()
    {
        ApplyFallBoost();
    }
    #endregion

    #region Movement and Actions
    private void HandleInput()
    {
        verticalInput = Input.GetAxisRaw("Vertical") + joystick.Vertical;

        if (Input.GetButtonDown("Jump") || inputCanvas.IsPressedJump)
        {
            Jump();
            inputCanvas.IsPressedJump = false;
        }
    }

    private void Jump()
    {
        if (!canJump) return;
        CheckIfGrounded();
        if (jumpsLeft > 0)
        {
            //  rb.velocity = new Vector2(rb.velocity.x, jumpForce + jumpBoostValue * verticalInput);
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(new Vector2(0f, jumpForce + jumpBoostValue * verticalInput), ForceMode2D.Impulse);
            jumpsLeft--;
            inputCanvas.JumpsLeft = jumpsLeft;
        }
    }

    private void ApplyFallBoost()
    {
        if (rb.velocity.y < 0f && verticalInput < 0f)
        {
            rb.velocity += fallBoostValue * verticalInput * Vector2.up; // fallSpeedIncrease sabit bir deðer
        }
    }
    #endregion

    #region Ground Check

    private void CheckIfGrounded()
    {
        // Karakterin yere temasýný kontrol et
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        canJump = !Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, noJumpLayer);
        ResetJumps();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((groundLayer & (1 << collision.gameObject.layer)) != 0) // LayerMask kullanýyorsanýz
        {
            CheckIfGrounded();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        CheckIfGrounded();

    }

    private void ResetJumps()
    {
        if (isGrounded)
        {
            // Yere temas edildiyse zýplama sayýsýný sýfýrla
            jumpsLeft = maxJumps;
            inputCanvas.JumpsLeft = jumpsLeft;
        }
    }

    #endregion
}
