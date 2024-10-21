using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour,IManagerModule
{
    private Dictionary<Action, List<ITask>> methodDictionary = new Dictionary<Action, List<ITask>>();

    public int ManagerExecutionOrder => 40;

    public void OnModuleAwake()
    {
 
    }

    public void RegisterTask(Action action, ITask task)
    {
        if (!methodDictionary.ContainsKey(action))
            methodDictionary.Add(action, new List<ITask>());

        methodDictionary[action].Add(task);
    }

    public void MarkTaskCompleted(Action action, ITask task)
    {
        if (!methodDictionary.ContainsKey(action))
            return;

        for (int i = methodDictionary[action].Count - 1; i >= 0; i--)
        {
            if (methodDictionary[action][i] == task)
            {
                methodDictionary[action].RemoveAt(i);
                CheckIfAllTasksCompleted(action);
            }
        }
    }

    private void CheckIfAllTasksCompleted(Action action)
    {
        if (methodDictionary[action].Count == 0)
        {
            ExecuteScript(action); // Tüm görevler tamamlandýðýnda çaðýr
            RemoveAction(action);
        }
    }

    private void ExecuteScript(Action action)
    {
        action?.Invoke();
    }

    private void RemoveAction(Action action)
    {
        methodDictionary.Remove(action);
    }
}