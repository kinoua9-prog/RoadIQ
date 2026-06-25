using UnityEngine;

public class LocationsManager : MonoBehaviour
{
    public GameObject locationsPanel;

    [Header("Location Buttons / Checkmarks")]
    public GameObject cityEquipped;
    public GameObject mallEquipped;
    public GameObject winterEquipped;
    public GameObject nightEquipped;

    private const string SelectedLocationKey = "SelectedLocation";

    private void Start()
    {
        if (locationsPanel != null)
            locationsPanel.SetActive(false);

        UpdateLocationUI();
    }

    public void OpenLocationsPanel()
    {
        if (locationsPanel != null)
            locationsPanel.SetActive(true);

        UpdateLocationUI();
    }

    public void CloseLocationsPanel()
    {
        if (locationsPanel != null)
            locationsPanel.SetActive(false);
    }

    public void SelectCityParking()
    {
        SelectLocation(0);
    }

    public void SelectMallParking()
    {
        SelectLocation(1);
    }

    public void SelectWinterParking()
    {
        SelectLocation(2);
    }

    public void SelectNightParking()
    {
        SelectLocation(3);
    }

    private void SelectLocation(int locationIndex)
    {
        PlayerPrefs.SetInt(SelectedLocationKey, locationIndex);
        PlayerPrefs.Save();

        UpdateLocationUI();

        Debug.Log("Selected location: " + locationIndex);
    }

    private void UpdateLocationUI()
    {
        int selectedLocation = PlayerPrefs.GetInt(SelectedLocationKey, 0);

        if (cityEquipped != null)
            cityEquipped.SetActive(selectedLocation == 0);

        if (mallEquipped != null)
            mallEquipped.SetActive(selectedLocation == 1);

        if (winterEquipped != null)
            winterEquipped.SetActive(selectedLocation == 2);

        if (nightEquipped != null)
            nightEquipped.SetActive(selectedLocation == 3);
    }
}