using UnityEngine;

[CreateAssetMenu(fileName = "New Symbol Type", menuName = "Panda/Symbols/Create Symbol Type")]
public class SymbolType: ScriptableObject
{
    public float PointMultiplier = 1f;
    public Color Color = Color.white;
}