using UnityEngine;

public static class PurchaseState
{
    private const string RemoveAdsKey = "RemoveAds";
    private const string DisableEnergyKey = "DisableEnergy";

    public static bool RemoveAds
    {
        get => PlayerPrefs.GetInt(RemoveAdsKey, 0) == 1;
        set
        {
            PlayerPrefs.SetInt(RemoveAdsKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool DisableEnergy
    {
        get => PlayerPrefs.GetInt(DisableEnergyKey, 0) == 1;
        set
        {
            PlayerPrefs.SetInt(DisableEnergyKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}