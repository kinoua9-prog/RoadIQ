using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;

    [Header("Production Ad Unit IDs")]
    [SerializeField]
    private string rewardedAdUnitId =
        "ca-app-pub-5183235188513724/8666522729";

    [SerializeField]
    private string interstitialAdUnitId =
        "ca-app-pub-5183235188513724/2140642821";

    [Header("Interstitial Frequency")]
    [SerializeField] private int minLevelsBetweenAds = 5;
    [SerializeField] private int maxLevelsBetweenAds = 10;

    private RewardedAd rewardedAd;
    private InterstitialAd interstitialAd;

    private bool rewardedAdIsShowing;
    private bool interstitialAdIsShowing;

    private const string LevelsSinceInterstitialKey =
        "LevelsSinceInterstitial";

    private const string NextInterstitialAfterKey =
        "NextInterstitialAfter";

    private void Awake()
    {
        if (Instance != null && Instance != this)
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
        Debug.Log("ADS: Initializing Mobile Ads...");

        // Події реклами виконуватимуться в головному потоці Unity.
        // Це важливо для WalletManager, EnergyManager та UI.
        MobileAds.RaiseAdEventsOnUnityMainThread = true;

        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("ADS: Mobile Ads initialized.");

            LoadRewardedAd();
            LoadInterstitialAd();
        });
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }
    }

    // =========================================================
    // REWARDED AD
    // =========================================================

    public void LoadRewardedAd()
    {
        if (rewardedAdIsShowing)
        {
            return;
        }

        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        Debug.Log("ADS: Loading rewarded ad...");

        AdRequest request = new AdRequest();

        RewardedAd.Load(
            rewardedAdUnitId,
            request,
            (RewardedAd loadedAd, LoadAdError error) =>
            {
                if (error != null || loadedAd == null)
                {
                    Debug.LogError(
                        "ADS: Rewarded failed to load: " + error
                    );

                    rewardedAd = null;
                    return;
                }

                rewardedAd = loadedAd;

                Debug.Log("ADS: Rewarded loaded.");

                loadedAd.OnAdFullScreenContentOpened += () =>
                {
                    rewardedAdIsShowing = true;

                    Debug.Log(
                        "ADS: Rewarded fullscreen opened."
                    );
                };

                loadedAd.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log(
                        "ADS: Rewarded closed. Loading next..."
                    );

                    rewardedAdIsShowing = false;

                    loadedAd.Destroy();

                    if (rewardedAd == loadedAd)
                    {
                        rewardedAd = null;
                    }

                    LoadRewardedAd();
                };

                loadedAd.OnAdFullScreenContentFailed +=
                    (AdError adError) =>
                    {
                        Debug.LogError(
                            "ADS: Rewarded fullscreen failed: " +
                            adError
                        );

                        rewardedAdIsShowing = false;

                        loadedAd.Destroy();

                        if (rewardedAd == loadedAd)
                        {
                            rewardedAd = null;
                        }

                        LoadRewardedAd();
                    };
            }
        );
    }

    // Нагорода: 10 монет
    public void ShowRewardedAd()
    {
        ShowRewardedAdWithAction(() =>
        {
            if (WalletManager.Instance == null)
            {
                Debug.LogError(
                    "ADS REWARD FAILED: WalletManager.Instance is null."
                );

                return;
            }

            WalletManager.Instance.AddCoins(10);

            Debug.Log(
                "ADS REWARD SUCCESS: Added 10 coins."
            );
        });
    }

    // Нагорода: 1 енергія
    public void ShowRewardedAdForEnergy()
    {
        ShowRewardedAdWithAction(() =>
        {
            if (EnergyManager.Instance == null)
            {
                Debug.LogError(
                    "ADS REWARD FAILED: EnergyManager.Instance is null."
                );

                return;
            }

            EnergyManager.Instance.AddEnergy(1);
            EnergyManager.Instance.RefreshUI();
            EnergyManager.Instance.CloseNoEnergyPanel();

            Debug.Log(
                "ADS REWARD SUCCESS: Added 1 energy."
            );
        });
    }

    private void ShowRewardedAdWithAction(Action rewardAction)
    {
        if (rewardedAdIsShowing)
        {
            Debug.LogWarning(
                "ADS: Rewarded ad is already showing."
            );

            return;
        }

        if (rewardAction == null)
        {
            Debug.LogError(
                "ADS: Reward action is null."
            );

            return;
        }

        if (rewardedAd == null || !rewardedAd.CanShowAd())
        {
            Debug.LogWarning(
                "ADS: Rewarded ad is not ready."
            );

            rewardedAdIsShowing = false;
            LoadRewardedAd();
            return;
        }

        RewardedAd adToShow = rewardedAd;

        rewardedAd = null;
        rewardedAdIsShowing = true;

        bool rewardWasGranted = false;

        Debug.Log("ADS: Showing rewarded ad.");

        adToShow.Show(reward =>
        {
            if (rewardWasGranted)
            {
                Debug.LogWarning(
                    "ADS: Reward callback was already executed."
                );

                return;
            }

            rewardWasGranted = true;

            Debug.Log(
                "ADS: Reward earned. Type: " +
                reward.Type +
                ", Amount: " +
                reward.Amount
            );

            try
            {
                rewardAction.Invoke();

                Debug.Log(
                    "ADS: Reward action completed successfully."
                );
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    "ADS: Reward action exception: " +
                    exception
                );
            }
        });
    }

    // =========================================================
    // INTERSTITIAL AD
    // =========================================================

    public void LoadInterstitialAd()
    {
        if (interstitialAdIsShowing)
        {
            return;
        }

        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        Debug.Log("ADS: Loading interstitial ad...");

        AdRequest request = new AdRequest();

        InterstitialAd.Load(
            interstitialAdUnitId,
            request,
            (InterstitialAd loadedAd, LoadAdError error) =>
            {
                if (error != null || loadedAd == null)
                {
                    Debug.LogError(
                        "ADS: Interstitial failed to load: " +
                        error
                    );

                    interstitialAd = null;
                    return;
                }

                interstitialAd = loadedAd;

                Debug.Log("ADS: Interstitial loaded.");

                loadedAd.OnAdFullScreenContentOpened += () =>
                {
                    interstitialAdIsShowing = true;

                    Debug.Log(
                        "ADS: Interstitial fullscreen opened."
                    );
                };

                loadedAd.OnAdFullScreenContentClosed += () =>
                {
                    Debug.Log(
                        "ADS: Interstitial closed. Loading next..."
                    );

                    interstitialAdIsShowing = false;

                    loadedAd.Destroy();

                    if (interstitialAd == loadedAd)
                    {
                        interstitialAd = null;
                    }

                    LoadInterstitialAd();
                };

                loadedAd.OnAdFullScreenContentFailed +=
                    (AdError adError) =>
                    {
                        Debug.LogError(
                            "ADS: Interstitial fullscreen failed: " +
                            adError
                        );

                        interstitialAdIsShowing = false;

                        loadedAd.Destroy();

                        if (interstitialAd == loadedAd)
                        {
                            interstitialAd = null;
                        }

                        LoadInterstitialAd();
                    };
            }
        );
    }

    public void ShowInterstitialAd()
    {
        ShowInterstitialAd(null);
    }

    public void ShowInterstitialAd(Action onFinished)
    {
        if (PurchaseState.RemoveAds)
        {
            Debug.Log(
                "ADS: Interstitial skipped because Remove Ads is purchased."
            );

            onFinished?.Invoke();
            return;
        }

        if (interstitialAdIsShowing)
        {
            Debug.LogWarning(
                "ADS: Interstitial is already showing."
            );

            onFinished?.Invoke();
            return;
        }

        if (interstitialAd == null ||
            !interstitialAd.CanShowAd())
        {
            Debug.LogWarning(
                "ADS: Interstitial is not ready. Continuing without ad."
            );

            LoadInterstitialAd();
            onFinished?.Invoke();
            return;
        }

        InterstitialAd adToShow = interstitialAd;

        interstitialAd = null;
        interstitialAdIsShowing = true;

        bool continuationCalled = false;

        void ContinueOnce()
        {
            if (continuationCalled)
                return;

            continuationCalled = true;
            onFinished?.Invoke();
        }

        adToShow.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log(
                "ADS: Interstitial closed."
            );

            interstitialAdIsShowing = false;

            adToShow.Destroy();
            LoadInterstitialAd();

            ContinueOnce();
        };

        adToShow.OnAdFullScreenContentFailed +=
            (AdError adError) =>
            {
                Debug.LogError(
                    "ADS: Interstitial failed to show: " +
                    adError
                );

                interstitialAdIsShowing = false;

                adToShow.Destroy();
                LoadInterstitialAd();

                ContinueOnce();
            };

        Debug.Log("ADS: Showing interstitial.");

        adToShow.Show();
    }

    public void TryShowRandomInterstitialAfterAction(Action continueAction)
    {
        if (PurchaseState.RemoveAds)
        {
            Debug.Log(
                "ADS: Interstitial skipped because Remove Ads is purchased."
            );

            continueAction?.Invoke();
            return;
        }

        int levelsSinceAd = PlayerPrefs.GetInt(
            LevelsSinceInterstitialKey,
            0
        );

        int nextAdAfter = PlayerPrefs.GetInt(
            NextInterstitialAfterKey,
            7
        );

        levelsSinceAd++;

        Debug.Log(
            "ADS: Interstitial counter: " +
            levelsSinceAd +
            "/" +
            nextAdAfter
        );

        if (levelsSinceAd < nextAdAfter)
        {
            PlayerPrefs.SetInt(
                LevelsSinceInterstitialKey,
                levelsSinceAd
            );

            PlayerPrefs.Save();

            continueAction?.Invoke();
            return;
        }

        PlayerPrefs.SetInt(
            LevelsSinceInterstitialKey,
            0
        );

        SetNextInterstitialTarget();
        PlayerPrefs.Save();

        ShowInterstitialAd(continueAction);
    }

    private void SetNextInterstitialTarget()
    {
        int minimum = Mathf.Max(1, minLevelsBetweenAds);
        int maximum = Mathf.Max(minimum, maxLevelsBetweenAds);

        int next = UnityEngine.Random.Range(
            minimum,
            maximum + 1
        );

        PlayerPrefs.SetInt(
            NextInterstitialAfterKey,
            next
        );

        PlayerPrefs.Save();

        Debug.Log(
            "ADS: Next interstitial after " +
            next +
            " completed levels."
        );
    }
}