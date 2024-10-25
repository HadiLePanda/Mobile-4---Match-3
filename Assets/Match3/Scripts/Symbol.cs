using System;
using UnityEngine;

public class Symbol : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Settings")]
    [SerializeField] private SymbolData data;
    [SerializeField] private float moveSpeed = 10f;

    //[Header("Animation")]
    //[SerializeField] private LeanTweenType movementAnimationType;

    [Header("Debug")]
    [SerializeField, ReadOnly] private int x;
    [SerializeField, ReadOnly] private int y;
    [SerializeField, ReadOnly] private bool isMatched;
    [SerializeField, ReadOnly] private bool isMoving;
    private int moveTweenId;

    public int X => x;
    public int Y => y;
    public bool IsMatched => isMatched;
    public bool IsMoving => isMoving;
    public SymbolData Data => data;
    public Vector2Int GetIndices() => new(x, y);
    
    private Vector2 currentPos;
    private Vector2 targetPos;

    public static Action onSymbolChanged;

    public void SetIndices(Vector2Int indices)
    {
        x = indices.x;
        y = indices.y;
    }
    public void SetIsMatched(bool isMatched) => this.isMatched = isMatched;
    public void SetIsMoving(bool isMoving) => this.isMoving = isMoving;
    public void SetData(SymbolData symbolData)
    {
        data = symbolData;
        spriteRenderer.sprite = symbolData.sprite;
    }

    // MOVEMENT
    public void MoveToPosition(Vector3 targetPos)
    {
        Vector3 moveDir = (targetPos - transform.position);
        transform.position += moveSpeed * Time.deltaTime * moveDir;

        //PlayMovementAnimation(targetPos); // we don't use animations for now to make it consistent
    }

    private void Update()
    {
        Vector3 targetPosition = Board.Instance.GetTilePosition(GetIndices());

        if (targetPosition != transform.position)
        {
            isMoving = true;
            MoveToPosition(targetPosition);
        }
        else
        {
            isMoving = false;
        }
    }

    // we don't use animations for now
    //private void PlayMovementAnimation(Vector2 targetPos)
    //{
    //    //StartCoroutine(MoveCoroutine(targetPos));
    //
    //    // cancel any ongoing movement animation
    //    LeanTween.cancel(moveTweenId);
    //
    //    // do the movement animation
    //    moveTweenId = LeanTween.move(gameObject, targetPos, Board.Instance.symbolSwapDuration)
    //        .setEase(movementAnimationType)
    //        .setOnComplete(() => FinishedMovementAnimation()).id;
    //}
    //private void FinishedMovementAnimation()
    //{
    //    isMoving = false;
    //}

    //private IEnumerator MoveCoroutine(Vector2 targetPos)
    //{
    //    isMoving = true;
    //
    //    float animationDuration = Board.Instance.symbolSwapDuration;
    //
    //    Vector2 startPos = transform.position;
    //    float elapsedTime = 0f;
    //    while (elapsedTime < animationDuration)
    //    {
    //        float t = elapsedTime / animationDuration;
    //        transform.position = Vector2.Lerp(startPos, targetPos, t);
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }
    //
    //    transform.position = targetPos;
    //    isMoving = false;
    //}
}