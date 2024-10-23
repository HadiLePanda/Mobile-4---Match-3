using UnityEngine;

[CreateAssetMenu(fileName = "New Stage", menuName = "Panda/Stages/New Stage")]
public class StageData : ScriptableObject
{
    [Header("References")]
    public GameObject backgroundPrefab;
    public SymbolData[] stageSymbols;

    [Header("Settings")]
    public int boardWidth = 6;
    public int boardHeight = 8;
    public int requiredScoreToWin = 100;
    public int maxMoves = 10;
}
