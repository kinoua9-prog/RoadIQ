using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Level Panels")]
    public GameObject[] levelPanels;

    [Header("Levels Pages Manager")]
    public LevelsPagesManager levelsPagesManager;

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
        PlayerPrefs.SetInt("SelectedLevel", levelNumber);
        SceneManager.LoadScene("GameScene");
    }
}