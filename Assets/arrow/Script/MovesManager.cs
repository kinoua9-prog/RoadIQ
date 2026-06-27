using TMPro;
using UnityEngine;

public class MovesManager : MonoBehaviour
{
    public static MovesManager Instance;

    public int startMoves = 50;
    public int movesLeft;

    public int usedMoves = 0;

    public TMP_Text movesText;

    public bool gameOver = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        movesLeft = startMoves;
        usedMoves = 0;
        gameOver = false;

        UpdateText();
    }

    public void UseMove()
    {
        if (gameOver)
            return;

        movesLeft--;
        usedMoves++;

        UpdateText();

        if (movesLeft <= 0)
        {
            movesLeft = 0;
            gameOver = true;

            UpdateText();

            Debug.Log("Game Over");

            if (LoseManager.Instance != null)
            {
                LoseManager.Instance.ShowLosePanel();
            }
        }
    }

    // Купівля додаткових ходів
    public void AddMoves(int amount)
    {
        if (amount <= 0)
            return;

        movesLeft += amount;

        UpdateText();
    }

    // Повернення одного ходу (UNDO)
    public void ReturnMove()
    {
        movesLeft++;
        usedMoves--;

        if (usedMoves < 0)
            usedMoves = 0;

        gameOver = false;

        UpdateText();
    }

    private void UpdateText()
    {
        if (movesText != null)
        {
            movesText.text = movesLeft.ToString();
        }
    }
}