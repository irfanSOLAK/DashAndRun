using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartCanvas : Listener, IExecutionOrder
{
    [Header("======================= Heart Settings =======================")]
    [SerializeField] private GameObject heart;
    [SerializeField] private Sprite heartSprite;
    [SerializeField] private string heartFullName = "HeartFull";


    [Header("======================= Spawn Settings =======================")]
    [SerializeField] private Transform spawnParent;


    private List<GameObject> heartList = new List<GameObject>();

    private int CurrentHealth => GameObject.FindWithTag("SceneController").GetComponent<HealthSystem>().CurrentHealth;

    public void ManagedAwake()
    {

    }

    private void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }

    private void CreateHearts(int amount)
    {
        // Kalp prefab'ýnýn RectTransform'ýný alýn
        RectTransform heartRectTransform = heart.GetComponent<RectTransform>();

        // Kalp prefab'ýnýn geniþliðini spacing olarak ayarlayýn
        float spacing = heartRectTransform.rect.width;

        // Ýlk kalp objesinin konumunu hesapla
        // heartPrefab'ýn RectTransform'ýnýn mevcut konumunu referans olarak kullan
        Vector2 startPosition = heartRectTransform.anchoredPosition;

        for (int i = 0; i < amount; i++)
        {
            // Yeni kalp objesini oluþturun ve spawnParent'ýn altýna yerleþtirin
            GameObject newHeart = Instantiate(heart, spawnParent);
            RectTransform newHeartRectTransform = newHeart.GetComponent<RectTransform>();

            // Yeni kalbin konumunu hesapla (yeni kalp, mevcut kalpten saða doðru yerleþtirilecek)
            Vector2 newPosition = startPosition + new Vector2(i * spacing, 0);
            newHeartRectTransform.anchoredPosition = newPosition;
            newHeart.SetActive(true);
            // Kalp objesini listeye ekleyin
            heartList.Add(newHeart);
        }
    }

    private IEnumerator PlayEmptyAnimation(int index)
    {
        GetComponent<Canvas>().sortingOrder = 1;
        GameObject heartToHit = heartList[index];
        Transform heartFullTransform = heartToHit.transform.Find(heartFullName);
        Animator animator = heartFullTransform.GetComponent<Animator>();

        float animationClipLength = animator.runtimeAnimatorController.animationClips[0].length;
        animator.Play("Empty");

        yield return new WaitForSeconds(animationClipLength);

        heartFullTransform.gameObject.SetActive(false);

        GetComponent<Canvas>().sortingOrder = 0;
    }


    private void ResetHearts()
    {
        foreach (GameObject heart in heartList)
        {
            heart.transform.Find(heartFullName).gameObject.SetActive(true);
        }
    }

    #region Admob Ads

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
        ResetHearts();
    }

    #endregion

    #region Listener Methods
    public void GameStateStart()
    {
        CreateHearts(CurrentHealth);
    }

    public void HealthStatusAlive()
    {

    }

    public void HealthStatusHit()
    {
        StartCoroutine(PlayEmptyAnimation(CurrentHealth));
    }
    #endregion

    #region Listener Implementation
    public override void AddThisToEventListener()
    {
        var notifications = GameBehaviour.Instance.Notifications;
        notifications.AddListener(HealthStatus.Hit, this);
        notifications.AddListener(HealthStatus.Alive, this);
        notifications.AddListener(GameState.Start, this);
    }

    public override void RemoveThisFromEventListener()
    {
        var notifications = GameBehaviour.Instance.Notifications;
        notifications.RemoveListener(HealthStatus.Hit, this);
        notifications.RemoveListener(HealthStatus.Alive, this);
        notifications.RemoveListener(GameState.Start, this);
    }
    #endregion
}