using UnityEngine;

public class AutoCarSorting : MonoBehaviour
{
    [Header("Sorting")]
    public int baseOrder = 5000;
    public int yMultiplier = 100;

    [Header("Optional")]
    public bool updateEveryFrame = true;

    private void Start()
    {
        UpdateSorting();
    }

    private void LateUpdate()
    {
        if (updateEveryFrame)
            UpdateSorting();
    }

    public void UpdateSorting()
    {
        GridCar[] cars = FindObjectsByType<GridCar>(FindObjectsSortMode.None);

        foreach (GridCar car in cars)
        {
            if (car == null || !car.gameObject.activeInHierarchy)
                continue;

            SpriteRenderer sr = car.GetComponent<SpriteRenderer>();

            if (sr == null)
                continue;

            sr.sortingOrder = baseOrder - Mathf.RoundToInt(car.transform.position.y * yMultiplier);
        }
    }
}