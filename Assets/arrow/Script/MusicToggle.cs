using UnityEngine;

public class MusicToggle : MonoBehaviour
{
    public GameObject onImage;
    public GameObject offImage;

    private bool musicOn = true;

    private void Start()
    {
        musicOn = PlayerPrefs.GetInt("Music", 1) == 1;
        UpdateUI();
        ApplyMusic();
    }

    public void ToggleMusic()
    {
        musicOn = !musicOn;

        PlayerPrefs.SetInt("Music", musicOn ? 1 : 0);
        PlayerPrefs.Save();

        UpdateUI();
        ApplyMusic();
    }

    private void UpdateUI()
    {
        onImage.SetActive(musicOn);
        offImage.SetActive(!musicOn);
    }

    private void ApplyMusic()
    {
        AudioSource[] allAudio = FindObjectsByType<AudioSource>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (AudioSource audio in allAudio)
        {
            if (audio.CompareTag("Music"))
            {
                audio.mute = !musicOn;
            }
        }
    }
}