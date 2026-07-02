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

        // Вибрані локації
        PlayerPrefs.DeleteKey("SelectedCityLocation");
        PlayerPrefs.DeleteKey("SelectedMallLocation");

        // Куплені City
        PlayerPrefs.DeleteKey("Bought_CityNight");
        PlayerPrefs.DeleteKey("Bought_CityWinter");

        // Куплені Mall
        PlayerPrefs.DeleteKey("Bought_MallNight");
        PlayerPrefs.DeleteKey("Bought_MallWinter");

        PlayerPrefs.Save();

        if (WalletManager.Instance != null)
            WalletManager.Instance.UpdateCoinsText();

        LocationShopManager shop = FindFirstObjectByType<LocationShopManager>();
        if (shop != null)
            shop.RefreshButtons();

        Debug.Log("Coins, Stars and Locations Reset");
    }
}