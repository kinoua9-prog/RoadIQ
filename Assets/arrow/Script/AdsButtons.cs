using UnityEngine;

public class AdsButtons : MonoBehaviour
{
    public void ShowCoinsRewardedAd()
    {
        if (AdsManager.Instance != null)
        {
            AdsManager.Instance.ShowRewardedAd();
        }
        else
        {
            Debug.LogError("AdsManager.Instance не знайдений!");
        }
    }

    public void ShowEnergyRewardedAd()
    {
        if (AdsManager.Instance != null)
        {
            AdsManager.Instance.ShowRewardedAdForEnergy();
        }
        else
        {
            Debug.LogError("AdsManager.Instance не знайдений!");
        }
    }

    public void ShowInterstitialAd()
    {
        if (AdsManager.Instance != null)
        {
            AdsManager.Instance.ShowInterstitialAd();
        }
        else
        {
            Debug.LogError("AdsManager.Instance не знайдений!");
        }
    }
}