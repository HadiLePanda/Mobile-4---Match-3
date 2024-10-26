using UnityEngine;

[CreateAssetMenu(fileName = "New Symbol", menuName = "Panda/Symbols/New Symbol")]
public class SymbolData: ScriptableObject
{
    [Header("Base Data")]
    public Sprite sprite;
    public int scoreValue = 10;
    public Color color = Color.white;
}