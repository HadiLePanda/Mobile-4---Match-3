using UnityEngine;

public class Tile
{
    private bool isUsable;
    private Symbol symbol;

    public bool IsUsable => isUsable;
    public Symbol Symbol => symbol;

    public Tile(bool isUsable, Symbol symbol)
    {
        this.isUsable = isUsable;
        this.symbol = symbol;
    }
}
