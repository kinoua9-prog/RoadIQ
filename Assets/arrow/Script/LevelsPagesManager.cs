using TMPro;
using UnityEngine;

public class LevelsPagesManager : MonoBehaviour
{
    [Header("Level Pages")]
    public GameObject[] levelPanels;

    [Header("Locked Panel")]
    public GameObject lockedPanel;
    public TMP_Text currentStarsText;
    public TMP_Text requiredStarsText;

    [Header("Unlock Settings")]
    public int[] starsRequiredForPages;

    private int currentPage = 0;

    private void Start()
    {
        if (lockedPanel != null)
            lockedPanel.SetActive(false);

        ResetToFirstPage();
    }

    public void ResetToFirstPage()
    {
        currentPage = 0;
        ShowPage(currentPage);
    }

    public void NextPage()
    {
        int nextPage = currentPage + 1;

        if (nextPage >= levelPanels.Length)
            return;

        if (CanOpenPage(nextPage))
        {
            ShowPage(nextPage);
        }
        else
        {
            ShowLockedPanel(nextPage);
        }
    }

    public void PreviousPage()
    {
        int previousPage = currentPage - 1;

        if (previousPage < 0)
            return;

        ShowPage(previousPage);
    }

    public void CloseLockedPanel()
    {
        if (lockedPanel != null)
            lockedPanel.SetActive(false);
    }

    private void ShowPage(int pageIndex)
    {
        currentPage = pageIndex;

        for (int i = 0; i < levelPanels.Length; i++)
        {
            if (levelPanels[i] != null)
                levelPanels[i].SetActive(i == currentPage);
        }
    }

    private bool CanOpenPage(int pageIndex)
    {
        if (pageIndex <= 0)
            return true;

        if (starsRequiredForPages == null || pageIndex >= starsRequiredForPages.Length)
            return true;

        int requiredStars = starsRequiredForPages[pageIndex];
        int totalStars = GetTotalStars();

        return totalStars >= requiredStars;
    }

    private void ShowLockedPanel(int pageIndex)
    {
        int requiredStars = 0;

        if (starsRequiredForPages != null && pageIndex < starsRequiredForPages.Length)
            requiredStars = starsRequiredForPages[pageIndex];

        int totalStars = GetTotalStars();

        if (currentStarsText != null)
            currentStarsText.text = totalStars.ToString();

        if (requiredStarsText != null)
            requiredStarsText.text = requiredStars.ToString();

        if (lockedPanel != null)
        {
            lockedPanel.SetActive(true);
            lockedPanel.transform.SetAsLastSibling();
        }
    }

    private int GetTotalStars()
    {
        int total = 0;

        for (int i = 1; i <= 100; i++)
        {
            int starsA = PlayerPrefs.GetInt("Level_" + i + "_Stars", 0);
            int starsB = PlayerPrefs.GetInt("LevelStars_" + i, 0);

            total += Mathf.Max(starsA, starsB);
        }

        return total;
    }
}