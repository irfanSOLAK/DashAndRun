using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [SerializeField] private float bounceForce = 10f;
    [SerializeField] private Animator animator;

    private void ApplyBounceEffect(Collider2D collision)
    {
        Rigidbody2D rb = collision.GetComponentInParent<Rigidbody2D>();
        Vector2 bounceDirection = (Vector2)transform.up.normalized;
        Vector2 newVelocity = new Vector2(bounceDirection.x * bounceForce, bounceDirection.y * bounceForce);
        rb.velocity = newVelocity;
        GameBehaviour.Instance.Audio.PlaySound("TrambolineJump");
        animator.SetTrigger("JumpTrigger");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerGround") || collision.CompareTag("PlayerSideGround"))
        {
            ApplyBounceEffect(collision);
        }
    }
}
