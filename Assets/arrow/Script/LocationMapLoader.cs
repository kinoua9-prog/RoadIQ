using UnityEngine;

public class LocationMapLoader : MonoBehaviour
{
    [Header("Small maps: 1-14")]
    public Sprite cityDaySmall;
    public Sprite cityNightSmall;
    public Sprite cityWinterSmall;

    [Header("Big maps: 15-30")]
    public Sprite cityDayBig;
    public Sprite cityNightBig;
    public Sprite cityWinterBig;

    private const string SelectedLocationKey = "SelectedLocation";
    private const string SelectedLevelKey = "SelectedLevel";

    private void Start()
    {
        Invoke(nameof(LoadSelectedMap), 0.05f);
    }

    public void LoadSelectedMap()
    {
        string selectedLocation = PlayerPrefs.GetString(SelectedLocationKey, "CityDay");
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

        bool useBigMap = selectedLevel >= 15 && selectedLevel <= 30;

        Sprite selectedSprite;

        if (selectedLocation == "CityNight")
            selectedSprite = useBigMap ? cityNightBig : cityNightSmall;
        else if (selectedLocation == "CityWinter")
            selectedSprite = useBigMap ? cityWinterBig : cityWinterSmall;
        else
            selectedSprite = useBigMap ? cityDayBig : cityDaySmall;

        mapRenderer.sprite = selectedSprite;

        Debug.Log("Loaded map sprite: " + selectedLocation + " | Level: " + selectedLevel);
    }
}