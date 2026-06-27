using TMPro;
using UnityEngine;

public class WalletManager : MonoBehaviour
{
    public static WalletManager Instance;

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

        PlayerPrefs.SetInt(CoinsKey, GetCoins() + amount);
        PlayerPrefs.Save();

        UpdateCoinsText();
    }

    public bool SpendCoins(int amount)
    {
        if (GetCoins() < amount)
            return false;

        PlayerPrefs.SetInt(CoinsKey, GetCoins() - amount);
        PlayerPrefs.Save();

        UpdateCoinsText();

        return true;
    }

    public void UpdateCoinsText()
    {
        CoinsText[] texts = FindObjectsByType<CoinsText>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (CoinsText text in texts)
        {
            if (text != null)
                text.UpdateText();
        }
    }
}