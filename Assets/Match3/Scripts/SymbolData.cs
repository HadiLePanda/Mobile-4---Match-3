using UnityEngine;

public enum SymbolColor
{
    White,
    Green,
    Blue,
    Purple,
    Yellow,
    Orange,
    Red,
}

[CreateAssetMenu(fileName = "New Symbol", menuName = "Panda/Symbols/Create Symbol")]
public class SymbolData : ScriptableObject
{
    public Sprite icon;
    public SymbolColor color;
}