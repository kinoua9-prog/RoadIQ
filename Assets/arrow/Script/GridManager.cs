using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public int width = 6;
    public int height = 11;
    public float cellSize = 1f;

    public Vector2 gridStart = new Vector2(-2.5f, -4.5f);

    private GridCar[,] grid;

    private void Awake()
    {
        Instance = this;
        grid = new GridCar[width, height];
    }

    public Vector3 GridToWorld(int x, int y, bool isHorizontal = true, int length = 1)
    {
        float offsetX = isHorizontal ? (length - 1) * cellSize * 0.5f : 0f;
        float offsetY = isHorizontal ? 0f : (length - 1) * cellSize * 0.5f;

        return new Vector3(
            gridStart.x + x * cellSize + offsetX,
            gridStart.y + y * cellSize + offsetY,
            0f
        );
    }

    public bool IsInside(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public void RegisterCar(GridCar car)
    {
        if (car == null)
            return;

        for (int i = 0; i < car.length; i++)
        {
            int x = car.gridX + (car.isHorizontal ? i : 0);
            int y = car.gridY + (car.isHorizontal ? 0 : i);

            if (IsInside(x, y))
                grid[x, y] = car;
        }
    }

    public void UnregisterCar(GridCar car)
    {
        if (car == null)
            return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == car)
                    grid[x, y] = null;
            }
        }
    }

    public bool CanPlace(GridCar car, int newX, int newY)
    {
        if (car == null)
            return false;

        for (int i = 0; i < car.length; i++)
        {
            int x = newX + (car.isHorizontal ? i : 0);
            int y = newY + (car.isHorizontal ? 0 : i);

            if (!IsInside(x, y))
                return false;

            if (grid[x, y] != null && grid[x, y] != car)
                return false;
        }

        return true;
    }

    public void MoveCarToCell(GridCar car, int newX, int newY)
    {
        if (car == null)
            return;

        UnregisterCar(car);

        car.gridX = newX;
        car.gridY = newY;

        car.transform.position = GridToWorld(
            car.gridX,
            car.gridY,
            car.isHorizontal,
            car.length
        );

        RegisterCar(car);
    }

    public void ClearGrid()
    {
        grid = new GridCar[width, height];
    }
}