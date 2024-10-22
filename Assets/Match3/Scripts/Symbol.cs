using System.Collections;
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
    public void SetIsMatched(bool isMatched) => this.isMatched = isMatched;
    public void SetIsMoving(bool isMoving) => this.isMoving = isMoving;

    // MOVEMENT
    public void MoveToPosition(Vector2 targetPos)
    {
        // TODO: use LeanTween to easily animate movement
        StartCoroutine(MoveCoroutine(targetPos));
    }
    private IEnumerator MoveCoroutine(Vector2 targetPos)
    {
        isMoving = true;

        float animationDuration = Board.Instance.symbolSwapDuration;

        Vector2 startPos = transform.position;
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;
            transform.position = Vector2.Lerp(startPos, targetPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
    }
}