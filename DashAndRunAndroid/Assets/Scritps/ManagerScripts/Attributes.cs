using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class)]
public class ExecutionOrderAttribute : Attribute
{
    public int Order { get; }

    // Default constructor for default value
    public ExecutionOrderAttribute() : this(100) // Varsay�lan de�er 100
    {
    }

    // Constructor with order parameter
    public ExecutionOrderAttribute(int order)
    {
        Order = order;
    }
}

public class ReadOnlyAttribute : PropertyAttribute
{

}