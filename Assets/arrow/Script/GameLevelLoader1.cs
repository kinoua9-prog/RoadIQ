using UnityEngine;

public class GameLevelLoader1 : MonoBehaviour
{
    public GameObject[] levels;

    void Start()
    {
        int level = PlayerPrefs.GetInt("SelectedLevel", 1);

        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].SetActive(false);
        }

        if (level > 0 && level <= levels.Length)
        {
            levels[level - 1].SetActive(true);
        }
    }
}