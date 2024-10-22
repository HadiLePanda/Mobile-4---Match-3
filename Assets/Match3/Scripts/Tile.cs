public class Tile
{
    public bool isUsable;
    public Symbol symbol;

    public Tile(bool isUsable, Symbol symbol)
    {
        this.isUsable = isUsable;
        this.symbol = symbol;
    }
}
