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

    public Vector3 smallCameraPosition =
        new Vector3(0f, 0.5f, -10f);

    public float smallCameraSize16x9 = 7.8f;
    public float smallCameraSize20x9 = 9.6f;

    public int smallWinX = 4;
    public int smallWinY = 6;

    [Header("Levels 16-29")]
    public int bigGridWidth = 7;
    public int bigGridHeight = 13;
    public Vector2 bigGridStart = new Vector2(-2.5f, -5.5f);

    public Vector3 bigCameraPosition =
        new Vector3(0.5f, 0f, -10f);

    public float bigCameraSize16x9 = 9f;
    public float bigCameraSize20x9 = 11f;

    public int bigWinX = 5;
    public int bigWinY = 7;

    [Header("Level 30 Fix")]
    public Vector2 level30GridStart =
        new Vector2(-2.5f, -6.5f);

    public Vector3 level30CameraPosition =
        new Vector3(0.5f, 0f, -10f);

    public float level30CameraSize16x9 = 9f;
    public float level30CameraSize20x9 = 11f;

    [Header("Levels 31-45")]
    public int hugeGridWidth = 8;
    public int hugeGridHeight = 13;
    public Vector2 hugeGridStart =
        new Vector2(-3.5f, -6.5f);

    public Vector3 hugeCameraPosition =
        new Vector3(0f, 0f, -10f);

    public float hugeCameraSize16x9 = 10f;
    public float hugeCameraSize20x9 = 12f;

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

        if (MovesManager.Instance != null)
            MovesManager.Instance.InitMoves();

        if (LevelTimer.Instance != null)
            LevelTimer.Instance.InitTimer();
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

        GridCar[] cars = FindObjectsByType<GridCar>(
            FindObjectsSortMode.None
        );

        foreach (GridCar car in cars)
        {
            if (!car.gameObject.activeInHierarchy)
                continue;

            SetupMainCarWinPosition(car, levelNumber);

            car.transform.position =
                GridManager.Instance.GridToWorld(
                    car.gridX,
                    car.gridY,
                    car.isHorizontal,
                    car.length
                );

            GridManager.Instance.RegisterCar(car);
        }
    }

    private void SetupMainCarWinPosition(
        GridCar car,
        int levelNumber
    )
    {
        if (!car.isMainCar)
            return;

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

    private void SetupGridAndCamera(int levelNumber)
    {
        if (GridManager.Instance == null)
            return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (levelNumber >= 1 && levelNumber <= 15)
        {
            SetupSmallLevel();
        }
        else if (levelNumber >= 16 && levelNumber <= 29)
        {
            SetupBigLevel();
        }
        else if (levelNumber == 30)
        {
            SetupLevel30();
        }
        else if (levelNumber >= 31 && levelNumber <= 45)
        {
            SetupHugeLevel();
        }
        else
        {
            Debug.LogWarning(
                "Unknown level range: " + levelNumber
            );
        }
    }

    private void SetupSmallLevel()
    {
        GridManager.Instance.width = smallGridWidth;
        GridManager.Instance.height = smallGridHeight;
        GridManager.Instance.gridStart = smallGridStart;

        if (mainCamera == null)
            return;

        mainCamera.transform.position =
            smallCameraPosition;

        SetAdaptiveCameraSize(
            smallCameraSize16x9,
            smallCameraSize20x9
        );
    }

    private void SetupBigLevel()
    {
        GridManager.Instance.width = bigGridWidth;
        GridManager.Instance.height = bigGridHeight;
        GridManager.Instance.gridStart = bigGridStart;

        if (mainCamera == null)
            return;

        mainCamera.transform.position =
            bigCameraPosition;

        SetAdaptiveCameraSize(
            bigCameraSize16x9,
            bigCameraSize20x9
        );
    }

    private void SetupLevel30()
    {
        // Розмір сітки такий самий, як у 16–29
        GridManager.Instance.width = bigGridWidth;
        GridManager.Instance.height = bigGridHeight;

        // Окреме положення сітки для 30 рівня
        GridManager.Instance.gridStart =
            level30GridStart;

        if (mainCamera == null)
            return;

        // Окремі налаштування камери 30 рівня
        mainCamera.transform.position =
            level30CameraPosition;

        SetAdaptiveCameraSize(
            level30CameraSize16x9,
            level30CameraSize20x9
        );
    }

    private void SetupHugeLevel()
    {
        GridManager.Instance.width = hugeGridWidth;
        GridManager.Instance.height = hugeGridHeight;
        GridManager.Instance.gridStart = hugeGridStart;

        if (mainCamera == null)
            return;

        mainCamera.transform.position =
            hugeCameraPosition;

        SetAdaptiveCameraSize(
            hugeCameraSize16x9,
            hugeCameraSize20x9
        );
    }

    private void SetAdaptiveCameraSize(
        float size16x9,
        float size20x9
    )
    {
        if (mainCamera == null)
            return;

        float aspect =
            (float)Screen.width / Screen.height;

        float aspect16x9 = 1080f / 1920f;
        float aspect20x9 = 1080f / 2400f;

        float t = Mathf.InverseLerp(
            aspect16x9,
            aspect20x9,
            aspect
        );

        mainCamera.orthographicSize =
            Mathf.Lerp(size16x9, size20x9, t);

        Debug.Log(
            "Screen: " +
            Screen.width + "x" + Screen.height +
            " | Aspect: " + aspect +
            " | Camera Size: " +
            mainCamera.orthographicSize
        );
    }
}