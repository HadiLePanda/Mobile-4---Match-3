using System;
using System.Collections;
using UnityEngine;

public enum GameState
{
    MainMenu,
    Playing,
    Win,
    GameOver
}

public class GameManager : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField, ReadOnly] private GameState state;
    [SerializeField, ReadOnly] private int score;
    [SerializeField, ReadOnly] private int currentStageIndex;
    [SerializeField, ReadOnly] private int movesRemaining;

    public GameState State => state;
    public int Score => score;
    public int MovesRemaining => movesRemaining;

    public Action onStartGame, onStageWin, onGameOver;
    public Action onScoreChanged;
    public Action onStageLoaded;
    public Action onStageWinSequenceFinished, onGameOverSequenceFinished;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        state = GameState.MainMenu;
    }

    // GAME STATE
    public void GoToMainMenu()
    {
        state = GameState.MainMenu;
    }

    public void StartGame()
    {
        state = GameState.Playing;
        Debug.Log("Starting game.");
        onStartGame?.Invoke();

        Board.Instance.CreateBoardWithNoMatches();
    }

    public void GameOver()
    {
        state = GameState.GameOver;
        onGameOver?.Invoke();

        // trigger game over sequence
        StartCoroutine(GameOverSequence());
    }

    public void StageWon()
    {
        state = GameState.Win;
        onStageWin?.Invoke();

        // trigger win sequence
        StartCoroutine(StageWinSequence());
    }

    private IEnumerator StageWinSequence()
    {
        Debug.Log("Stage won!");
        yield return null;

        // TODO: show stage win panel
        onStageWinSequenceFinished?.Invoke();
    }

    private IEnumerator GameOverSequence()
    {
        Debug.Log("Game over!");
        yield return null;

        // TODO: show game over panel
        onGameOverSequenceFinished?.Invoke();
    }

    // SCORE
    public void AddScore(int amount)
    {
        score = Mathf.Max(0, score + amount);
        onScoreChanged?.Invoke();
    }

    // STAGES
    public void LoadStage(int stageIndex)
    {
        // TODO: do loading
        currentStageIndex = stageIndex;
        onStageLoaded?.Invoke();
    }
    public void LoadFirstStage()
    {
        LoadStage(0);
    }
    public void LoadNextStage()
    {
        LoadStage(currentStageIndex + 1);
    }
    public void RestartStage()
    {
        LoadStage(currentStageIndex);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
