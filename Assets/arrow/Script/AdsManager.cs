using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;

    [Header("Test Ad Unit IDs")]
    [SerializeField] private string rewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";
    [SerializeField] private string interstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";

    private RewardedAd rewardedAd;
    private InterstitialAd interstitialAd;

    private Action pendingRewardAction;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        MobileAds.Initialize(initStatus =>
        {
            LoadRewardedAd();
            LoadInterstitialAd();
        });
    }

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
            AddCoins(10);
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

    private void AddCoins(int amount)
    {
        int coins = PlayerPrefs.GetInt("Coins", 0);
        coins += amount;

        PlayerPrefs.SetInt("Coins", coins);
        PlayerPrefs.Save();

        if (WalletManager.Instance != null)
        {
            WalletManager.Instance.UpdateCoinsText();
        }

        Debug.Log("Coins added: +" + amount);
    }

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
}