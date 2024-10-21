using UnityEngine;

public class Symbol : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private SymbolType type;
    
    private int x;
    private int y;
    private bool isMatched;
    private bool isMoving;
    
    public int X => x;
    public int Y => y;
    public bool IsMatched => isMatched;
    public bool IsMoving => isMoving;
    public SymbolType Type => type;
    
    private Vector2 currentPos;
    private Vector2 targetPos;

    // TODO: Initialize by updating icon etc..

    public void SetIndices(Vector2Int indices)
    {
        x = indices.x;
        y = indices.y;
    }
    public void SetMatchedState(bool isMatched) => this.isMatched = isMatched;
    public void SetMovingState(bool isMoving) => this.isMoving = isMoving;

    // TODO: MoveToTarget
    // TODO: MoveCoroutine
}