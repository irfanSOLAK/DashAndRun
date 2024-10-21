using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NotificationManager : MonoBehaviour,IManagerModule
{
    private Dictionary<Enum, List<IListener>> _listeners = new Dictionary<Enum, List<IListener>>();

    public int ManagerExecutionOrder => 10;
    public void OnModuleAwake()
    {
        SceneManager.sceneLoaded += RemoveRedundancies;
    }

    public void AddListener(Enum eventName, IListener lObject)
    {
        if (!_listeners.ContainsKey(eventName))
            _listeners.Add(eventName, new List<IListener>());

        _listeners[eventName].Add(lObject);
    }

    public void RemoveListener(Enum eventName, IListener lObject)
    {
        if (!_listeners.ContainsKey(eventName))
            return;

        for (int i = _listeners[eventName].Count - 1; i >= 0; i--)
        {
            if ((_listeners[eventName][i] as Component).GetInstanceID() == (lObject as Component).GetInstanceID())
                _listeners[eventName].RemoveAt(i);
        }
    }

    public void PostNotification(Enum eventName, params object[] parameters)
    {
        if (!_listeners.ContainsKey(eventName))
        {
            Debug.Log(eventName.ToString() + " does not exist in the NotificationManager dictionary.");
            return;
        }

        foreach (IListener Listener in _listeners[eventName])
            Listener.OnEventOccured(eventName, parameters);
    }

    public void RemoveRedundancies(Scene scene, LoadSceneMode mode)
    {
        Dictionary<Enum, List<IListener>> TmpListeners = new Dictionary<Enum, List<IListener>>();

        foreach (KeyValuePair<Enum, List<IListener>> Item in _listeners)
        {
            for (int i = Item.Value.Count - 1; i >= 0; i--)
            {
                if (Item.Value[i] == null)
                    Item.Value.RemoveAt(i);
            }

            if (Item.Value.Count > 0)
                TmpListeners.Add(Item.Key, Item.Value);
        }

        _listeners = TmpListeners;
    }

}