using UnityEditor;
using System.IO;
using UnityEngine;

[InitializeOnLoad]
[CustomEditor(typeof(AdConfiguration))]
public class AdConfigurationEditor : Editor
{
    private const string AdConfigurationResDir = "Assets/Reklamlar/Resources";
    private const string AdConfigurationFile = "AdConfiguration";
    private const string AdConfigurationFileExtension = ".asset";

    [MenuItem("Assets/Reklamlar/Settings...")]
    public static void OpenInspector()
    {
        var instance = GetInstance();
        if (instance != null)
        {
            Selection.activeObject = instance;
            EditorUtility.FocusProjectWindow();
        }
    }

    static AdConfigurationEditor()
    {
        // EditorApplication.update ile Asset'in varlýðýný kontrol et ve gerekirse oluþtur
        EditorApplication.update += CheckAndCreateAsset;
    }

    private static void CheckAndCreateAsset()
    {
        // EditorApplication.update'ý bir kere çalýþtýr ve ardýndan kaldýr
        EditorApplication.update -= CheckAndCreateAsset;

        if (AdConfiguration.LoadInstance() == null)
        {
            GetInstance();
        }
    }

    public static AdConfiguration GetInstance()
    {
        var instance = AdConfiguration.LoadInstance();
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

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Update Settings"))
        {
            UpdateSettings((AdConfiguration)target);
        }
    }

    public static bool UpdateSettings(AdConfiguration adConfig)
    {
        UpdateGoogleMobileAdsSettings(adConfig);
        CheckAdConfiguration(adConfig);
        return true;
    }

    private static void UpdateGoogleMobileAdsSettings(AdConfiguration adConfig)
    {
        string settingsPath = "Assets/GoogleMobileAds/Resources/GoogleMobileAdsSettings.asset";

        var settings = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(settingsPath);
        if (settings == null)
        {
            Debug.LogError("GoogleMobileAdsSettings.asset not found at path: " + settingsPath);
            return;
        }

        SerializedObject serializedObject = new SerializedObject(settings);
        SerializedProperty androidAppIdProperty = serializedObject.FindProperty("adMobAndroidAppId");

        if (androidAppIdProperty == null)
        {
            Debug.LogError("Unable to find property 'adMobAndroidAppId' in GoogleMobileAdsSettings.");
            return;
        }

        string newAppId = adConfig.GetAndroidAppId();
        androidAppIdProperty.stringValue = newAppId;

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Updated GoogleMobileAdsSettings with App ID: " + newAppId);
    }

    private static void CheckAdConfiguration(AdConfiguration adConfig)
    {
        if (adConfig.useTestIds) return;

        ValidateAdId(adConfig.GetAndroidAppId(), "App ID");
        ValidateAdId(adConfig.GetBannerAdId(), "Banner Ad ID");
        ValidateAdId(adConfig.GetInterstitialAdId(), "Interstitial Ad ID");
        ValidateAdId(adConfig.GetRewardedAdId(), "Rewarded Ad ID");
    }

    private static void ValidateAdId(string adId, string adType)
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