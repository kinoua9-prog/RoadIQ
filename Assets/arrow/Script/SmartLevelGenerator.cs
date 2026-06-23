using System.Collections.Generic;
using UnityEngine;

public class SmartLevelGenerator : MonoBehaviour
{
    public Transform carsParent;

    public GridCar mainCarTemplate;

    public GridCar[] horizontalCars2;
    public GridCar[] verticalCars2;
    public GridCar[] horizontalCars3;
    public GridCar[] verticalCars3;

    public int minMoves = 12;
    public int maxMoves = 18;

    public int carCount = 14;
    public int maxTries = 300;

    private class DataCar
    {
        public int x;
        public int y;
        public int length;
        public bool horizontal;
        public bool main;
        public GridCar prefab;
    }

    public void GenerateSmartLevel()
    {
        for (int i = 0; i < maxTries; i++)
        {
            List<DataCar> level = CreateRandomLevel();

            int moves = Solve(level);

            if (moves >= minMoves && moves <= maxMoves)
            {
                SpawnLevel(level);
                Debug.Log("SMART LEVEL GENERATED. MOVES: " + moves);
                return;
            }
        }

        Debug.Log("NO GOOD LEVEL FOUND. Try bigger maxTries or change min/max moves.");
    }

    private List<DataCar> CreateRandomLevel()
    {
        List<DataCar> cars = new List<DataCar>();

        DataCar main = new DataCar();
        main.x = 0;
        main.y = 6;
        main.length = 2;
        main.horizontal = true;
        main.main = true;
        main.prefab = mainCarTemplate;
        cars.Add(main);

        int attempts = 0;

        while (cars.Count < carCount && attempts < 1000)
        {
            attempts++;

            bool horizontal = Random.value > 0.5f;
            int length = Random.value > 0.75f ? 3 : 2;

            GridCar prefab = GetRandomPrefab(horizontal, length);
            if (prefab == null) continue;

            int maxX = 6 - (horizontal ? length : 1);
            int maxY = 11 - (horizontal ? 1 : length);

            int x = Random.Range(0, maxX + 1);
            int y = Random.Range(0, maxY + 1);

            DataCar car = new DataCar();
            car.x = x;
            car.y = y;
            car.length = length;
            car.horizontal = horizontal;
            car.main = false;
            car.prefab = prefab;

            if (CanAdd(cars, car))
                cars.Add(car);
        }

        return cars;
    }

    private GridCar GetRandomPrefab(bool horizontal, int length)
    {
        if (horizontal && length == 2 && horizontalCars2.Length > 0)
            return horizontalCars2[Random.Range(0, horizontalCars2.Length)];

        if (!horizontal && length == 2 && verticalCars2.Length > 0)
            return verticalCars2[Random.Range(0, verticalCars2.Length)];

        if (horizontal && length == 3 && horizontalCars3.Length > 0)
            return horizontalCars3[Random.Range(0, horizontalCars3.Length)];

        if (!horizontal && length == 3 && verticalCars3.Length > 0)
            return verticalCars3[Random.Range(0, verticalCars3.Length)];

        return null;
    }

    private bool CanAdd(List<DataCar> cars, DataCar newCar)
    {
        foreach (DataCar car in cars)
        {
            for (int i = 0; i < car.length; i++)
            {
                int ax = car.x + (car.horizontal ? i : 0);
                int ay = car.y + (car.horizontal ? 0 : i);

                for (int j = 0; j < newCar.length; j++)
                {
                    int bx = newCar.x + (newCar.horizontal ? j : 0);
                    int by = newCar.y + (newCar.horizontal ? 0 : j);

                    if (ax == bx && ay == by)
                        return false;
                }
            }
        }

        return true;
    }

    private void SpawnLevel(List<DataCar> level)
    {
        for (int i = carsParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(carsParent.GetChild(i).gameObject);
        }

        GridManager.Instance.ClearGrid();

        foreach (DataCar data in level)
        {
            GridCar car = Instantiate(data.prefab, carsParent);

            car.gridX = data.x;
            car.gridY = data.y;
            car.length = data.length;
            car.isHorizontal = data.horizontal;
            car.isMainCar = data.main;

            car.winX = 4;
            car.winY = 6;

            car.transform.position = GridManager.Instance.GridToWorld(
                car.gridX,
                car.gridY,
                car.isHorizontal,
                car.length
            );

            GridManager.Instance.RegisterCar(car);
        }
    }

    private int Solve(List<DataCar> start)
    {
        Queue<(List<DataCar> cars, int moves)> queue = new Queue<(List<DataCar>, int)>();
        HashSet<string> visited = new HashSet<string>();

        queue.Enqueue((Clone(start), 0));
        visited.Add(Key(start));

        int checkedStates = 0;

        while (queue.Count > 0)
        {
            checkedStates++;

            if (checkedStates > 100000)
                return -1;

            var current = queue.Dequeue();

            foreach (DataCar car in current.cars)
            {
                if (car.main && car.x == 4 && car.y == 6)
                    return current.moves;
            }

            for (int i = 0; i < current.cars.Count; i++)
            {
                DataCar car = current.cars[i];

                if (car.horizontal)
                {
                    TrySolverMove(current.cars, queue, visited, current.moves, i, -1, 0);
                    TrySolverMove(current.cars, queue, visited, current.moves, i, 1, 0);
                }
                else
                {
                    TrySolverMove(current.cars, queue, visited, current.moves, i, 0, -1);
                    TrySolverMove(current.cars, queue, visited, current.moves, i, 0, 1);
                }
            }
        }

        return -1;
    }

    private void TrySolverMove(
        List<DataCar> cars,
        Queue<(List<DataCar> cars, int moves)> queue,
        HashSet<string> visited,
        int moves,
        int index,
        int dx,
        int dy)
    {
        for (int step = 1; step <= 11; step++)
        {
            List<DataCar> copy = Clone(cars);

            copy[index].x += dx * step;
            copy[index].y += dy * step;

            if (!IsValid(copy))
                break;

            string key = Key(copy);

            if (!visited.Contains(key))
            {
                visited.Add(key);
                queue.Enqueue((copy, moves + step));
            }
        }
    }

    private bool IsValid(List<DataCar> cars)
    {
        int[,] grid = new int[6, 11];

        for (int i = 0; i < cars.Count; i++)
        {
            DataCar car = cars[i];

            for (int j = 0; j < car.length; j++)
            {
                int x = car.x + (car.horizontal ? j : 0);
                int y = car.y + (car.horizontal ? 0 : j);

                if (x < 0 || x >= 6 || y < 0 || y >= 11)
                    return false;

                if (grid[x, y] != 0)
                    return false;

                grid[x, y] = i + 1;
            }
        }

        return true;
    }

    private string Key(List<DataCar> cars)
    {
        string key = "";

        foreach (DataCar car in cars)
            key += car.x + "," + car.y + "|";

        return key;
    }

    private List<DataCar> Clone(List<DataCar> original)
    {
        List<DataCar> copy = new List<DataCar>();

        foreach (DataCar car in original)
        {
            DataCar c = new DataCar();
            c.x = car.x;
            c.y = car.y;
            c.length = car.length;
            c.horizontal = car.horizontal;
            c.main = car.main;
            c.prefab = car.prefab;

            copy.Add(c);
        }

        return copy;
    }
}