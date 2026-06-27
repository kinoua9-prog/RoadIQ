using UnityEngine;

public class LevelsPagesManager : MonoBehaviour
{
    public GameObject[] levelPanels;

    private int currentPage = 0;

    private void Start()
    {
        ResetToFirstPage();
    }

    public void ResetToFirstPage()
    {
        currentPage = 0;
        ShowPage(currentPage);
    }

    public void NextPage()
    {
        if (currentPage < levelPanels.Length - 1)
        {
            currentPage++;
            ShowPage(currentPage);
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            ShowPage(currentPage);
        }
    }

    private void ShowPage(int pageIndex)
    {
        currentPage = pageIndex;

        for (int i = 0; i < levelPanels.Length; i++)
        {
            levelPanels[i].SetActive(i == currentPage);
        }
    }
}