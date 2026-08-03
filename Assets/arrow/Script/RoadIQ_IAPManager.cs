using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;

public class RoadIQ_IAPManager : MonoBehaviour
{
    public static RoadIQ_IAPManager Instance;

    public const string Coins100 = "coins_100";
    public const string Coins300 = "coins_300";
    public const string RemoveAds = "remove_ads";
    public const string DisableEnergy = "disable_energy";

    private StoreController storeController;
    private bool isReady;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // ТИМЧАСОВО: не дозволяємо старій тестовій покупці
        // автоматично ввімкнути нескінченну енергію.
        PurchaseState.DisableEnergy = false;

        InitializeIAP();
    }

    private async void InitializeIAP()
    {
        Debug.Log("IAP: Initializing...");

        storeController = UnityIAPServices.StoreController();

        storeController.OnPurchasePending += OnPurchasePending;
        storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;

        storeController.OnPurchasesFetched += OnPurchasesFetched;
        storeController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;

        await storeController.Connect();

        Debug.Log("IAP: Connected to store.");

        FetchProducts();

        storeController.FetchPurchases();
    }

    private void FetchProducts()
    {
        var products = new List<ProductDefinition>
        {
            new ProductDefinition(Coins100, ProductType.Consumable),
            new ProductDefinition(Coins300, ProductType.Consumable),
            new ProductDefinition(RemoveAds, ProductType.NonConsumable),
            new ProductDefinition(DisableEnergy, ProductType.NonConsumable)
        };

        storeController.FetchProducts(products);
        isReady = true;

        Debug.Log("IAP: FetchProducts called.");
    }

    public void BuyCoins100()
    {
        BuyProduct(Coins100);
    }

    public void BuyCoins300()
    {
        BuyProduct(Coins300);
    }

    public void BuyRemoveAds()
    {
        BuyProduct(RemoveAds);
    }

    public void BuyDisableEnergy()
    {
        BuyProduct(DisableEnergy);
    }

    private void BuyProduct(string productId)
    {
        if (!isReady || storeController == null)
        {
            Debug.LogWarning("IAP: Store is not ready yet.");
            return;
        }

        Product product = storeController.GetProducts()
            .FirstOrDefault(p => p.definition.id == productId);

        if (product == null)
        {
            Debug.LogWarning("IAP: Product not found: " + productId);
            return;
        }

        Debug.Log("IAP: Buying " + productId);
        storeController.PurchaseProduct(product);
    }

    private void OnPurchasePending(PendingOrder order)
    {
        string productId =
            order.CartOrdered.Items().First().Product.definition.id;

        Debug.Log("IAP: Purchase pending: " + productId);

        GiveReward(productId);

        storeController.ConfirmPurchase(order);
    }

    private void OnPurchaseConfirmed(Order order)
    {
        switch (order)
        {
            case FailedOrder failedOrder:
                Debug.LogError(
                    "IAP: Purchase confirmation failed: " +
                    failedOrder.FailureReason
                );
                break;

            case ConfirmedOrder confirmedOrder:
                string productId =
                    confirmedOrder.CartOrdered.Items()
                        .First().Product.definition.id;

                Debug.Log(
                    "IAP: Purchase confirmed: " + productId
                );
                break;
        }
    }

    private void OnPurchasesFetched(Orders orders)
    {
        Debug.Log("IAP: Purchases fetched.");

        foreach (Order order in orders.ConfirmedOrders)
        {
            string productId =
                order.CartOrdered.Items().First().Product.definition.id;

            if (productId == RemoveAds)
            {
                PurchaseState.RemoveAds = true;
                Debug.Log("IAP: Restore Remove Ads.");
            }

            if (productId == DisableEnergy)
            {
                // ТИМЧАСОВО НЕ ВІДНОВЛЮЄМО НЕСКІНЧЕННУ ЕНЕРГІЮ.
                // Це перевірка, чи стара тестова покупка
                // ховає інтерфейс енергії через секунду.

                PurchaseState.DisableEnergy = false;

                if (EnergyManager.Instance != null)
                {
                    EnergyManager.Instance.CloseNoEnergyPanel();
                    EnergyManager.Instance.RefreshUI();
                }

                Debug.LogWarning(
                    "IAP: Disable Energy purchase found, " +
                    "but restore is temporarily disabled."
                );
            }
        }
    }

    private void OnPurchasesFetchFailed(
        PurchasesFetchFailureDescription failure)
    {
        Debug.LogWarning(
            "IAP: Purchases fetch failed: " + failure.message
        );
    }

    private void GiveReward(string productId)
    {
        switch (productId)
        {
            case Coins100:
                if (WalletManager.Instance != null)
                    WalletManager.Instance.AddCoins(100);
                break;

            case Coins300:
                if (WalletManager.Instance != null)
                    WalletManager.Instance.AddCoins(300);
                break;

            case RemoveAds:
                PurchaseState.RemoveAds = true;
                Debug.Log("IAP: Remove Ads enabled.");
                break;

            case DisableEnergy:
                // ТИМЧАСОВО НЕ ВМИКАЄМО НЕСКІНЧЕННУ ЕНЕРГІЮ.
                PurchaseState.DisableEnergy = false;

                if (EnergyManager.Instance != null)
                {
                    EnergyManager.Instance.CloseNoEnergyPanel();
                    EnergyManager.Instance.RefreshUI();
                }

                Debug.LogWarning(
                    "IAP: Disable Energy is temporarily disabled."
                );
                break;
        }

        Debug.Log("IAP: Reward given for " + productId);
    }

    public void RestorePurchases()
    {
        if (storeController == null)
        {
            Debug.LogWarning("IAP: StoreController is null.");
            return;
        }

        storeController.FetchPurchases();
        storeController.RestoreTransactions(OnTransactionsRestored);
    }

    private void OnTransactionsRestored(
        bool success,
        string error)
    {
        Debug.Log(
            "IAP: Restore transactions success: " +
            success +
            " Error: " +
            error
        );
    }
}