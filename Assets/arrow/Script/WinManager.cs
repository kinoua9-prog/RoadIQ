using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinManager : MonoBehaviour
{
    public static WinManager Instance;

    public GameObject winPanel;

    public GameObject star1;
    public GameObject star2;
    public GameObject star3;

    public TMP_Text winTimeText;

    [Header("Sound")]
    public AudioSource winSound;

    public int threeStarMoves = 15;
    public int twoStarMoves = 25;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1f;

        if (winPanel != null)
            winPanel.SetActive(false);
    }

    public void ShowWinPanel()
    {
        if (winPanel != null)
            winPanel.SetActive(true);

        if (winSound != null)
            winSound.Play();

        if (LevelTimer.Instance != null)
        {
            LevelTimer.Instance.StopTimer();

            if (winTimeText != null)
                winTimeText.text = LevelTimer.Instance.GetTimeText();
        }

        int used = MovesManager.Instance.usedMoves;
        int stars = 1;

        if (used <= threeStarMoves)
            stars = 3;
        else if (used <= twoStarMoves)
            stars = 2;
        else
            stars = 1;

        if (star1 != null)
            star1.SetActive(stars >= 1);

        if (star2 != null)
            star2.SetActive(stars >= 2);

        if (star3 != null)
            star3.SetActive(stars >= 3);

        Debug.Log("LEVEL COMPLETE");
        Debug.Log("Stars: " + stars);
        Debug.Log("Used moves: " + used);
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

    public void NextLevel()
    {
        Time.timeScale = 1f;

        int currentLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        currentLevel++;

        PlayerPrefs.SetInt("SelectedLevel", currentLevel);

        SceneManager.LoadScene("GameScene");
    }
}