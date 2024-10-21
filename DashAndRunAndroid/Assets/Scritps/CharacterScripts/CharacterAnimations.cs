using UnityEngine;

public class CharacterAnimations : Listener
{
    #region Serialized Fields
    [SerializeField] private Animator animator;  // Animator bileþeni
    #endregion

    #region AnimatorHash
    private readonly int IdleHash = Animator.StringToHash("Idle");
    private readonly int RunHash = Animator.StringToHash("Run");
    private readonly int JumpHash = Animator.StringToHash("Jump");
    private readonly int DoubleJumpHash = Animator.StringToHash("DoubleJump");
    private readonly int FallHash = Animator.StringToHash("Fall");
    private readonly int HitHash = Animator.StringToHash("Hit");
    #endregion


    private void SetAnimationState(int animID)
    {
        //  float animationClipLength = animator.runtimeAnimatorController.animationClips[0].length;
        // animator.Play(animID, 0, animationClipLength);
        animator.CrossFade(animID, 0.2f, 0, 0.5f);
    }

    #region Listener Methods

    public void CharacterStateFall()
    {
        SetAnimationState(FallHash);
    }

    public void CharacterStateHit()
    {
        SetAnimationState(HitHash);
    }

    public void CharacterStateIdle()
    {
        SetAnimationState(IdleHash);
    }

    public void CharacterStateDoubleJump()
    {
        SetAnimationState(DoubleJumpHash);
    }
    public void CharacterStateJump()
    {
        SetAnimationState(JumpHash);
    }

    public void CharacterStateRun()
    {
        SetAnimationState(RunHash);
    }

    #endregion

    #region Listener Implementation
    public override void AddThisToEventListener()
    {
        var notifications = GameBehaviour.Instance.Notifications;
        foreach (CharacterState state in System.Enum.GetValues(typeof(CharacterState)))
        {
            notifications.AddListener(state, this);
        }
    }

    public override void RemoveThisFromEventListener()
    {
        var notifications = GameBehaviour.Instance.Notifications;
        foreach (CharacterState state in System.Enum.GetValues(typeof(CharacterState)))
        {
            notifications.RemoveListener(state, this);
        }
    }
    #endregion
}