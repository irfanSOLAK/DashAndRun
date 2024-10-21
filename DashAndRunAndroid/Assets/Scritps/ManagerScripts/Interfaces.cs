using System;

#region Manager Module

/// <summary>
/// This interface is designed for manager modules that can be added 
/// to the GameBehaviour class. 
/// 
/// <para>Only classes implementing this interface can be used as 
/// managers.</para>
/// </summary>
public interface IManagerModule
{
    /// <summary>
    /// Called during initialization to set up the module, 
    /// replacing the use of Awake and OnEnable methods.
    /// </summary>
    void OnModuleAwake(); // Method to initialize the module

    /// <summary>
    /// Defines the order in which modules are initialized, 
    /// with the smallest value being initialized first.
    /// </summary>
    int ManagerExecutionOrder { get; } // Order in which the module is executed
}

#endregion

#region Task System

/// <summary>
/// Interface for defining a task in the task management system.
/// This interface provides methods to register tasks and set them as completed.
/// 
/// Note: The current registration order is not fixed. 
/// In the future, a method can be added to register all tasks at once, 
/// or this process can be handled in the TaskManager's OnModuleAwake method.
/// </summary>
public interface ITask
{
    /// <summary>
    /// Registers the task within the task management system.
    /// This method should be called to initialize the task and prepare it for execution.
    /// </summary>
    void RegisterTask();

    /// <summary>
    /// Marks the task as completed.
    /// This method should be called when the task has finished executing.
    /// </summary>
    void SetTaskCompleted();
}

#endregion

#region Execution Order Interface

/// <summary>
/// Interface for defining the execution order of scripts.
/// 
/// This interface is intended for scripts that require a specific 
/// initialization sequence. Implementing this interface allows 
/// for controlled execution of the <see cref="ManagedAwake"/> 
/// method, ensuring that scripts are initialized in the 
/// desired order during gameplay.
/// 
/// <para> 
/// The order of execution can be defined using the <see cref="ExecutionOrderAttribute"/>. 
/// Scripts with lower execution order values are initialized first, 
/// allowing for precise control over the startup sequence.
/// </para>
/// </summary>
public interface IExecutionOrder
{
    /// <summary>
    /// Called to initialize the script in a controlled manner, 
    /// replacing the standard Awake method. 
    /// 
    /// This method should contain any setup logic that needs 
    /// to occur before the script is executed.
    /// </summary>
    void ManagedAwake();
}

#endregion

public interface IListener
{
    void OnEventOccured(Enum eventName, params object[] parameters);
}