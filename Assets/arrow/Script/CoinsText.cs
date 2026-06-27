using TMPro;
using UnityEngine;

public class CoinsText : MonoBehaviour
{
    private TMP_Text coinText;

    private void Awake()
    {
        coinText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        if (coinText == null)
            coinText = GetComponent<TMP_Text>();

        if (WalletManager.Instance != null)
            coinText.text = WalletManager.Instance.GetCoins().ToString();
    }
}