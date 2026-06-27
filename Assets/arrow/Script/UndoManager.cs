using System.Collections.Generic;
using UnityEngine;

public class UndoManager : MonoBehaviour
{
    public static UndoManager Instance;

    private class MoveData
    {
        public GridCar car;
        public int oldX;
        public int oldY;
    }

    private Stack<MoveData> history = new Stack<MoveData>();

    private void Awake()
    {
        Instance = this;
    }

    public void SaveMove(GridCar car, int oldX, int oldY)
    {
        history.Push(new MoveData()
        {
            car = car,
            oldX = oldX,
            oldY = oldY
        });
    }

    public bool CanUndo()
    {
        return history.Count > 0;
    }

    public void Undo()
    {
        if (history.Count == 0)
            return;

        MoveData move = history.Pop();

        GridManager.Instance.UnregisterCar(move.car);

        move.car.gridX = move.oldX;
        move.car.gridY = move.oldY;

        move.car.transform.position =
            GridManager.Instance.GridToWorld(
                move.oldX,
                move.oldY,
                move.car.isHorizontal,
                move.car.length);

        GridManager.Instance.RegisterCar(move.car);

        if (MovesManager.Instance != null)
            MovesManager.Instance.ReturnMove();
    }
}