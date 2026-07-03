using TMPro;
using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    public static LevelTimer Instance;

    public TMP_Text timerText;

    [Header("Level Time")]
    public float levelTime = 60f; // Час на рівень у секундах

    public float currentTime;
    public bool timerRunning = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentTime = levelTime;
        timerRunning = true;
        UpdateText();
    }

    private void Update()
    {
        if (!timerRunning)
            return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            timerRunning = false;

            if (LoseManager.Instance != null)
                LoseManager.Instance.ShowLosePanel();
        }

        UpdateText();
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public void AddTime(float seconds)
    {
        currentTime += seconds;
        UpdateText();
    }

    public string GetTimeText()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    private void UpdateText()
    {
        if (timerText != null)
            timerText.text = GetTimeText();
    }
}