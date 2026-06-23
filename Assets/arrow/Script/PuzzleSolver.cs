using System.Collections.Generic;
using UnityEngine;

public class PuzzleSolver : MonoBehaviour
{
    public int width = 6;
    public int height = 11;

    public int maxSearchStates = 100000;

    private class SolverCar
    {
        public int x;
        public int y;
        public int length;
        public bool horizontal;
        public bool main;
    }

    private class SolverState
    {
        public List<SolverCar> cars;
        public int moves;
        public string path;
    }

    public void CheckCurrentLevel()
    {
        GridCar[] sceneCars = FindObjectsOfType<GridCar>();

        List<SolverCar> startCars = new List<SolverCar>();

        foreach (GridCar car in sceneCars)
        {
            if (!car.gameObject.activeInHierarchy)
                continue;

            SolverCar solverCar = new SolverCar();

            solverCar.x = car.gridX;
            solverCar.y = car.gridY;
            solverCar.length = car.length;
            solverCar.horizontal = car.isHorizontal;
            solverCar.main = car.isMainCar;

            startCars.Add(solverCar);
        }

        Debug.Log("Cars found: " + startCars.Count);

        int result = Solve(startCars, out string solutionPath);

        if (result == -2)
        {
            Debug.Log("SEARCH LIMIT REACHED. Try bigger Max Search States.");
        }
        else if (result == -1)
        {
            Debug.Log("LEVEL REALLY NOT SOLVABLE");
        }
        else
        {
            Debug.Log("LEVEL SOLVABLE");
            Debug.Log("MINIMUM MOVES: " + result);
            Debug.Log(solutionPath);
        }

        Debug.Log("CHECK FINISHED");
    }

    private int Solve(List<SolverCar> startCars, out string solutionPath)
    {
        Queue<SolverState> queue = new Queue<SolverState>();
        HashSet<string> visited = new HashSet<string>();

        SolverState start = new SolverState();
        start.cars = CloneCars(startCars);
        start.moves = 0;
        start.path = "";

        queue.Enqueue(start);
        visited.Add(GetKey(start.cars));

        int checkedStates = 0;

        while (queue.Count > 0)
        {
            checkedStates++;
            if (checkedStates > maxSearchStates)
            {
                solutionPath = "";
                return -2;
            }

            SolverState current = queue.Dequeue();

            if (IsSolved(current.cars))
            {
                solutionPath = current.path;
                return current.moves;
            }

            List<SolverState> nextStates = GetNextStates(current);

            foreach (SolverState next in nextStates)
            {
                string key = GetKey(next.cars);

                if (!visited.Contains(key))
                {
                    visited.Add(key);
                    queue.Enqueue(next);
                }
            }
        }

        solutionPath = "";
        return -1;
    }

    private List<SolverState> GetNextStates(SolverState state)
    {
        List<SolverState> result = new List<SolverState>();

        for (int i = 0; i < state.cars.Count; i++)
        {
            SolverCar car = state.cars[i];

            if (car.horizontal)
            {
                TryMove(state, result, i, -1, 0, "LEFT");
                TryMove(state, result, i, 1, 0, "RIGHT");
            }
            else
            {
                TryMove(state, result, i, 0, -1, "DOWN");
                TryMove(state, result, i, 0, 1, "UP");
            }
        }

        return result;
    }

    private void TryMove(SolverState state, List<SolverState> result, int carIndex, int dx, int dy, string direction)
    {
        for (int step = 1; step <= 11; step++)
        {
            List<SolverCar> newCars = CloneCars(state.cars);

            newCars[carIndex].x += dx * step;
            newCars[carIndex].y += dy * step;

            if (!IsValid(newCars))
                break;

            SolverState newState = new SolverState();
            newState.cars = newCars;
            newState.moves = state.moves + step;
            newState.path = state.path + "\nCar " + carIndex + " -> " + direction + " " + step;

            result.Add(newState);
        }
    }

    private bool IsValid(List<SolverCar> cars)
    {
        int[,] grid = new int[width, height];

        for (int i = 0; i < cars.Count; i++)
        {
            SolverCar car = cars[i];

            for (int j = 0; j < car.length; j++)
            {
                int x = car.x + (car.horizontal ? j : 0);
                int y = car.y + (car.horizontal ? 0 : j);

                if (x < 0 || x >= width || y < 0 || y >= height)
                    return false;

                if (grid[x, y] != 0)
                    return false;

                grid[x, y] = i + 1;
            }
        }

        return true;
    }

    private bool IsSolved(List<SolverCar> cars)
    {
        foreach (SolverCar car in cars)
        {
            if (!car.main)
                continue;

            return car.x == 4 && car.y == 6;
        }

        return false;
    }

    private string GetKey(List<SolverCar> cars)
    {
        string key = "";

        for (int i = 0; i < cars.Count; i++)
        {
            key += cars[i].x + "," + cars[i].y + "|";
        }

        return key;
    }

    private List<SolverCar> CloneCars(List<SolverCar> original)
    {
        List<SolverCar> copy = new List<SolverCar>();

        foreach (SolverCar car in original)
        {
            SolverCar newCar = new SolverCar();

            newCar.x = car.x;
            newCar.y = car.y;
            newCar.length = car.length;
            newCar.horizontal = car.horizontal;
            newCar.main = car.main;

            copy.Add(newCar);
        }

        return copy;
    }
}