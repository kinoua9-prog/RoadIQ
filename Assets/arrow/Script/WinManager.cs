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

    [Header("Reward UI")]
    public TMP_Text rewardCoinsText;

    [Header("Sound")]
    public AudioSource winSound;

    private bool transitionStarted;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1f;
        transitionStarted = false;

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
                winTimeText.text =
                    LevelTimer.Instance.GetTimeText();
        }

        int used = 0;

        if (MovesManager.Instance != null)
        {
            used = MovesManager.Instance.usedMoves;
            MovesManager.Instance.gameOver = true;
        }
        else
        {
            Debug.LogError(
                "WIN: MovesManager.Instance не знайдений!"
            );
        }

        LevelSettings levelSettings =
            FindFirstObjectByType<LevelSettings>();

        int threeStarMoves = 15;
        int twoStarMoves = 25;

        if (levelSettings != null)
        {
            threeStarMoves =
                levelSettings.threeStarMoves;

            twoStarMoves =
                levelSettings.twoStarMoves;
        }

        int stars = 1;

        if (used <= threeStarMoves)
            stars = 3;
        else if (used <= twoStarMoves)
            stars = 2;

        if (star1 != null)
            star1.SetActive(stars >= 1);

        if (star2 != null)
            star2.SetActive(stars >= 2);

        if (star3 != null)
            star3.SetActive(stars >= 3);

        int reward = GiveCoinsForStars(stars);

        if (rewardCoinsText != null)
            rewardCoinsText.text = "+" + reward;

        Debug.Log("LEVEL COMPLETE");
        Debug.Log("Stars: " + stars);
        Debug.Log("Used moves: " + used);
        Debug.Log("Reward: +" + reward + " coins");
    }

    private int GiveCoinsForStars(int newStars)
    {
        int currentLevel = PlayerPrefs.GetInt(
            "SelectedLevel",
            1
        );

        string starsKey =
            "Level_" + currentLevel + "_Stars";

        int oldStars = PlayerPrefs.GetInt(
            starsKey,
            0
        );

        if (newStars <= oldStars)
            return 0;

        int reward =
            GetRewardByStars(newStars) -
            GetRewardByStars(oldStars);

        if (WalletManager.Instance != null)
        {
            WalletManager.Instance.AddCoins(reward);
        }
        else
        {
            Debug.LogError(
                "WIN: WalletManager.Instance не знайдений!"
            );
        }

        PlayerPrefs.SetInt(starsKey, newStars);
        PlayerPrefs.Save();

        return reward;
    }

    private int GetRewardByStars(int stars)
    {
        switch (stars)
        {
            case 1:
                return 1;

            case 2:
                return 3;

            case 3:
                return 5;

            default:
                return 0;
        }
    }

    public void RestartLevel()
    {
        if (transitionStarted)
            return;

        if (EnergyManager.Instance != null)
        {
            if (!EnergyManager.Instance.TryUseEnergy())
            {
                EnergyManager.Instance.ShowNoEnergyPanel();
                return;
            }
        }

        transitionStarted = true;
        Time.timeScale = 1f;

        ShowPossibleInterstitialAndLoadGame();
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

    public void NextLevel()
    {
        if (transitionStarted)
            return;

        if (EnergyManager.Instance == null)
        {
            Debug.LogError(
                "NEXT LEVEL: EnergyManager.Instance не знайдений!"
            );

            return;
        }

        if (!EnergyManager.Instance.TryUseEnergy())
        {
            EnergyManager.Instance.ShowNoEnergyPanel();

            Debug.Log(
                "NEXT LEVEL: Енергії немає."
            );

            return;
        }

        transitionStarted = true;
        Time.timeScale = 1f;

        int currentLevel = PlayerPrefs.GetInt(
            "SelectedLevel",
            1
        );

        currentLevel++;

        PlayerPrefs.SetInt(
            "SelectedLevel",
            currentLevel
        );

        PlayerPrefs.Save();

        ShowPossibleInterstitialAndLoadGame();
    }

    private void ShowPossibleInterstitialAndLoadGame()
    {
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
                "WIN: AdsManager.Instance не знайдений. " +
                "Сцена відкривається без реклами."
            );

            LoadGameScene();
        }
    }

    private void LoadGameScene()
    {
        Time.timeScale = 1f;

        if (MovesManager.Instance != null)
            MovesManager.Instance.gameOver = false;

        SceneManager.LoadScene("GameScene");
    }
}