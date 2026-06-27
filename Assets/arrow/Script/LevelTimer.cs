using TMPro;
using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    public static LevelTimer Instance;

    public TMP_Text timerText;

    public float elapsedTime;
    public bool timerRunning = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        elapsedTime = 0f;
        timerRunning = true;
        UpdateText();
    }

    private void Update()
    {
        if (!timerRunning)
            return;

        elapsedTime += Time.deltaTime;
        UpdateText();
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    // ===== Новий метод =====
    public void AddTime(float seconds)
    {
        elapsedTime -= seconds;

        if (elapsedTime < 0f)
            elapsedTime = 0f;

        UpdateText();
    }

    public string GetTimeText()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    private void UpdateText()
    {
        if (timerText != null)
            timerText.text = GetTimeText();
    }
}