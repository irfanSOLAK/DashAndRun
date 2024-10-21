using UnityEngine;


public class AdConfiguration : ScriptableObject
{
    private const string AdConfigurationFile = "AdConfiguration";

    public static AdConfiguration LoadInstance()
    {
        //Read from resources.
        var instance = Resources.Load<AdConfiguration>(AdConfigurationFile);
        return instance;
    }


    public bool useTestIds = true;

    [Header("App ID")]
    [SerializeField] private string androidAppIdTest = "ca-app-pub-3940256099942544~3347511713";
    [SerializeField] private string androidAppIdReal;

    [Header("Banner Ads")]
    [SerializeField] private string bannerAdIdTest = "ca-app-pub-3940256099942544/6300978111";
    [SerializeField] private string bannerAdIdReal;

    [Header("Interstitial Ads")]
    [SerializeField] private string interstitialAdIdTest = "ca-app-pub-3940256099942544/1033173712";
    [SerializeField] private string interstitialAdIdReal;

    [Header("Rewarded Ads")]
    [SerializeField] private string rewardedAdIdTest = "ca-app-pub-3940256099942544/5224354917";
    [SerializeField] private string rewardedAdIdReal;

    public string GetAndroidAppId() => useTestIds ? androidAppIdTest : androidAppIdReal;
    public string GetBannerAdId() => useTestIds ? bannerAdIdTest : bannerAdIdReal;
    public string GetInterstitialAdId() => useTestIds ? interstitialAdIdTest : interstitialAdIdReal;
    public string GetRewardedAdId() => useTestIds ? rewardedAdIdTest : rewardedAdIdReal;
}

#region eski

/*

Google asset için önceden böyleydi ama editorde OnValidate falan kendi için çaðýrýlýyor bu asset için çaðýrýlmýyor

public class AdConfiguration : ScriptableObject
{
    private const string AdConfigurationResDir = "Assets/Reklamlar/Resources";

    private const string AdConfigurationFile = "AdConfiguration";

    private const string AdConfigurationFileExtension = ".asset";

    public static AdConfiguration LoadInstance()
    {
        //Read from resources.
        var instance = Resources.Load<AdConfiguration>(AdConfigurationFile);

        //Create instance if null.
        if (instance == null)
        {
            Directory.CreateDirectory(AdConfigurationResDir);
            instance = ScriptableObject.CreateInstance<AdConfiguration>();
            string assetPath = Path.Combine(
                AdConfigurationResDir,
                AdConfigurationFile + AdConfigurationFileExtension);
            AssetDatabase.CreateAsset(instance, assetPath);
            AssetDatabase.SaveAssets();
        }

        return instance;
    }


    private void OnEnable() => UpdateSettings();
    private void OnValidate() => UpdateSettings();
    private void Reset() => UpdateSettings();

    private void UpdateSettings()
    {
        UpdateGoogleMobileAdsSettings();
        CheckAdConfiguration();
    }

    public void UpdateGoogleMobileAdsSettings()
    {
        // GoogleMobileAdsSettings.asset dosyasýnýn yolu
        string settingsPath = "Assets/GoogleMobileAds/Resources/GoogleMobileAdsSettings.asset";

        // Dosyayý yükle
        var settings = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(settingsPath);
        if (settings == null)
        {
            Debug.LogError("GoogleMobileAdsSettings.asset not found at path: " + settingsPath);
            return;
        }

        // ScriptableObject'i serialize et
        SerializedObject serializedObject = new SerializedObject(settings);
        SerializedProperty androidAppIdProperty = serializedObject.FindProperty("adMobAndroidAppId");

        if (androidAppIdProperty == null)
        {
            Debug.LogError("Unable to find property 'adMobAndroidAppId' in GoogleMobileAdsSettings.");
            return;
        }

        // AdConfiguration'dan App ID'yi al
        string newAppId = GetAndroidAppId();
        androidAppIdProperty.stringValue = newAppId;

        // Deðiþiklikleri kaydet
        serializedObject.ApplyModifiedProperties();

        // Deðiþikliklerin editor penceresinde görünmesini saðlamak için
        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Updated GoogleMobileAdsSettings with App ID: " + newAppId);
    }

    private void CheckAdConfiguration()
    {
        if (useTestIds) return;

        ValidateAdId(GetAndroidAppId(), "App ID");
        ValidateAdId(GetBannerAdId(), "Banner Ad ID");
        ValidateAdId(GetInterstitialAdId(), "Interstitial Ad ID");
        ValidateAdId(GetRewardedAdId(), "Rewarded Ad ID");
    }

    private void ValidateAdId(string adId, string adType)
    {
        if (string.IsNullOrEmpty(adId))
        {
            if (adType == "App ID")
            {
                Debug.LogError($"Warning: {adType} is missing. Please ensure all required fields are filled.");
            }
            else
            {
                Debug.LogWarning($"Warning: {adType} is missing. Please ensure all required fields are filled.");
            }
        }
    }
}

*/

#endregion