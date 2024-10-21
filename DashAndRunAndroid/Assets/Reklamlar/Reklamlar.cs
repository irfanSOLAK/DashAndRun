using UnityEngine;
using GoogleMobileAds.Api;
using System;


public class Reklamlar : MonoBehaviour,IManagerModule
{
    int interstitialShowNumber;
    public int ManagerExecutionOrder => 100;
    public void OnModuleAwake()
    {
        interstitialShowNumber = 1;
        adConfiguration = AdConfiguration.LoadInstance();
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
        });

        CreateAds();
    }

    public BannerAds bannerAds;
    public InterstitialAds interstitialAds;
    public RewardAds rewardAds;
    public static Action OnRewardReceived;

    private AdConfiguration adConfiguration;

    private void CreateAds()
    {
        if (!string.IsNullOrEmpty(adConfiguration.GetBannerAdId()))
        {
            bannerAds = new BannerAds(adConfiguration.GetBannerAdId());
        }
        if (!string.IsNullOrEmpty(adConfiguration.GetInterstitialAdId()))
        {
            interstitialAds = new InterstitialAds(adConfiguration.GetInterstitialAdId());
        }
        if (!string.IsNullOrEmpty(adConfiguration.GetRewardedAdId()))
        {
            rewardAds = new RewardAds(adConfiguration.GetRewardedAdId());
        }
    }

    public void ShowBanner(AdPosition adPosition)
    {
        if (bannerAds != null)
            bannerAds.ShowBanner(adPosition);
    }

    public void HideBanner()
    {
        if (bannerAds != null)
            bannerAds.HideBanner();
    }

     void ShowInterstitialAd()
    {
        if (interstitialAds != null)
            interstitialAds.ShowInterstitialAd();
    }
     void ShowInterstitialAdWithTime()
    {
        if (Time.realtimeSinceStartup > interstitialShowNumber * 80)
        {
            ShowInterstitialAd();
            interstitialShowNumber++;
        }
    }
    public void ShowInterAdOnEvenScene(int sceneNumber)
    {
        if (sceneNumber % 2 == 0)
        {
            GameBehaviour.Instance.Reklam.ShowInterstitialAdWithTime();
        }
    }
    public void ShowRewardedAd()
    {
        if (rewardAds != null)
            rewardAds.ShowRewardedAd();
    }
}

public class BannerAds
{
    private BannerView bannerView;
    public BannerView BannerView
    {
        get { return bannerView; }
    }

    private string bannerAdID;

    public BannerAds(string bannerAdID)
    {
        this.bannerAdID = bannerAdID;
        LoadBannerAd();
    }

    public void ShowBanner(AdPosition adPosition)
    {
        bannerView.SetPosition(adPosition);
        bannerView.Show();
    }

    public void HideBanner()
    {
        if (bannerView != null)
        {
            bannerView.Hide();
            DestroyBannerAd();
            LoadBannerAd();
        }
    }

    private void LoadBannerAd()
    {
        // create an instance of a banner view first.
        if (bannerView == null)
        {
            CreateBannerView();
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        bannerView.LoadAd(adRequest);
        bannerView.Hide();
    }

    private void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (bannerView != null)
        {
            DestroyBannerAd();
        }

        // Create a 320x50 banner at top of the screen
        bannerView = new BannerView(bannerAdID, AdSize.Banner, AdPosition.Top);
    }

    private void DestroyBannerAd()
    {
        if (bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            bannerView.Destroy();
            bannerView = null;
        }
    }
}

public class InterstitialAds
{
    private InterstitialAd interstitialAd;
    public InterstitialAd InterstitialAd
    {
        get { return interstitialAd; }
    }

    private string intersititialAdID;

    public InterstitialAds(string intersititialAdID)
    {
        this.intersititialAdID = intersititialAdID;
        LoadInterstitialAd();
    }

    public void ShowInterstitialAd()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            interstitialAd.Show();
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
        }
    }

    private void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(intersititialAdID, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                interstitialAd = ad;
                RegisterInterstitialReloadHandler(interstitialAd);
            });
    }

    private void RegisterInterstitialReloadHandler(InterstitialAd interstitialAd)
    {
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial Ad full screen content closed.");

            // Reload the ad so that we can show another as soon as possible.
            LoadInterstitialAd();
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            LoadInterstitialAd();
        };
    }

}

public class RewardAds
{
    private RewardedAd rewardedAd;
    public RewardedAd RewardedAd
    {
        get { return rewardedAd; }
    }

    private string rewardBasedAdID;

    public RewardAds(string rewardBasedAdID)
    {
        this.rewardBasedAdID = rewardBasedAdID;
        LoadRewardedAd();
    }

    public void ShowRewardedAd()
    {
        const string rewardMsg =
            "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                Reklamlar.OnRewardReceived();
                // TODO: Reward the user.
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }
    }

    private void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(rewardBasedAdID, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                rewardedAd = ad;
                RegisterRewardedReloadHandler(rewardedAd);
            });
    }

    private void RegisterRewardedReloadHandler(RewardedAd ad)
    {
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded Ad full screen content closed.");

            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedAd();
        };
    }
}