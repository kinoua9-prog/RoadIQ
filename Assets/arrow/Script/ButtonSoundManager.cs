using UnityEngine;

public class ButtonSoundManager : MonoBehaviour
{
    public static ButtonSoundManager Instance;

    public AudioSource audioSource;
    public AudioClip clickSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayClick()
    {
        if (clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }
}