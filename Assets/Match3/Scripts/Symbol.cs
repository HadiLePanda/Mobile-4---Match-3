using UnityEngine;

public class Symbol : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private SymbolData data;
    
    private int x;
    private int y;
    
    public bool isMatched;
    public bool isMoving;
    
    public int X => x;
    public int Y => y;
    public bool IsMatched => isMatched;
    public bool IsMoving => isMoving;
    public SymbolData Data => data;
    
    private Vector2 currentPos;
    private Vector2 targetPos;

    public Symbol(int x, int y)
    {
        SetIndices(x, y);
    }

    // TODO: Initialize by updating icon etc..

    public void SetIndices(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}