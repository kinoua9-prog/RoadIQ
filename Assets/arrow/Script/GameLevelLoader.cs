using UnityEngine;

public class GameLevelLoader : MonoBehaviour
{
    public GameObject[] levels;

    [Header("Test Mode")]
    public bool useTestLevel = false;
    public int testLevelNumber = 1;

    private void Start()
    {
        int level;

        if (useTestLevel)
            level = testLevelNumber;
        else
            level = PlayerPrefs.GetInt("SelectedLevel", 1);

        LoadLevel(level);
    }

    public void LoadLevel(int levelNumber)
    {
        if (GridManager.Instance != null)
            GridManager.Instance.ClearGrid();

        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].SetActive(i == levelNumber - 1);
        }

        GridCar[] cars = FindObjectsByType<GridCar>(FindObjectsSortMode.None);

        foreach (GridCar car in cars)
        {
            if (!car.gameObject.activeInHierarchy)
                continue;

            car.transform.position = GridManager.Instance.GridToWorld(
                car.gridX,
                car.gridY,
                car.isHorizontal,
                car.length
            );

            GridManager.Instance.RegisterCar(car);
        }

        Debug.Log("Loaded Level: " + levelNumber);
    }
}