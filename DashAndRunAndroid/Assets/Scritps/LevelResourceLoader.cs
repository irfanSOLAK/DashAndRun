using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelResourceLoader : MonoBehaviour, ITask
{
    public ResourceData resourceData;

    private void Awake()
    {
        if (resourceData == null)
        {
            Debug.LogError("ResourceData is not assigned!");
            return;
        }

        RegisterTask();
        HashSet<int> instantiatedPrefabIds = GetInstantiatedPrefabIds();
        LoadPrefabs(instantiatedPrefabIds);
        SetTaskCompleted();
    }

    #region Instantiate resourceData

    private HashSet<int> GetInstantiatedPrefabIds()
    {
        HashSet<int> instantiatedPrefabIds = new HashSet<int>();
        foreach (var obj in FindObjectsOfType<GameObject>())
        {
            var prefabComponent = obj.GetComponent<PrefabIdentifier>();
            if (prefabComponent != null)
            {
                instantiatedPrefabIds.Add(prefabComponent.id);
            }
        }
        return instantiatedPrefabIds;
    }

    private void LoadPrefabs(HashSet<int> instantiatedPrefabIds)
    {
        foreach (PrefabData prefabData in resourceData.prefabs)
        {
            if (prefabData.prefab != null)
            {
                if (!instantiatedPrefabIds.Contains(prefabData.id))
                {
                    InstantiatePrefab(prefabData);
                }
                else
                {
                    Debug.LogWarning($"{prefabData.prefab.name} is already placed in the scene with ID {prefabData.id} and description \"{prefabData.description}\".");
                }
            }
            else
            {
                Debug.LogWarning("Prefab is null in ResourceData.");
            }
        }
    }

    private void InstantiatePrefab(PrefabData prefabData)
    {
        GameObject instantiatedPrefab = Instantiate(prefabData.prefab);
        Debug.LogWarning($"{prefabData.prefab.name} instantiated with ID {prefabData.id} and description \"{prefabData.description}\".");
    }
    #endregion

    #region Task Management

    public void RegisterTask()
    {
        GameBehaviour.Instance.Task.RegisterTask(GameBehaviour.Instance.ExecutionOrder.InitializeScriptsInOrder, this);
    }

    public void SetTaskCompleted()
    {
       GameBehaviour.Instance.Task.MarkTaskCompleted(GameBehaviour.Instance.ExecutionOrder.InitializeScriptsInOrder,this);
    }

    #endregion
}
