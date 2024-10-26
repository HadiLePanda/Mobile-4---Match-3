using UnityEngine;

public abstract class ConsumableSymbol : SymbolData
{
    [Header("Consumable Settings")]
    public AudioClip consumeSound;

    public abstract void Consume(Board board, Symbol symbol);

    public virtual void PlayConsumeSound()
    {
        AudioManager.Instance.PlaySound2DOneShot(consumeSound);
    }
}
