using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecutionOrder(30)]
public class GameStart : Listener, IExecutionOrder
{

    [Header("======================= Game Settings =======================")]
    [SerializeField] private float waitingTimeForGameStart = 0.3f;


    [Header("======================= Prefab Path =======================")]
    [SerializeField] private string playerPrefabPath = "Prefabs/NotActive/Pinkman";
    [SerializeField] private string AppearAnimationPath = "Prefabs/NotActive/CharacterAppearingPrefab";
    [SerializeField] private string levelLoaderAnimationPath = "Prefabs/NotActive/LevelLoader";
    [SerializeField] private string inputCanvasPath = "Prefabs/NotActive/InputCanvas";


    #region Properties
    public GameObject Player { get; set; }
    public GameObject LevelLoaderAnimation { get; set; }
    #endregion


    #region Prefabs
    private GameObject playerPrefab;
    private GameObject appearAnimationPrefab;
    private GameObject levelLoaderAnimationPrefab;
    private GameObject inputCanvasPrefab;
    #endregion


    #region Animation Objects
    private GameObject appearAnimation;
    private float appearAnimationLength;
    private float levelLoaderStartAnimLength;
    #endregion


    private NotificationManager notification;
    private GameObject inputCanvas;

    public void ManagedAwake()
    {
        LoadPrefabs();
        InitializeObjects();
        SetAnimationLengths();
    }

    #region Initialization Routines

    private void LoadPrefabs()
    {
        playerPrefab = Resources.Load<GameObject>(playerPrefabPath);
        appearAnimationPrefab = Resources.Load<GameObject>(AppearAnimationPath);
        levelLoaderAnimationPrefab = Resources.Load<GameObject>(levelLoaderAnimationPath);
        inputCanvasPrefab = Resources.Load<GameObject>(inputCanvasPath);
    }
    private void InitializeObjects()
    {
        notification = GameBehaviour.Instance.Notifications;

        LevelLoaderAnimation = Instantiate(levelLoaderAnimationPrefab);
        Player = Instantiate(playerPrefab);
        appearAnimation = Instantiate(appearAnimationPrefab);
        inputCanvas = Instantiate(inputCanvasPrefab);
    }
    private void SetAnimationLengths()
    {
        levelLoaderStartAnimLength = GetAnimationLength(LevelLoaderAnimation, 0, 0);
        appearAnimationLength = GetAnimationLength(appearAnimationPrefab, 0, 0);
    }
    private float GetAnimationLength(GameObject animationProviderParent, int providerChildIndex, int animIndex)
    {
        Animator animator = animationProviderParent.transform.GetChild(providerChildIndex).GetComponent<Animator>();
        float animationClipLength = animator.runtimeAnimatorController.animationClips[animIndex].length;
        return animationClipLength;
    }

    #endregion

    // Start is called before the first frame update
    public void Start()
    {
        StartCoroutine(InitializeGameSequence());
    }

    private IEnumerator InitializeGameSequence()
    {
        yield return StartCoroutine(StartGame());
        notification.PostNotification(GameState.Start);
    }

    public IEnumerator StartGame()
    {
        StartCoroutine(PlayLevelLoaderAnim());
        yield return StartCoroutine(PlayAppearanceAnimation(ActivatePlayer));
        inputCanvas.SetActive(true);
        notification.PostNotification(HealthStatus.Alive);
    }

    private IEnumerator PlayLevelLoaderAnim()
    {
        LevelLoaderAnimation.SetActive(true);
        yield return new WaitForSeconds(levelLoaderStartAnimLength); // Bu süre animasyon sürenize göre ayarlanabilir
        LevelLoaderAnimation.SetActive(false);
    }

    private IEnumerator PlayAppearanceAnimation(System.Action onComplete) // öðrenem diye böyle kaldý
    {
        yield return new WaitForSeconds(waitingTimeForGameStart);
        appearAnimation.transform.position = GetComponent<SpawnPoints>().SpawnTransform.position;
        GameBehaviour.Instance.Audio.PlaySound("PlayerSpawnEffect");
        appearAnimation.SetActive(true);
        yield return new WaitForSeconds(appearAnimationLength);
        appearAnimation.SetActive(false);
        onComplete?.Invoke();
    }

    private void ActivatePlayer()
    {
        Player.SetActive(true);
    }

    #region Listener Methods

    public void GameStatePause()
    {
        inputCanvas.SetActive(false);
    }

    public void GameStatePauseContinue()
    {
        inputCanvas.SetActive(true);
    }

    #endregion


    #region Listener Implementation
    public override void AddThisToEventListener()
    {
        var notifications = GameBehaviour.Instance.Notifications;
        notifications.AddListener(GameState.Pause, this);
        notifications.AddListener(GameState.PauseContinue, this);
    }

    public override void RemoveThisFromEventListener()
    {
        var notifications = GameBehaviour.Instance.Notifications;
        notifications.RemoveListener(GameState.Pause, this);
        notifications.RemoveListener(GameState.PauseContinue, this);
    }
    #endregion
}
