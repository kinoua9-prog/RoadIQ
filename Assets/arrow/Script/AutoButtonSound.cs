using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AutoButtonSound : MonoBehaviour
{
    private void Start()
    {
        AddSoundToAllButtons();
    }

    public void AddSoundToAllButtons()
    {
        Button[] buttons = FindObjectsByType<Button>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (Button button in buttons)
        {
            if (button.GetComponent<ButtonClickSound>() == null)
            {
                button.gameObject.AddComponent<ButtonClickSound>();
            }
        }
    }
}

public class ButtonClickSound : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (ButtonSoundManager.Instance != null)
        {
            ButtonSoundManager.Instance.PlayClick();
        }
    }
}