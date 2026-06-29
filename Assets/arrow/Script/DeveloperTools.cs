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

        PlayerPrefs.DeleteKey("SelectedLocation");

        PlayerPrefs.DeleteKey("Bought_CityNight");
        PlayerPrefs.DeleteKey("Bought_CityWinter");

        PlayerPrefs.Save();

        if (WalletManager.Instance != null)
            WalletManager.Instance.UpdateCoinsText();

        LocationShopManager shop = FindFirstObjectByType<LocationShopManager>();
        if (shop != null)
            shop.RefreshButtons();

        Debug.Log("Coins, Stars and Locations Reset");
    }
}