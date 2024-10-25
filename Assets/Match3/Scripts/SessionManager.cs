using System;
using UnityEngine;

public class SessionManager : SingletonMonoBehaviour<SessionManager>
{
    // handle data that is processed during a session
    // like score, time, remainingMoves, maxMoves ...

    [Header("Settings")]
    [SerializeField] private int coinsPerMoveRemaining = 100;
    [SerializeField] private int maxBonusRemainingMoves = 3;

    [Header("Debug")]
    [SerializeField, ReadOnly] private int score;
    [SerializeField, ReadOnly] private int sessionCoins;
    [SerializeField, ReadOnly] private int movesRemaining;

    public int Score => score;
    public int SessionCoins => sessionCoins;
    public int MovesRemaining => movesRemaining;

    public int ScoreToWin => GameManager.Instance.GetCurrentStage().requiredScoreToWin;
    public int MaxMoves => GameManager.Instance.GetCurrentStage().maxMoves;
    public int ExtraMoves => GameManager.Instance.GetCurrentStage().maxMoves;

    public float ScoreProgressPercent
    {
        get
        {
            if (ScoreToWin <= 0)
                return 0f;

            return Mathf.Clamp01((float)Score / (float)ScoreToWin);
        }
    }

    public static Action onScoreChanged;

    protected override void Init() => InitializeSession();

    public void InitializeSession()
    {
        score = 0;
        sessionCoins = 0;
        movesRemaining = GameManager.Instance.GetCurrentStage().maxMoves;
    }

    public void OnStageWin()
    {
        // calculate coins earned for this session
        int movesRemainingMultiplier = Mathf.Clamp(movesRemaining, 0, maxBonusRemainingMoves);
        int extraMovesCoinsReward = coinsPerMoveRemaining * movesRemainingMultiplier;

        // update the session earned coins
        sessionCoins = extraMovesCoinsReward;
    }

    private void SetMovesRemaining(in int newMovesRemaining) => movesRemaining = newMovesRemaining;
    private void SetCoinsEarned(int newCoinsEarned) => sessionCoins = newCoinsEarned;

    #region SCORE
    public void SetScore(int newScore) => score = newScore;
    public void AddScore(int amount)
    {
        score = Mathf.Max(0, score + amount);
        onScoreChanged?.Invoke();

        // TODO: play score increase effects
        // TODO: different sounds based on amount of score
    }

    public void ConsumeMove()
    {
        movesRemaining = Mathf.Max(0, movesRemaining - 1);
    }
    #endregion
}
