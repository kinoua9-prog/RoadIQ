using TMPro;
using UnityEngine;

public class WalletManager : MonoBehaviour
{
    public static WalletManager Instance;

    [Header("UI")]
    public TMP_Text coinsText;

    private const string CoinsKey = "Coins";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateCoinsText();
    }

    public int GetCoins()
    {
        return PlayerPrefs.GetInt(CoinsKey, 0);
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0)
            return;

        int coins = GetCoins();
        coins += amount;

        PlayerPrefs.SetInt(CoinsKey, coins);
        PlayerPrefs.Save();

        UpdateCoinsText();
    }

    public bool SpendCoins(int amount)
    {
        int coins = GetCoins();

        if (coins < amount)
            return false;

        coins -= amount;

        PlayerPrefs.SetInt(CoinsKey, coins);
        PlayerPrefs.Save();

        UpdateCoinsText();

        return true;
    }

    public void UpdateCoinsText()
    {
        if (coinsText != null)
            coinsText.text = GetCoins().ToString();
    }
}