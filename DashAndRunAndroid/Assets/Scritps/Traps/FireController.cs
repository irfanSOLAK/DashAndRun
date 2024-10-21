using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{

    [SerializeField] private GameObject damageArea;
    [SerializeField] private float activationDelay;
    [SerializeField] private float activeTime;
    [SerializeField] Animator animator;

    private bool isTriggered;
    private bool isActive;
    private bool isFireTouching;

    private void PostHit()
    {
        isFireTouching = CheckIfTouchesFire();

        if (isFireTouching)
        {
            GameBehaviour.Instance.Notifications.PostNotification(HealtSystemManager.Hit);
        }
        isFireTouching = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("PlayerGround"))
        {
            if (!isTriggered)
            {
                StartCoroutine(TriggerFireSequence());
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("PlayerGround"))
        {
            if (isActive)
            {
                PostHit();
            }
        }

    }

    private IEnumerator TriggerFireSequence()
    {
        yield return StartCoroutine(HandleActivationDelay());
        yield return StartCoroutine(ActivateFireEffects());
        ResetFireState();
    }

    private IEnumerator HandleActivationDelay()
    {
        isTriggered = true;
        animator.SetBool("IsHit", isTriggered);
        yield return new WaitForSeconds(activationDelay);
    }

    private IEnumerator ActivateFireEffects()
    {
        isActive = true;
        animator.SetBool("IsActivated", isActive);
        GameBehaviour.Instance.Audio.PlaySound("Fire");
        PostHit();
        yield return new WaitForSeconds(activeTime);
    }

    private void ResetFireState()
    {
        isActive = false;
        isTriggered = false;
        animator.SetBool("IsHit", isTriggered);
        animator.SetBool("IsActivated", isActive);
    }

    private bool CheckIfTouchesFire()
    {
        CapsuleCollider2D collider = damageArea.GetComponent<CapsuleCollider2D>();
        return Physics2D.OverlapCapsule(
            damageArea.transform.position,
            collider.size,
            collider.direction,
            0f,
            LayerMask.GetMask("Player")
        );
    }
}
