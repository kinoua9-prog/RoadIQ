using UnityEngine;

public class LevelButtonStars : MonoBehaviour
{
    public int levelNumber;

    public GameObject star1;
    public GameObject star2;
    public GameObject star3;

    private void OnEnable()
    {
        UpdateStars();
    }

    private void Start()
    {
        UpdateStars();
    }

    public void UpdateStars()
    {
        string key = "Level_" + levelNumber + "_Stars";
        int stars = PlayerPrefs.GetInt(key, 0);

        if (star1 != null)
            star1.SetActive(stars >= 1);

        if (star2 != null)
            star2.SetActive(stars >= 2);

        if (star3 != null)
            star3.SetActive(stars >= 3);
    }
}