using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    Loading,
    Playing,
    Win,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static readonly string SCENE_MAINMENU = "MainMenu";
    public static readonly string SCENE_GAME = "Game";

    [Header("References")]
    [SerializeField] private StageData[] stages;

    [Header("Debug")]
    [SerializeField, ReadOnly] private GameState state;
    [SerializeField, ReadOnly] private int score;
    [SerializeField, ReadOnly] private int movesRemaining;
    [SerializeField, ReadOnly] private int scoreToWin;
    [SerializeField, ReadOnly] private int currentStageIndex;

    public GameState State => state;
    public int Score => score;
    public int MovesRemaining => movesRemaining;
    public void SetScore(int newScore) => score = newScore;

    public Action onStartStage, onStageWin, onGameOver;
    public Action onScoreChanged;
    public Action onStageWinSequenceFinished, onGameOverSequenceFinished;
    public Action onStageLoaded;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        state = GameState.MainMenu;

        // TODO remove after adding proper stage selection
        yield return new WaitForSeconds(0.5f);
        PlayStage(0);
    }

    #region GAME LOGIC
    public void ProcessTurn(int scoreGained, bool consumeMove)
    {
        if (state != GameState.Playing)
            return;

        // increase score
        AddScore(scoreGained);

        // consume a move this turn cost one
        if (consumeMove &&
            movesRemaining > 0)
        {
            ConsumeMove();
        }

        // we reached the stage win score, stage won
        if (score >= scoreToWin)
        {
            StageWin();
        }
        // we don't have any moves left, game over
        else if (movesRemaining == 0)
        {
            GameOver();
        }
    }

    private void ConsumeMove()
    {
        movesRemaining = Mathf.Max(0, movesRemaining - 1);
    }
    #endregion

    #region GAME STATE
    public void GoToMainMenu()
    {
        state = GameState.Loading;

        SceneManager.LoadScene(SCENE_MAINMENU);

        state = GameState.MainMenu;
    }

    public void PlayStage(int stageIndex)
    {
        // TODO: add transition screen while loading scene

        state = GameState.Loading;
        Debug.Log("Loading Stage " + stageIndex);

        // load the game scene
        //SceneManager.LoadScene(SCENE_GAME);

        // load the stage
        LoadStage(stageIndex);

        // generate board
        Board.Instance.CreateBoardWithNoMatches();

        state = GameState.Playing;
        onStartStage?.Invoke();
        Debug.Log("Started Stage " + stageIndex);
    }

    public void GameOver()
    {
        state = GameState.GameOver;
        onGameOver?.Invoke();

        // trigger game over sequence
        StartCoroutine(GameOverSequence());
    }

    private void StageWin()
    {
        state = GameState.Win;
        onStageWin?.Invoke();

        // trigger win sequence
        StartCoroutine(StageWinSequence());
    }

    private IEnumerator GameOverSequence()
    {
        Debug.Log("Game over!");
        // TODO: play game over effects

        yield return null;

        // TODO: show game over panel
        onGameOverSequenceFinished?.Invoke();
    }

    private IEnumerator StageWinSequence()
    {
        Debug.Log("Stage won!");
        // TODO: play game win effects

        yield return null;

        // TODO: show stage win panel
        onStageWinSequenceFinished?.Invoke();
    }
    #endregion

    #region STAGES
    public StageData GetCurrentStage() => stages[currentStageIndex];

    public void LoadStage(int stageIndex)
    {
        if (stageIndex > stages.Length - 1)
            return;

        currentStageIndex = stageIndex;
        var stage = stages[stageIndex];
        InitializeStage(stage);

        onStageLoaded?.Invoke();
    }
    public void InitializeStage(StageData stage)
    {
        // initialize stage data
        score = 0;
        movesRemaining = stage.maxMoves;
        scoreToWin = stage.requiredScoreToWin;

        // TODO: spawn stage background
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
    #endregion

    #region SCORE
    public void AddScore(int amount)
    {
        score = Mathf.Max(0, score + amount);

        // TODO: play score increase effects
        // TODO: different sounds based on amount of score

        onScoreChanged?.Invoke();
    }
    #endregion

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
