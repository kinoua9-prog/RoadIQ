using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject levelsPanel;

    public void OpenLevels()
    {
        levelsPanel.SetActive(true);
    }

    public void CloseLevels()
    {
        levelsPanel.SetActive(false);
    }

    public void LoadLevel(int levelNumber)
    {
        PlayerPrefs.SetInt("SelectedLevel", levelNumber);
        SceneManager.LoadScene("GameScene");
    }
}