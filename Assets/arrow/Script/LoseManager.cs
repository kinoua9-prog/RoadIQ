using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoseManager : MonoBehaviour
{
    public static LoseManager Instance;

    public GameObject losePanel;
    public TMP_Text loseTimeText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (losePanel != null)
            losePanel.SetActive(false);
    }

    public void ShowLosePanel()
    {
        // Блокуємо рух машин
        if (MovesManager.Instance != null)
            MovesManager.Instance.gameOver = true;

        // Зупиняємо таймер
        if (LevelTimer.Instance != null)
        {
            LevelTimer.Instance.StopTimer();

            if (loseTimeText != null)
                loseTimeText.text = LevelTimer.Instance.GetTimeText();
        }

        // Показуємо панель програшу
        if (losePanel != null)
            losePanel.SetActive(true);

        // Ставимо гру на паузу
        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        if (MovesManager.Instance != null)
            MovesManager.Instance.gameOver = false;

        SceneManager.LoadScene("GameScene");
    }

    public void OpenMenu()
    {
        Time.timeScale = 1f;

        if (MovesManager.Instance != null)
            MovesManager.Instance.gameOver = false;

        SceneManager.LoadScene("MainMenuScene");
    }
}