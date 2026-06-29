using UnityEngine;

public class LocationShopManager : MonoBehaviour
{
    [System.Serializable]
    public class LocationItem
    {
        public string locationId;

        public GameObject priceButton;
        public GameObject equipButton;
        public GameObject equippedButton;

        public int price;
        public bool isDefault;
    }

    public LocationItem[] locations;

    private const string SelectedLocationKey = "SelectedLocation";
    private const string BoughtKey = "Bought_";

    private void Start()
    {
        if (!PlayerPrefs.HasKey(SelectedLocationKey))
        {
            PlayerPrefs.SetString(SelectedLocationKey, "CityDay");
            PlayerPrefs.Save();
        }

        RefreshButtons();
    }

    public void BuyLocation(string locationId)
    {
        LocationItem item = GetLocation(locationId);

        if (item == null)
            return;

        if (IsBought(locationId))
            return;

        int coins = PlayerPrefs.GetInt("Coins", 0);

        if (coins < item.price)
        {
            Debug.Log("Недостатньо монет");
            return;
        }

        coins -= item.price;
        PlayerPrefs.SetInt("Coins", coins);

        PlayerPrefs.SetInt(BoughtKey + locationId, 1);
        PlayerPrefs.Save();

        if (WalletManager.Instance != null)
            WalletManager.Instance.UpdateCoinsText();

        RefreshButtons();
    }

    public void EquipLocation(string locationId)
    {
        LocationItem item = GetLocation(locationId);

        if (item == null)
            return;

        if (!item.isDefault && !IsBought(locationId))
            return;

        PlayerPrefs.SetString(SelectedLocationKey, locationId);
        PlayerPrefs.Save();

        RefreshButtons();
    }

    public void RefreshButtons()
    {
        string selectedLocation = PlayerPrefs.GetString(SelectedLocationKey, "CityDay");

        foreach (LocationItem item in locations)
        {
            bool bought = item.isDefault || IsBought(item.locationId);
            bool equipped = selectedLocation == item.locationId;

            if (item.priceButton != null)
                item.priceButton.SetActive(!bought);

            if (item.equipButton != null)
                item.equipButton.SetActive(bought && !equipped);

            if (item.equippedButton != null)
                item.equippedButton.SetActive(equipped);
        }
    }

    private bool IsBought(string locationId)
    {
        LocationItem item = GetLocation(locationId);

        if (item != null && item.isDefault)
            return true;

        return PlayerPrefs.GetInt(BoughtKey + locationId, 0) == 1;
    }

    private LocationItem GetLocation(string locationId)
    {
        foreach (LocationItem item in locations)
        {
            if (item.locationId == locationId)
                return item;
        }

        return null;
    }
}