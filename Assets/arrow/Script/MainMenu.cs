using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Level Panels")]
    public GameObject[] levelPanels;

    [Header("Levels Pages Manager")]
    public LevelsPagesManager levelsPagesManager;

    private void Start()
    {
        Time.timeScale = 1f;
    }

    public void OpenLevels()
    {
        levelsPagesManager.ResetToFirstPage();
    }

    public void CloseLevels()
    {
        foreach (GameObject panel in levelPanels)
        {
            panel.SetActive(false);
        }
    }

    public void LoadLevel(int levelNumber)
    {
        // Перевіряємо чи є енергія
        if (EnergyManager.Instance != null)
        {
            if (!EnergyManager.Instance.TryUseEnergy())
            {
                Debug.Log("Недостатньо енергії");
                return;
            }
        }

        PlayerPrefs.SetInt("SelectedLevel", levelNumber);
        PlayerPrefs.Save();

        SceneManager.LoadScene("GameScene");
    }
}