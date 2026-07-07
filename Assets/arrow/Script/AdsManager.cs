using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;

    [Header("Test Ad Unit IDs")]
    [SerializeField] private string rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";
    [SerializeField] private string interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";

    [Header("Interstitial Frequency")]
    [SerializeField] private int minLevelsBetweenAds = 5;
    [SerializeField] private int maxLevelsBetweenAds = 10;

    private RewardedAd rewardedAd;
    private InterstitialAd interstitialAd;

    private Action pendingRewardAction;

    private const string LevelsSinceInterstitialKey = "LevelsSinceInterstitial";
    private const string NextInterstitialAfterKey = "NextInterstitialAfter";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!PlayerPrefs.HasKey(NextInterstitialAfterKey))
        {
            SetNextInterstitialTarget();
        }
    }

    private void Start()
    {
        MobileAds.Initialize(initStatus =>
        {
            LoadRewardedAd();
            LoadInterstitialAd();
        });
    }

    // ================= REWARDED =================

    public void LoadRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        AdRequest request = new AdRequest();

        RewardedAd.Load(rewardedAdUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Rewarded failed to load: " + error);
                return;
            }

            rewardedAd = ad;
            Debug.Log("Rewarded loaded.");

            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Rewarded ad closed. Loading next...");
                LoadRewardedAd();
            };

            rewardedAd.OnAdFullScreenContentFailed += (AdError adError) =>
            {
                Debug.LogError("Rewarded fullscreen failed: " + adError);
                LoadRewardedAd();
            };
        });
    }

    public void ShowRewardedAd()
    {
        ShowRewardedAd(() =>
        {
            if (WalletManager.Instance != null)
                WalletManager.Instance.AddCoins(10);
        });
    }

    public void ShowRewardedAdForEnergy()
    {
        ShowRewardedAd(() =>
        {
            if (EnergyManager.Instance != null)
                EnergyManager.Instance.AddEnergy(1);
        });
    }

    public void ShowRewardedAd(Action rewardAction)
    {
        pendingRewardAction = rewardAction;

        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show(reward =>
            {
                Debug.Log("Reward earned.");
                pendingRewardAction?.Invoke();
                pendingRewardAction = null;
            });
        }
        else
        {
            Debug.Log("Rewarded ad is not ready.");
            pendingRewardAction = null;
            LoadRewardedAd();
        }
    }

    // ================= INTERSTITIAL =================

    public void LoadInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        AdRequest request = new AdRequest();

        InterstitialAd.Load(interstitialAdUnitId, request, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Interstitial failed to load: " + error);
                return;
            }

            interstitialAd = ad;
            Debug.Log("Interstitial loaded.");

            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial closed. Loading next...");
                LoadInterstitialAd();
            };

            interstitialAd.OnAdFullScreenContentFailed += (AdError adError) =>
            {
                Debug.LogError("Interstitial fullscreen failed: " + adError);
                LoadInterstitialAd();
            };
        });
    }

    public void ShowInterstitialAd()
    {
        if (PurchaseState.RemoveAds)
        {
            Debug.Log("Interstitial skipped: Remove Ads purchased.");
            return;
        }

        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
        }
        else
        {
            Debug.Log("Interstitial ad is not ready.");
            LoadInterstitialAd();
        }
    }

    public void TryShowRandomInterstitialAfterLevel()
    {
        if (PurchaseState.RemoveAds)
        {
            Debug.Log("Random interstitial skipped: Remove Ads purchased.");
            return;
        }

        int levelsSinceAd = PlayerPrefs.GetInt(LevelsSinceInterstitialKey, 0);
        int nextAdAfter = PlayerPrefs.GetInt(NextInterstitialAfterKey, 7);

        levelsSinceAd++;

        Debug.Log("Interstitial counter: " + levelsSinceAd + "/" + nextAdAfter);

        if (levelsSinceAd >= nextAdAfter)
        {
            levelsSinceAd = 0;
            PlayerPrefs.SetInt(LevelsSinceInterstitialKey, levelsSinceAd);
            SetNextInterstitialTarget();

            ShowInterstitialAd();
        }
        else
        {
            PlayerPrefs.SetInt(LevelsSinceInterstitialKey, levelsSinceAd);
            PlayerPrefs.Save();
        }
    }

    private void SetNextInterstitialTarget()
    {
        int next = UnityEngine.Random.Range(minLevelsBetweenAds, maxLevelsBetweenAds + 1);

        PlayerPrefs.SetInt(NextInterstitialAfterKey, next);
        PlayerPrefs.Save();

        Debug.Log("Next interstitial after " + next + " completed levels.");
    }
}