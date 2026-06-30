using UnityEngine;

public static class LocationBonus
{
    private const string SelectedLocationKey = "SelectedLocation";

    public static int GetBonusMoves()
    {
        string location = PlayerPrefs.GetString(SelectedLocationKey, "CityDay");

        switch (location)
        {
            case "CityNight":
                return 1;

            case "CityWinter":
                return 2;

            default:
                return 0;
        }
    }
}