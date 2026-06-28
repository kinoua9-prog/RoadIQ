using UnityEngine;

public class UIPanelController : MonoBehaviour
{
    [Header("Shop")]
    public GameObject shopPanel;

    [Header("Level Panels")]
    public GameObject[] levelPanels;

    private bool openedShopFromLevels = false;
    private int lastOpenedLevelPanel = 0;

    public void OpenShopFromMain()
    {
        openedShopFromLevels = false;

        CloseAllLevelPanels();

        shopPanel.SetActive(true);
    }

    public void OpenShopFromLevels()
    {
        openedShopFromLevels = true;

        lastOpenedLevelPanel = GetActiveLevelPanelIndex();

        CloseAllLevelPanels();

        shopPanel.SetActive(true);
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);

        if (openedShopFromLevels)
        {
            levelPanels[lastOpenedLevelPanel].SetActive(true);
        }
    }

    private void CloseAllLevelPanels()
    {
        foreach (GameObject panel in levelPanels)
        {
            panel.SetActive(false);
        }
    }

    private int GetActiveLevelPanelIndex()
    {
        for (int i = 0; i < levelPanels.Length; i++)
        {
            if (levelPanels[i].activeSelf)
                return i;
        }

        return 0;
    }
}