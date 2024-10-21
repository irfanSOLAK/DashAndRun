using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    #region AnimatorHash

    private readonly int enterCheckPointHash = Animator.StringToHash("EnterCheckpoint");
    private readonly int activatedCheckPointHash = Animator.StringToHash("ActivatedCheckpoint");

    #endregion

    [SerializeField] Animator animator;
    [SerializeField] bool isActivated = false;

    private void ProcessCheckpoint()
    {
        if (isActivated) return;
        PlayAnimations();
        GameBehaviour.Instance.Audio.PlaySound("Checkpoint");
        GameBehaviour.Instance.Notifications.PostNotification(RespawnLocation.New, gameObject);
        isActivated = true;
    }

    void PlayAnimations()
    {
        StartCoroutine(PlayAnimationsCoroutine());
    }

    IEnumerator PlayAnimationsCoroutine()
    {
        float animationClipLength = animator.runtimeAnimatorController.animationClips[0].length;
        animator.Play(enterCheckPointHash);

        // �lk animasyonun bitmesini bekleyin
        yield return new WaitForSeconds(animationClipLength); // Bu s�re animasyon s�renize g�re ayarlanabilir

        animator.Play(activatedCheckPointHash);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ProcessCheckpoint();
        }
    }
}
