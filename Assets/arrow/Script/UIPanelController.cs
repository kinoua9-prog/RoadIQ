using UnityEngine;

public class UIPanelController : MonoBehaviour
{
    public GameObject shopPanel;
    public GameObject levelsPanel;

    private bool openedShopFromLevels = false;

    public void OpenShopFromMain()
    {
        openedShopFromLevels = false;
        shopPanel.SetActive(true);
    }

    public void OpenShopFromLevels()
    {
        openedShopFromLevels = true;
        levelsPanel.SetActive(false);
        shopPanel.SetActive(true);
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);

        if (openedShopFromLevels)
        {
            levelsPanel.SetActive(true);
        }
    }
}