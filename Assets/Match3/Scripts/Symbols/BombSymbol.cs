using UnityEngine;

[CreateAssetMenu(fileName = "New Bomb", menuName = "Panda/Symbols/New Bomb")]
public class BombSymbol : ConsumableSymbol
{
    [Header("Bomb Settings")]
    public int explosionRadius = 2;
    public AudioClip explosionSound;

    public override void Consume(Board board, Symbol symbol)
    {
        Vector2Int bombPosition = symbol.GetIndices();
        Board.Instance.ActivateBomb(bombPosition, explosionRadius);

        // play sound
        PlayConsumeSound();
    }

    public override void PlayConsumeSound()
    {
        base.PlayConsumeSound();

        // play explosion sound
        AudioManager.Instance.PlaySound2DOneShot(explosionSound, pitchVariation: 0.1f);
    }
}
