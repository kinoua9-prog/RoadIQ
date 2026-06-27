using UnityEngine;

public class HintShopManager : MonoBehaviour
{
    public int price = 1;

    public void BuyAddTime()
    {
        if (!WalletManager.Instance.SpendCoins(price))
            return;

        LevelTimer.Instance.AddTime(5);
    }

    public void BuyAddMove()
    {
        if (!WalletManager.Instance.SpendCoins(price))
            return;

        MovesManager.Instance.AddMoves(1);
    }

    public void BuyUndo()
    {
        if (!WalletManager.Instance.SpendCoins(price))
            return;

        UndoManager.Instance.Undo();
    }
}