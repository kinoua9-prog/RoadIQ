using UnityEngine;

public class SettingsToggleButton : MonoBehaviour
{
    public string settingKey = "Music";

    public GameObject onImage;
    public GameObject offImage;

    private bool isOn;

    private void Start()
    {
        LoadState();
    }

    public void Toggle()
    {
        isOn = !isOn;

        PlayerPrefs.SetInt(settingKey, isOn ? 1 : 0);
        PlayerPrefs.Save();

        UpdateUI();
        ApplySetting();
    }

    private void LoadState()
    {
        isOn = PlayerPrefs.GetInt(settingKey, 1) == 1;

        UpdateUI();
        ApplySetting();
    }

    private void UpdateUI()
    {
        if (onImage != null)
            onImage.SetActive(isOn);

        if (offImage != null)
            offImage.SetActive(!isOn);
    }

    private void ApplySetting()
    {
        AudioSource[] allAudio = FindObjectsByType<AudioSource>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (AudioSource audio in allAudio)
        {
            if (settingKey == "Music" && audio.CompareTag("Music"))
            {
                audio.mute = !isOn;
            }

            if (settingKey == "SFX" && audio.CompareTag("SFX"))
            {
                audio.mute = !isOn;
            }
        }
    }
}