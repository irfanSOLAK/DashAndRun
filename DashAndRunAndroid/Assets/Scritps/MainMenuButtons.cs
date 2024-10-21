using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    [Header("==================== Menu Sound ====================")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject soundButton;
    [SerializeField] Sprite muteSprite;
    [SerializeField] Sprite unMuteSprite;

    [Header("==================== Play ====================")]
    [SerializeField] GameObject levelWindow;
    [SerializeField] private Transform levelButtonsParent; // Sahne yükleyecek butonlar

    [Header("==================== Share ====================")]
    [SerializeField] string subject;
    [TextArea] [SerializeField] string body;

    [Header("==================== Rate ====================")]
    [SerializeField] string marketLink;

    [Header("==================== Info ====================")]
    [SerializeField] GameObject infoButton;
    [SerializeField] GameObject infoWindow;

#if UNITY_IPHONE
	
	[DllImport("__Internal")]
	private static extern void sampleMethod (string iosPath, string message);
	
	[DllImport("__Internal")]
	private static extern void sampleTextMethod (string message);
	
#endif

    // Start is called before the first frame update
    void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        audioSource = GetAudioSource();
        SetMuteIcon();
        InitializeLevelButtons();
    }

    #region Reklamlar

    private void OnEnable()
    {
        GameBehaviour.Instance.Reklam.ShowBanner(GoogleMobileAds.Api.AdPosition.Bottom);
    }
    private void OnDisable()
    {
        GameBehaviour.Instance.Reklam.HideBanner();
    }

    #endregion

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


    #region Play

    public void Play()
    {
        GameBehaviour.Instance.Audio.PlaySound("Button");
        levelWindow.SetActive(true);
        soundButton.SetActive(false);
        infoButton.SetActive(false);
    }

    public void CloseLevelWindow(GameObject closeButton)
    {
        GameBehaviour.Instance.Audio.PlaySound("Button");

        Transform root = closeButton.transform.root;

        Transform windowToClose = null;

        foreach (Transform child in root)
        {
            if (closeButton.transform.IsChildOf(child)) // parentxxx bu child'ýn alt seviyesindeyse
            {
                windowToClose = child;
                break;
            }
        }

        windowToClose.gameObject.SetActive(false);
        soundButton.SetActive(true);
        infoButton.SetActive(true);
    }

    private void InitializeLevelButtons()
    {
        Button[] levelButtons = levelButtonsParent.GetComponentsInChildren<Button>();

        foreach (Button button in levelButtons)
        {
            // Her butona LoadLevel metodunu dinleyici olarak ekle
            button.onClick.AddListener(() => StartCoroutine(LoadLevel(button)));
        }
    }

    private IEnumerator LoadLevel(Button button)
    {
        string sceneName = button.name; // Butonun ismini al
        yield return new WaitForSeconds(GameBehaviour.Instance.Audio.PlaySound("Button"));
        int sceneIndex = SceneManager.GetSceneByName(sceneName).buildIndex;
        GameBehaviour.Instance.Reklam.ShowInterAdOnEvenScene(sceneIndex);
        SceneManager.LoadScene(sceneName);
    }

    #endregion

    #region Info

    public void Info()
    {
        GameBehaviour.Instance.Audio.PlaySound("Button");
        infoWindow.SetActive(true);
        soundButton.SetActive(false);
        infoButton.SetActive(false);
    }

    #endregion
    public void Quit()
    {
        Application.Quit();
    }


    #region Share & Rate

    public void Share()
    {
        StartCoroutine(ShareAndroidText());
    }
    IEnumerator ShareAndroidText()
    {
        yield return new WaitForEndOfFrame();
        //execute the below lines if being run on a Android device
#if UNITY_ANDROID
        //Reference of AndroidJavaClass class for intent
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        //Reference of AndroidJavaObject class for intent
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
        //call setAction method of the Intent object created
        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        //set the type of sharing that is happening
        intentObject.Call<AndroidJavaObject>("setType", "text/plain");
        //add data to be passed to the other activity i.e., the data to be sent
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), subject);
        //intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TITLE"), "Text Sharing ");
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), body);
        //get the current activity
        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        //start the activity by sending the intent data
        AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share Via");
        currentActivity.Call("startActivity", jChooser);
#endif
    }

    public void OniOSTextSharingClick()
    {

#if UNITY_IPHONE || UNITY_IPAD
		string shareMessage = "Wow I Just Share Text ";
		sampleTextMethod (shareMessage);
#endif
    }

    public void Rate()
    {
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=com." + marketLink);
#elif UNITY_IPHONE
		Application.OpenURL("itms-apps://itunes.apple.com/app/idYOUR_ID");
#endif
    }

    #endregion
}
