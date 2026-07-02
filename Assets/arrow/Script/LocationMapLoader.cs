using UnityEngine;

public class LocationMapLoader : MonoBehaviour
{
    [Header("City maps: 1-14")]
    public Sprite cityDaySmall;
    public Sprite cityNightSmall;
    public Sprite cityWinterSmall;

    [Header("City maps: 15-30")]
    public Sprite cityDayBig;
    public Sprite cityNightBig;
    public Sprite cityWinterBig;

    [Header("Mall maps: 31-60")]
    public Sprite mallDay;
    public Sprite mallNight;
    public Sprite mallWinter;

    private const string SelectedCityLocationKey = "SelectedCityLocation";
    private const string SelectedMallLocationKey = "SelectedMallLocation";
    private const string SelectedLevelKey = "SelectedLevel";

    private void Start()
    {
        Invoke(nameof(LoadSelectedMap), 0.05f);
    }

    public void LoadSelectedMap()
    {
        int selectedLevel = PlayerPrefs.GetInt(SelectedLevelKey, 1);

        GameObject levelObject = GameObject.Find(selectedLevel.ToString());

        if (levelObject == null)
        {
            Debug.LogError("Не знайдено рівень: " + selectedLevel);
            return;
        }

        SpriteRenderer mapRenderer = levelObject.GetComponent<SpriteRenderer>();

        if (mapRenderer == null)
        {
            Debug.LogError("На рівні " + selectedLevel + " немає SpriteRenderer карти!");
            return;
        }

        Sprite selectedSprite = GetMapSprite(selectedLevel);

        if (selectedSprite == null)
        {
            Debug.LogError("Карта не призначена для рівня: " + selectedLevel);
            return;
        }

        mapRenderer.sprite = selectedSprite;

        Debug.Log("Loaded map sprite | Level: " + selectedLevel);
    }

    private Sprite GetMapSprite(int selectedLevel)
    {
        if (selectedLevel >= 31 && selectedLevel <= 60)
        {
            string mallLocation = PlayerPrefs.GetString(SelectedMallLocationKey, "MallDay");

            if (mallLocation == "MallNight")
                return mallNight;

            if (mallLocation == "MallWinter")
                return mallWinter;

            return mallDay;
        }

        string cityLocation = PlayerPrefs.GetString(SelectedCityLocationKey, "CityDay");

        if (selectedLevel >= 15 && selectedLevel <= 30)
        {
            if (cityLocation == "CityNight")
                return cityNightBig;

            if (cityLocation == "CityWinter")
                return cityWinterBig;

            return cityDayBig;
        }

        if (cityLocation == "CityNight")
            return cityNightSmall;

        if (cityLocation == "CityWinter")
            return cityWinterSmall;

        return cityDaySmall;
    }
}