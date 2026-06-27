using UnityEngine;

public class HintPanel : MonoBehaviour
{
    public GameObject panel;

    bool opened;

    public void TogglePanel()
    {
        opened = !opened;

        panel.SetActive(opened);
    }
}