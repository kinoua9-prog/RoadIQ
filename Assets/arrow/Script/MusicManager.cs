using UnityEngine;

public class LevelMusicManager : MonoBehaviour
{
    public AudioSource musicSource;

    [Range(0f, 1f)]
    public float volume = 0.4f;

    private void Start()
    {
        if (musicSource == null)
            musicSource = GetComponent<AudioSource>();

        musicSource.volume = volume;
        musicSource.loop = true;

        if (musicSource.clip != null)
        {
            float randomTime = Random.Range(0f, musicSource.clip.length);
            musicSource.time = randomTime;
            musicSource.Play();
        }
    }
}