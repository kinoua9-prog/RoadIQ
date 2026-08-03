using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseManager : MonoBehaviour
{
    public static LoseManager Instance;

    public GameObject losePanel;
    public TMP_Text loseTimeText;

    private bool transitionStarted;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        transitionStarted = false;

        if (losePanel != null)
            losePanel.SetActive(false);
    }

    public void ShowLosePanel()
    {
        if (MovesManager.Instance != null)
            MovesManager.Instance.gameOver = true;

        if (LevelTimer.Instance != null)
        {
            LevelTimer.Instance.StopTimer();

            if (loseTimeText != null)
            {
                loseTimeText.text =
                    LevelTimer.Instance.GetTimeText();
            }
        }

        if (losePanel != null)
            losePanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        if (transitionStarted)
            return;

        transitionStarted = true;
        Time.timeScale = 1f;

        if (AdsManager.Instance != null)
        {
            AdsManager.Instance
                .TryShowRandomInterstitialAfterAction(
                    LoadGameScene
                );
        }
        else
        {
            Debug.LogWarning(
                "LOSE: AdsManager.Instance не знайдений. " +
                "Рівень перезапускається без реклами."
            );

            LoadGameScene();
        }
    }

    public void OpenMenu()
    {
        if (transitionStarted)
            return;

        transitionStarted = true;
        Time.timeScale = 1f;

        if (MovesManager.Instance != null)
            MovesManager.Instance.gameOver = false;

        SceneManager.LoadScene("MainMenuScene");
    }

    private void LoadGameScene()
    {
        Time.timeScale = 1f;

        if (MovesManager.Instance != null)
            MovesManager.Instance.gameOver = false;

        SceneManager.LoadScene("GameScene");
    }
}