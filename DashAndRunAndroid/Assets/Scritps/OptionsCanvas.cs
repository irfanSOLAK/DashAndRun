using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsCanvas : MonoBehaviour, IExecutionOrder
{
    [Header("==================== Menu Sound ====================")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject soundButton;
    [SerializeField] Sprite muteSprite;
    [SerializeField] Sprite unMuteSprite;
    [SerializeField] GameObject areYouSure;

    [Header("==================== Text ====================")]
    [SerializeField] private TMP_Text levelText;

    public void ManagedAwake()
    {

    }

    void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        levelText.text = $"{SceneManager.GetActiveScene().name.ToUpper()}";
        audioSource = GetAudioSource();
        SetMuteIcon();
    }

    public void Home()
    {
        GameBehaviour.Instance.Notifications.PostNotification(GameState.Pause);
        Time.timeScale = 0;
        GameBehaviour.Instance.Audio.PlaySound("Button");
        areYouSure.SetActive(true);
    }

    public void Yes()
    {
        StartCoroutine(LoadSceneAfterDelay("MainMenu"));
    }

    public void No()
    {
        Time.timeScale = 1;
        GameBehaviour.Instance.Audio.PlaySound("Button");
        GameBehaviour.Instance.Notifications.PostNotification(GameState.PauseContinue);
        areYouSure.SetActive(false);
    }

    public IEnumerator LoadSceneAfterDelay(string sceneName)
    {
        yield return new WaitForSecondsRealtime(GameBehaviour.Instance.Audio.PlaySound("Button"));
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }

    #region Sound

    private AudioSource GetAudioSource()
    {
        if (GetComponent<AudioSource>())
        {
            return GetComponent<AudioSource>();
        }

        return gameObject.AddComponent<AudioSource>();
    }

    private void SetMuteIcon()
    {
        bool isMuted = IsAudioMuted();
        Sprite spriteToUse = GetSoundButtonSprite(isMuted);
        UpdateSoundButtonIcon(spriteToUse);
    }

    private bool IsAudioMuted()
    {
        return PlayerPrefs.GetInt(GameBehaviour.Instance.Audio.MuteKey, 0) == 1;
    }

    private Sprite GetSoundButtonSprite(bool isMuted)
    {
        return isMuted ? muteSprite : unMuteSprite;
    }

    private void UpdateSoundButtonIcon(Sprite sprite)
    {
        soundButton.GetComponent<Image>().sprite = sprite;
    }

    public void MuteButton()
    {
        GameBehaviour.Instance.Audio.ToggleMute();
        audioSource.PlayOneShot(Resources.Load<AudioClip>("Sounds/Button"));
        SetMuteIcon();
    }

    #endregion
}
