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

            Debug.Log("Game Over");

            if (LoseManager.Instance != null)
            {
                LoseManager.Instance.ShowLosePanel();
            }
        }
    }

    private void UpdateText()
    {
        if (movesText != null)
        {
            movesText.text = "Moves: " + movesLeft;
        }
    }
}