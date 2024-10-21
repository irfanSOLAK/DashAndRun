using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSounds : Listener
{
    #region SoundNames
    private readonly string JumpSound = "Jump";
    private readonly string DoubleJumpSound = "DoubleJump";
    #endregion


    public void PlaySound(string soundName)
    {
        GameBehaviour.Instance.Audio.PlaySound(soundName);
    }

    #region Listener Methods

    public void CharacterStateFall()
    {

    }

    public void CharacterStateHit()
    {

    }

    public void CharacterStateIdle()
    {

    }

    public void CharacterStateDoubleJump()
    {
        PlaySound(DoubleJumpSound);
    }
    public void CharacterStateJump()
    {
        PlaySound(JumpSound);
    }

    public void CharacterStateRun()
    {

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