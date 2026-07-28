using UnityEngine;

public class DeveloperTools : MonoBehaviour
{
    public void ResetCoinsAndStars()
    {
        PlayerPrefs.DeleteKey("Coins");

        for (int i = 1; i <= 100; i++)
        {
            PlayerPrefs.DeleteKey("Level_" + i + "_Stars");
            PlayerPrefs.DeleteKey("LevelStars_" + i);
        }

        PlayerPrefs.DeleteKey("SelectedCityLocation");
        PlayerPrefs.DeleteKey("SelectedMallLocation");

        PlayerPrefs.DeleteKey("Bought_CityNight");
        PlayerPrefs.DeleteKey("Bought_CityWinter");
        PlayerPrefs.DeleteKey("Bought_MallNight");
        PlayerPrefs.DeleteKey("Bought_MallWinter");

        PlayerPrefs.Save();

        if (WalletManager.Instance != null)
            WalletManager.Instance.UpdateCoinsText();

        LocationShopManager shop =
            FindFirstObjectByType<LocationShopManager>();

        if (shop != null)
            shop.RefreshButtons();

        Debug.Log("Coins, Stars and Locations Reset");
    }

    public void ResetDisableEnergy()
    {
        PurchaseState.DisableEnergy = false;

        if (EnergyManager.Instance != null)
            EnergyManager.Instance.RefreshUI();

        Debug.Log("DisableEnergy reset to FALSE");
    }

    public void ResetRemoveAds()
    {
        PurchaseState.RemoveAds = false;

        Debug.Log("RemoveAds reset to FALSE");
    }

    public void ResetAllPurchasesForTesting()
    {
        PurchaseState.DisableEnergy = false;
        PurchaseState.RemoveAds = false;

        if (EnergyManager.Instance != null)
            EnergyManager.Instance.RefreshUI();

        Debug.Log("All local purchase states reset for testing");
    }
}