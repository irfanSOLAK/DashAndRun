using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


/// <summary>
/// Any class that listen an event must derived from Listener. It must have methods same name with the listened enum and event. 
/// (e.g. If a class listen Countdown event from GameEvents Enum
/// it must have a method named GameEventsCountdown as public. When the event triggered GameEventsCountdown method is called.)
/// </summary>
public abstract class Listener : MonoBehaviour, IListener
{
    public abstract void AddThisToEventListener();
    public abstract void RemoveThisFromEventListener();

    public virtual void OnEnable()
    {
        AddThisToEventListener();
    }

    public virtual void OnDisable()
    {
        RemoveThisFromEventListener();
    }

    public void OnEventOccured(Enum eventName, params object[] parameters)
    {
        if (eventName == null)
        {
            throw new ArgumentNullException(nameof(eventName));
        }

        string methodName = eventName.GetType().Name + eventName.ToString();
        MethodInfo theMethod = this.GetType().GetMethod(methodName);

        if (theMethod == null)
        {
            throw new ArgumentException($"Method '{methodName}' does not exist in type '{this.GetType().Name}'.");
        }

        theMethod.Invoke(this, parameters);
    }
}