using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GridCar mainCarPrefab;
    public GridCar[] carPrefabs;

    public int carCount = 12;
    public Transform carsParent;

    private List<GridCar> spawnedCars = new List<GridCar>();

    public void GenerateLevel()
    {
        ClearLevel();

        SpawnMainCar();

        int attempts = 0;
        int spawned = 0;

        while (spawned < carCount && attempts < 500)
        {
            attempts++;

            GridCar prefab = carPrefabs[Random.Range(0, carPrefabs.Length)];

            bool horizontal = Random.value > 0.5f;
            int length = Random.value > 0.75f ? 3 : 2;

            int maxX = GridManager.Instance.width - (horizontal ? length : 1);
            int maxY = GridManager.Instance.height - (horizontal ? 1 : length);

            int x = Random.Range(0, maxX + 1);
            int y = Random.Range(0, maxY + 1);

            if (CanSpawn(x, y, horizontal, length))
            {
                GridCar car = Instantiate(prefab, carsParent);

                car.InitCar(
                    x,
                    y,
                    horizontal,
                    length,
                    false
                );

                spawnedCars.Add(car);

                spawned++;
            }
        }

        Debug.Log("LEVEL GENERATED");
    }

    private void SpawnMainCar()
    {
        GridCar car = Instantiate(mainCarPrefab, carsParent);

        car.InitCar(
            0,      // x
            5,      // y
            true,   // horizontal
            2,      // length
            true    // main car
        );

        car.winX = 4;
        car.winY = 5;

        spawnedCars.Add(car);
    }

    private bool CanSpawn(int x, int y, bool horizontal, int length)
    {
        for (int i = 0; i < length; i++)
        {
            int checkX = x + (horizontal ? i : 0);
            int checkY = y + (horizontal ? 0 : i);

            if (!GridManager.Instance.IsInside(checkX, checkY))
                return false;

            foreach (GridCar car in spawnedCars)
            {
                for (int j = 0; j < car.length; j++)
                {
                    int carX = car.gridX + (car.isHorizontal ? j : 0);
                    int carY = car.gridY + (car.isHorizontal ? 0 : j);

                    if (checkX == carX && checkY == carY)
                        return false;
                }
            }
        }

        if (y == 5 && horizontal)
            return false;

        return true;
    }

    private void ClearLevel()
    {
        foreach (GridCar car in spawnedCars)
        {
            if (car != null)
                Destroy(car.gameObject);
        }

        spawnedCars.Clear();

       
    }
}