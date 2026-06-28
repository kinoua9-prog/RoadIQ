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
        // Якщо звукові ефекти вимкнені – нічого не відтворюємо
        if (PlayerPrefs.GetInt("SFX", 1) == 0)
            return;

        if (clickSound != null && audioSource != null)
            audioSource.PlayOneShot(clickSound);
    }
}