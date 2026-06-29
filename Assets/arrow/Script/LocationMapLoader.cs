using UnityEngine;

public class LocationMapLoader : MonoBehaviour
{
    [Header("City Maps")]
    public GameObject cityDayMap;
    public GameObject cityNightMap;
    public GameObject cityWinterMap;

    private const string SelectedLocationKey = "SelectedLocation";

    private void Start()
    {
        LoadSelectedMap();
    }

    public void LoadSelectedMap()
    {
        string selectedLocation = PlayerPrefs.GetString(SelectedLocationKey, "CityDay");

        if (cityDayMap != null)
            cityDayMap.SetActive(false);

        if (cityNightMap != null)
            cityNightMap.SetActive(false);

        if (cityWinterMap != null)
            cityWinterMap.SetActive(false);

        if (selectedLocation == "CityNight")
        {
            if (cityNightMap != null)
                cityNightMap.SetActive(true);
        }
        else if (selectedLocation == "CityWinter")
        {
            if (cityWinterMap != null)
                cityWinterMap.SetActive(true);
        }
        else
        {
            if (cityDayMap != null)
                cityDayMap.SetActive(true);
        }

        Debug.Log("Loaded location map: " + selectedLocation);
    }
}