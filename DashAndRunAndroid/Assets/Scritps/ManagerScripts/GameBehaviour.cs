using System;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(NotificationManager))]              // Exectuion order 10
[RequireComponent(typeof(ViewportScaler))]                   // Exectuion order 20
[RequireComponent(typeof(AudioManager))]                     // Exectuion order 30
[RequireComponent(typeof(TaskManager))]                      // Exectuion order 40
[RequireComponent(typeof(ScriptExecutionOrderManager))]      // Exectuion order 50
[RequireComponent(typeof(Reklamlar))]                        // Exectuion order 100

public class GameBehaviour : Singleton<GameBehaviour>
{
    #region Task Manager

    private TaskManager taskManager = null;
    public TaskManager Task
    {
        get
        {
            if (taskManager == null) taskManager = Instance.GetComponent<TaskManager>();
            return taskManager;
        }
    }

    #endregion

    #region Script Execution Order

    private ScriptExecutionOrderManager executionOrder = null;
    public ScriptExecutionOrderManager ExecutionOrder
    {
        get
        {
            if (executionOrder == null) executionOrder = Instance.GetComponent<ScriptExecutionOrderManager>();
            return executionOrder;
        }
    }

    #endregion

    #region NotificationManager

    private NotificationManager _notifications = null;
    public NotificationManager Notifications
    {
        get
        {
            if (_notifications == null) _notifications = Instance.GetComponent<NotificationManager>();
            return _notifications;
        }
    }

    #endregion

    #region AudioManager

    private AudioManager _audioManager = null;
    public AudioManager Audio
    {
        get
        {
            if (_audioManager == null) _audioManager = Instance.GetComponent<AudioManager>();
            return _audioManager;
        }
    }

    #endregion

    #region Reklamlar

    private Reklamlar _reklam = null;
    public Reklamlar Reklam
    {
        get
        {
            if (_reklam == null) _reklam = Instance.GetComponent<Reklamlar>();
            return _reklam;
        }
    }

    #endregion

    public override void Awake()
    {
        base.Awake();
        //  AddMissingModules();
        InitializeModules();
    }

    #region Module Management

    /// <summary>
    /// RequireComponent yerine projedeki tüm IManagerModule leri otomatik ekliyor
    /// </summary>
    private void AddMissingModules()
    {
        // Tüm IManagerModule bileþenlerini bul
        var allModuleTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IManagerModule).IsAssignableFrom(type) && !type.IsAbstract);

        foreach (var moduleType in allModuleTypes)
        {
            // Eðer bu modül henüz eklenmemiþse, yeni bir örneðini oluþtur
            if (GetComponent(moduleType) == null)
            {
                gameObject.AddComponent(moduleType);
            }
        }
    }

    private void InitializeModules()
    {
        IManagerModule[] modules = GetComponents<IManagerModule>();
        System.Array.Sort(modules, (x, y) => x.ManagerExecutionOrder.CompareTo(y.ManagerExecutionOrder));

        foreach (var module in modules)
        {
            module.OnModuleAwake(); // Her modülün OnModuleAwake metodunu çaðýr
        }
    }

    #endregion

    #region Application Quit
    // OnDisable method in Listener class causes an error when exiting game on editor. This method is to avoid the error.
    private void OnApplicationQuit()
    {
        MonoBehaviour[] scripts = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
            script.enabled = false;
    }

    #endregion
}
