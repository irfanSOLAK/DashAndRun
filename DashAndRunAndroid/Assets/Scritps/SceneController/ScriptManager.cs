using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecutionOrder(20)]
public class ScriptManager : Listener, IExecutionOrder
{

    [SerializeField] private List<MonoBehaviour> relatedScripts = new List<MonoBehaviour>();

    public List<MonoBehaviour> RelatedScripts
    {
        get { return relatedScripts; }
        set { relatedScripts = value; }
    }

    public void ManagedAwake()
    {
        DeActivateScripts();
    }

    private void ActivateScripts()
    {
        for (int i = 0; i < relatedScripts.Count; i++)
        {
            relatedScripts[i].enabled = true;
        }
    }

    private void DeActivateScripts()
    {
        for (int i = 0; i < relatedScripts.Count; i++)
        {
            relatedScripts[i].enabled = false;
        }
    }

    #region Listener Methods

    public void HealthStatusHit()
    {
        DeActivateScripts();
    }
    public void HealthStatusAlive()
    {
        ActivateScripts();
    }
    public void GameStateFinished()
    {
        DeActivateScripts();
    }
    #endregion

    #region Listener Implementation
    public override void AddThisToEventListener()
    {
        var notifications = GameBehaviour.Instance.Notifications;
        notifications.AddListener(HealthStatus.Hit, this);
        notifications.AddListener(HealthStatus.Alive, this);
        notifications.AddListener(GameState.Finished, this);

    }

    public override void RemoveThisFromEventListener()
    {
        var notifications = GameBehaviour.Instance.Notifications;
        notifications.RemoveListener(HealthStatus.Hit, this);
        notifications.RemoveListener(HealthStatus.Alive, this);
        notifications.RemoveListener(GameState.Finished, this);
    }
    #endregion
}
