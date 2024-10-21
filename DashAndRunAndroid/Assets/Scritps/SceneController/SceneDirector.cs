using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecutionOrder(40)]
public class SceneDirector : Listener, IExecutionOrder
{
    [Header("======================= Prefab Path =======================")]
    [SerializeField] private string playerDisappearAnimationPath = "Prefabs/NotActive/CharacterDesappearingPrefab";


    private GameObject playerDisappearAnimation;
    private GameObject disappearAnim;
    private GameObject LevelLoaderAnimation;


    private float disappearAnimLength;
    private float levelLoaderEndAnimLength;
    private Vector3 camToSpawnPointOffset;

    private bool isHitProcessing=false;
    private bool shouldContinue=false;

    public void ManagedAwake()
    {
        playerDisappearAnimation = Resources.Load<GameObject>(playerDisappearAnimationPath);
        camToSpawnPointOffset = Camera.main.transform.position - GetComponent<SpawnPoints>().SpawnTransform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        disappearAnim = Instantiate(playerDisappearAnimation);

        disappearAnimLength = GetAnimationLength(disappearAnim, 0, 0);

        LevelLoaderAnimation = GetComponent<GameStart>().LevelLoaderAnimation;
        levelLoaderEndAnimLength = GetAnimationLength(LevelLoaderAnimation, 0, 1);
    }

    private float GetAnimationLength(GameObject animationProviderParent, int providerChildIndex, int animIndex)
    {
        Animator animator = animationProviderParent.transform.GetChild(providerChildIndex).GetComponent<Animator>();
        float animationClipLength = animator.runtimeAnimatorController.animationClips[animIndex].length;
        return animationClipLength;
    }

    
    private IEnumerator RestartGame()
    {
        yield return StartCoroutine(KillPlayer());
        StartAgain();
    }

    private IEnumerator ProcessHit()
    {
        yield return StartCoroutine(KillPlayer());
        isHitProcessing = false;

        if (shouldContinue)
        {
            shouldContinue = false;
            GameStateContinue();
        }
    }

    private IEnumerator KillPlayer()
    {
        GetComponent<GameStart>().Player.SetActive(false);
        StartCoroutine(PlayDisappear());
        yield return StartCoroutine(PlayLevelLoaderAnim());
    }

    IEnumerator PlayDisappear()
    {
        disappearAnim.transform.position = GetComponent<GameStart>().Player.transform.position;
        disappearAnim.SetActive(true);
        GameBehaviour.Instance.Audio.PlaySound("Death");
        yield return new WaitForSeconds(disappearAnimLength);
        disappearAnim.SetActive(false);
    }

    IEnumerator PlayLevelLoaderAnim()
    {
        LevelLoaderAnimation.SetActive(true);
        Animator animator = LevelLoaderAnimation.transform.GetChild(0).GetComponent<Animator>();
        animator.Play("Crossfade_End");
        yield return new WaitForSeconds(levelLoaderEndAnimLength);
        LevelLoaderAnimation.SetActive(false);
    }
    private void StartAgain()
    {
        Camera.main.transform.position = GetComponent<SpawnPoints>().SpawnTransform.position + camToSpawnPointOffset;
        GetComponent<GameStart>().Player.transform.position = GetComponent<SpawnPoints>().SpawnTransform.position;
        StartCoroutine(GetComponent<GameStart>().StartGame());
    }

    private IEnumerator ProcessFinished()
    {
       yield return StartCoroutine(KillPlayer());
        SceneManager.LoadScene(0);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        Reklamlar.OnRewardReceived += AdsReward;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        Reklamlar.OnRewardReceived -= AdsReward;
    }

    public void AdsReward()
    {
        StartAgain();
    }

    #region Listener Methods

    public void HealthStatusHit()
    {
        if (!isHitProcessing)
        {
            isHitProcessing = true;
            StartCoroutine(ProcessHit());
        }
    }

    public void HealthStatusDead()
    {
        StartCoroutine(KillPlayer());
    }
    public void GameStateContinue()
    {
        if (isHitProcessing)
        {
            shouldContinue = true;
        }
        else
        {
            StartAgain();
        }      
    }
    public void GameStateFinished()
    {
        StartCoroutine(ProcessFinished());
    }

    #endregion

    #region Listener Implementation

    public override void AddThisToEventListener()
    {
        var notifications = GameBehaviour.Instance.Notifications;
        notifications.AddListener(HealthStatus.Hit, this);
        notifications.AddListener(HealthStatus.Dead, this);
        notifications.AddListener(GameState.Continue, this);
        notifications.AddListener(GameState.Finished, this);
    }

    public override void RemoveThisFromEventListener()
    {
        var notifications = GameBehaviour.Instance.Notifications;
        notifications.RemoveListener(HealthStatus.Hit, this);
        notifications.RemoveListener(HealthStatus.Dead, this);
        notifications.RemoveListener(GameState.Continue, this);
        notifications.RemoveListener(GameState.Finished, this);
    }
    #endregion
}