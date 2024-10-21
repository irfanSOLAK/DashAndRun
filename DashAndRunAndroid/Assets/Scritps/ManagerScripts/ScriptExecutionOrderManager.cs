using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScriptExecutionOrderManager : MonoBehaviour, IManagerModule
{
    public int ManagerExecutionOrder => 50;

    public void OnModuleAwake()
    {

    }

    public void InitializeScriptsInOrder()
    {
        var scripts = GetScriptsImplementingIExecutionOrder();
        var executionOrders = GetExecutionOrders(scripts);
        ExecuteScriptsInOrder(executionOrders);
    }

    private List<IExecutionOrder> GetScriptsImplementingIExecutionOrder()
    {
        return FindObjectsOfType<MonoBehaviour>()
            .OfType<IExecutionOrder>()
            .ToList();
    }

    private Dictionary<IExecutionOrder, int> GetExecutionOrders(List<IExecutionOrder> scripts)
    {
        Dictionary<IExecutionOrder, int> executionOrders = new Dictionary<IExecutionOrder, int>();

        foreach (var script in scripts)
        {
            var attribute = (ExecutionOrderAttribute)Attribute.GetCustomAttribute(script.GetType(), typeof(ExecutionOrderAttribute));
            if (attribute != null)
            {
                executionOrders.Add(script, attribute.Order);
            }
            else
            {
                int order = attribute?.Order ?? new ExecutionOrderAttribute().Order; // Varsayýlan deðer al
                executionOrders.Add(script, order);
                Debug.LogWarning($"{script.GetType().Name} does not have an ExecutionOrderAttribute. Default execution order {order} will be used.");
            }
        }

        return executionOrders;
    }

    private void ExecuteScriptsInOrder(Dictionary<IExecutionOrder, int> executionOrders)
    {
        if (executionOrders == null) return; // Hata durumunu kontrol et

        var orderedScripts = executionOrders.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();

        foreach (var script in orderedScripts)
        {
            script.ManagedAwake(); // Burada Initialize çaðrýlýyor
        }
    }
}