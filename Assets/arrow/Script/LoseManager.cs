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
        losePanel.SetActive(true);

        if (LevelTimer.Instance != null)
        {
            LevelTimer.Instance.StopTimer();

            if (loseTimeText != null)
                loseTimeText.text = LevelTimer.Instance.GetTimeText();
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }

    public void OpenMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
}