using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialPanel;
    public Image tutorialImage;

    public Sprite[] tutorialSprites; // 3 картинки

    private int currentIndex = 0;

    private void Start()
    {
        int selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 1);

        if (selectedLevel == 1)
        {
            ShowTutorial();
        }
        else
        {
            tutorialPanel.SetActive(false);
        }
    }

    private void ShowTutorial()
    {
        currentIndex = 0;
        tutorialPanel.SetActive(true);
        Time.timeScale = 0f;

        tutorialImage.sprite = tutorialSprites[currentIndex];
    }

    public void NextTutorial()
    {
        currentIndex++;

        if (currentIndex >= tutorialSprites.Length)
        {
            CloseTutorial();
            return;
        }

        tutorialImage.sprite = tutorialSprites[currentIndex];
    }

    private void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}