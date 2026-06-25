using UnityEngine;

public class DeveloperTools : MonoBehaviour
{
    public void ResetCoinsAndStars()
    {
        PlayerPrefs.DeleteKey("Coins");

        for (int i = 1; i <= 100; i++)
        {
            PlayerPrefs.DeleteKey("Level_" + i + "_Stars");
        }

        PlayerPrefs.Save();

        if (WalletManager.Instance != null)
            WalletManager.Instance.UpdateCoinsText();

        Debug.Log("Coins and Stars Reset");
    }
}