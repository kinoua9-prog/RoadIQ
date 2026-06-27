using UnityEngine;
using UnityEngine.UI;

public class AutoButtonSound : MonoBehaviour
{
    private void Start()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() =>
            {
                if (ButtonSoundManager.Instance != null)
                    ButtonSoundManager.Instance.PlayClick();
            });
        }
    }
}