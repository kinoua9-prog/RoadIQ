using UnityEngine;

public class LevelSettings : MonoBehaviour
{
    public int threeStarMoves = 15;
    public int twoStarMoves = 25;
    [Header("Timer")]
    public float levelTime = 60f;
    [Header("Moves")]
    public int startMoves = 50;
}