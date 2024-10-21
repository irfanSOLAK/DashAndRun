using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : Listener, IExecutionOrder
{
    [field: SerializeField] private int MaxHealth { get; set; } = 3;
    [field: SerializeField] public int CurrentHealth { get; set; }


    [SerializeField] private float hitCooldown = 1.0f; // Cooldown period in seconds
    private float lastHitTime = -Mathf.Infinity; // Initialize to a value that ensures the first hit is allowed

    public void ManagedAwake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        Reklamlar.OnRewardReceived += AdsReward;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        Reklamlar.OnRewardReceived -= AdsReward;
    }

    public void AdsReward()
    {
        CurrentHealth = MaxHealth;
    }

    #region Listener Methods

    public void HealtSystemManagerHit()
    {
        if (Time.time - lastHitTime >= hitCooldown)
        {
            lastHitTime = Time.time;
            CurrentHealth--;
            GameBehaviour.Instance.Notifications.PostNotification(HealthStatus.Hit);
            if (CurrentHealth > 0)
            {
                GameBehaviour.Instance.Notifications.PostNotification(GameState.Continue);
            }
            else
            {
                CurrentHealth = 0;
                GameBehaviour.Instance.Notifications.PostNotification(HealthStatus.Dead);
            }
        }
        else
        {
            Debug.Log("Hit ignored due to cooldown");
        }
    }

    #endregion

    #region Listener Implementation
    public override void AddThisToEventListener()
    {
        var notifications = GameBehaviour.Instance.Notifications;
        foreach (HealtSystemManager state in System.Enum.GetValues(typeof(HealtSystemManager)))
        {
            notifications.AddListener(state, this);
        }
    }

    public override void RemoveThisFromEventListener()
    {
        var notifications = GameBehaviour.Instance.Notifications;
        foreach (HealtSystemManager state in System.Enum.GetValues(typeof(HealtSystemManager)))
        {
            notifications.RemoveListener(state, this);
        }
    }
    #endregion
}