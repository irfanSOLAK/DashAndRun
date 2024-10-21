using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecutionOrder(10)]
public class SpawnPoints : Listener, IExecutionOrder
{
    public Transform SpawnTransform { get; set; }

    [SerializeField] private GameObject spawnPointsParent;
    public List<CheckpointData> gameObjectStatus = new List<CheckpointData>();

    public void ManagedAwake()
    {
        spawnPointsParent=GameObject.FindWithTag("SpawnPointsParent");
        AddAllChildsToList();
        SortList();
        SpawnTransform = gameObjectStatus[0].CheckPoint.transform;
        gameObjectStatus[0].IsActive = true;
    }

    private void AddAllChildsToList()
    {
        foreach (Transform child in spawnPointsParent.transform)
        {
            CheckpointData status = new CheckpointData
            {
                CheckPoint = child.gameObject,
                IsActive = false
            };
            gameObjectStatus.Add(status);
        }
    }

    private void SortList()
    {
        gameObjectStatus.Sort((a, b) => a.CheckPoint.transform.position.x.CompareTo(b.CheckPoint.transform.position.x));
    }

    private void SetCheckPoint(GameObject checkPoint)
    {
        gameObjectStatus.Find(data => data.CheckPoint == checkPoint).IsActive=true;
        SpawnTransform = gameObjectStatus.FindLast(data => data.IsActive).CheckPoint.transform;
    }
    

    #region Listener Methods

    public void RespawnLocationNew(GameObject checkPoint)
    {
        SetCheckPoint(checkPoint);

    }

    #endregion


    #region Listener Implementation
    public override void AddThisToEventListener()
    {
        GameBehaviour.Instance.Notifications.AddListener(RespawnLocation.New, this);
    }

    public override void RemoveThisFromEventListener()
    {
        GameBehaviour.Instance.Notifications.RemoveListener(RespawnLocation.New, this);
    }
    #endregion
}

[System.Serializable]
public class CheckpointData
{
    public GameObject CheckPoint;
    public bool IsActive;
}
