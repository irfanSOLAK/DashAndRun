using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class GameOverCanvas : Listener, IExecutionOrder
{
    [SerializeField] private GameObject buttonPanel;
    [SerializeField] private Button rewardButton;
    [SerializeField] private TMP_Text networkWarning;
    [SerializeField] private float internetCheckInterval = 5f;

    public void ManagedAwake()
    {

    }

    private void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }

    public void Home()
    {
        StartCoroutine(LoadSceneAfterDelay("MainMenu"));
    }

    public void Replay()
    {
        StartCoroutine(LoadSceneAfterDelay(SceneManager.GetActiveScene().name));
    }

    public IEnumerator LoadSceneAfterDelay(string sceneName)
    {
        yield return new WaitForSeconds(GameBehaviour.Instance.Audio.PlaySound("Button"));
        SceneManager.LoadScene(sceneName);
    }


    public void WatchVideoAd()
    {
        GameBehaviour.Instance.Reklam.ShowRewardedAd();
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
        buttonPanel.SetActive(false);
    }

    #region Internet Connection Check

    private IEnumerator CheckInternetConnection()
    {
        while (true) // Sürekli döngü
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get("http://www.google.com"))
            {
                // Ulaþým süresini bekle
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                    webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    // Ýnternet yoksa butonu devre dýþý býrak
                    rewardButton.interactable = false;
                    networkWarning.gameObject.SetActive(true); // Uyarý mesajýný göster
                }
                else
                {
                    // Ýnternet varsa butonu etkinleþtir
                    rewardButton.interactable = true;
                    networkWarning.gameObject.SetActive(false); // Uyarý mesajýný gizle
                }
            }
            yield return new WaitForSeconds(internetCheckInterval); // Belirtilen süre kadar bekle
        }
    }

    #endregion

    #region Listener Methods

    public void GameStateStart()
    {
        buttonPanel.SetActive(false);
        StopCoroutine(CheckInternetConnection());
    }

    public void HealthStatusDead()
    {
        GameBehaviour.Instance.Reklam.ShowInterAdOnEvenScene(SceneManager.GetActiveScene().buildIndex);
        buttonPanel.SetActive(true);
        StartCoroutine(CheckInternetConnection());
    }

    #endregion

    #region Listener Implementation

    public override void AddThisToEventListener()
    {
        var notifications = GameBehaviour.Instance.Notifications;
        notifications.AddListener(GameState.Start, this);
        notifications.AddListener(HealthStatus.Dead, this);
    }

    public override void RemoveThisFromEventListener()
    {
        var notifications = GameBehaviour.Instance.Notifications;
        notifications.RemoveListener(GameState.Start, this);
        notifications.RemoveListener(HealthStatus.Dead, this);
    }

    #endregion

}
