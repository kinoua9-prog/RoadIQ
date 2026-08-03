using UnityEngine;

public class AdsButtons : MonoBehaviour
{
    public void ShowCoinsRewardedAd()
    {
        if (AdsManager.Instance == null)
        {
            Debug.LogError(
                "ADS BUTTON: AdsManager.Instance не знайдений!"
            );

            return;
        }

        Debug.Log(
            "ADS BUTTON: Coins rewarded button pressed."
        );

        AdsManager.Instance.ShowRewardedAd();
    }

    public void ShowEnergyRewardedAd()
    {
        if (AdsManager.Instance == null)
        {
            Debug.LogError(
                "ADS BUTTON: AdsManager.Instance не знайдений!"
            );

            return;
        }

        Debug.Log(
            "ADS BUTTON: Energy rewarded button pressed."
        );

        AdsManager.Instance.ShowRewardedAdForEnergy();
    }

    public void ShowInterstitialAd()
    {
        if (AdsManager.Instance == null)
        {
            Debug.LogError(
                "ADS BUTTON: AdsManager.Instance не знайдений!"
            );

            return;
        }

        Debug.Log(
            "ADS BUTTON: Interstitial button pressed."
        );

        AdsManager.Instance.ShowInterstitialAd();
    }
}