using UnityEngine;

public class GameLevelLoader : MonoBehaviour
{
    public GameObject[] levels;

    [Header("Camera")]
    public Camera mainCamera;

    [Header("Levels 1-15")]
    public int smallGridWidth = 6;
    public int smallGridHeight = 11;
    public Vector2 smallGridStart = new Vector2(-2.5f, -4.5f);
    public Vector3 smallCameraPosition = new Vector3(0f, 0.5f, -10f);
    public float smallCameraSize = 7.8f;
    public int smallWinX = 4;
    public int smallWinY = 6;

    [Header("Levels 16-29")]
    public int bigGridWidth = 7;
    public int bigGridHeight = 13;
    public Vector2 bigGridStart = new Vector2(-2.5f, -5.5f);
    public Vector3 bigCameraPosition = new Vector3(0.5f, 0f, -10f);
    public float bigCameraSize = 9f;
    public int bigWinX = 4;
    public int bigWinY = 7;

    [Header("Level 30 Fix")]
    public Vector2 level30GridStart = new Vector2(-2.5f, -6.5f);

    [Header("Levels 31-45")]
    public int hugeGridWidth = 8;
    public int hugeGridHeight = 13;
    public Vector2 hugeGridStart = new Vector2(-3.5f, -6.5f);
    public Vector3 hugeCameraPosition = new Vector3(0f, 0f, -10f);
    public float hugeCameraSize = 10f;
    public int hugeWinX = 3;
    public int hugeWinY = 0;

    [Header("Test Mode")]
    public bool useTestLevel = false;
    public int testLevelNumber = 1;

    private void Start()
    {
        Time.timeScale = 1f;

        int level = useTestLevel
            ? testLevelNumber
            : PlayerPrefs.GetInt("SelectedLevel", 1);

        LoadLevel(level);
    }

    public void LoadLevel(int levelNumber)
    {
        Time.timeScale = 1f;

        Debug.Log("Loaded Level: " + levelNumber);

        SetupGridAndCamera(levelNumber);

        if (GridManager.Instance != null)
            GridManager.Instance.ClearGrid();

        int levelIndex = levelNumber - 1;

        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].SetActive(i == levelIndex);
        }

        GridCar[] cars = FindObjectsByType<GridCar>(FindObjectsSortMode.None);

        foreach (GridCar car in cars)
        {
            if (!car.gameObject.activeInHierarchy)
                continue;

            if (car.isMainCar)
            {
                if (levelNumber >= 1 && levelNumber <= 15)
                {
                    car.winX = smallWinX;
                    car.winY = smallWinY;
                    car.exitDirection = ExitDirection.Right;
                }
                else if (levelNumber >= 16 && levelNumber <= 30)
                {
                    car.winX = bigWinX;
                    car.winY = bigWinY;
                    car.exitDirection = ExitDirection.Right;
                }
                else if (levelNumber >= 31 && levelNumber <= 45)
                {
                    car.winX = hugeWinX;
                    car.winY = hugeWinY;
                    car.exitDirection = ExitDirection.Down;
                }
            }

            car.transform.position = GridManager.Instance.GridToWorld(
                car.gridX,
                car.gridY,
                car.isHorizontal,
                car.length
            );

            GridManager.Instance.RegisterCar(car);
        }
    }

    private void SetupGridAndCamera(int levelNumber)
    {
        if (GridManager.Instance == null)
            return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (levelNumber >= 1 && levelNumber <= 15)
        {
            GridManager.Instance.width = smallGridWidth;
            GridManager.Instance.height = smallGridHeight;
            GridManager.Instance.gridStart = smallGridStart;

            if (mainCamera != null)
            {
                mainCamera.transform.position = smallCameraPosition;
                mainCamera.orthographicSize = smallCameraSize;
            }
        }
        else if (levelNumber >= 16 && levelNumber <= 30)
        {
            GridManager.Instance.width = bigGridWidth;
            GridManager.Instance.height = bigGridHeight;

            if (levelNumber == 30)
                GridManager.Instance.gridStart = level30GridStart;
            else
                GridManager.Instance.gridStart = bigGridStart;

            if (mainCamera != null)
            {
                mainCamera.transform.position = bigCameraPosition;
                mainCamera.orthographicSize = bigCameraSize;
            }
        }
        else if (levelNumber >= 31 && levelNumber <= 45)
        {
            GridManager.Instance.width = hugeGridWidth;
            GridManager.Instance.height = hugeGridHeight;
            GridManager.Instance.gridStart = hugeGridStart;

            if (mainCamera != null)
            {
                mainCamera.transform.position = hugeCameraPosition;
                mainCamera.orthographicSize = hugeCameraSize;
            }
        }
        else
        {
            Debug.LogWarning("Unknown level range: " + levelNumber);
        }
    }
}