using UnityEngine;

public class ShopButtons : MonoBehaviour
{
    public void Buy100Coins()
    {
        if (RoadIQ_IAPManager.Instance != null)
        {
            RoadIQ_IAPManager.Instance.BuyCoins100();
        }
        else
        {
            Debug.LogError("RoadIQ_IAPManager.Instance не знайдений!");
        }
    }

    public void Buy300Coins()
    {
        if (RoadIQ_IAPManager.Instance != null)
        {
            RoadIQ_IAPManager.Instance.BuyCoins300();
        }
        else
        {
            Debug.LogError("RoadIQ_IAPManager.Instance не знайдений!");
        }
    }

    public void BuyRemoveAds()
    {
        if (RoadIQ_IAPManager.Instance != null)
        {
            RoadIQ_IAPManager.Instance.BuyRemoveAds();
        }
        else
        {
            Debug.LogError("RoadIQ_IAPManager.Instance не знайдений!");
        }
    }

    public void BuyDisableEnergy()
    {
        if (RoadIQ_IAPManager.Instance != null)
        {
            RoadIQ_IAPManager.Instance.BuyDisableEnergy();
        }
        else
        {
            Debug.LogError("RoadIQ_IAPManager.Instance не знайдений!");
        }
    }
}