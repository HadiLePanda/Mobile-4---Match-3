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

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public static readonly string SCENE_MAINMENU = "MainMenu";
    public static readonly string SCENE_GAME = "Game";

    [Header("References")]
    [SerializeField] private StageData[] stages;

    [Header("Settings")]
    [SerializeField] private float gameOverSequenceTime = 1f;
    [SerializeField] private float gameWinSequenceTime = 1f;
    [SerializeField] private float mainMenuMusicVolume = 0.7f;
    [SerializeField] private float stageOverMusicVolume = 0.4f;
    [SerializeField] private float musicFadeTime = 1f;
    [SerializeField] private int extraMoveScoreValue = 100;

    [Header("Sounds")]
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip gameOverSound;

    [Header("Debug")]
    [SerializeField, ReadOnly] private GameState state;
    [SerializeField, ReadOnly] private int score;
    [SerializeField, ReadOnly] private int movesRemaining;
    [SerializeField, ReadOnly] private int scoreToWin;
    [SerializeField, ReadOnly] private int currentStageIndex;

    public GameState State => state;
    public int Score => score;
    public int ScoreToWin => stages[currentStageIndex].requiredScoreToWin;
    public int MaxMoves => stages[currentStageIndex].maxMoves;
    public int MovesRemaining => movesRemaining;
    public int CurrentStageIndex => currentStageIndex;
    public float ScoreProgressPercent
    {
        get
        {
            if (scoreToWin <= 0)
                return 0f;

            return Mathf.Clamp01((float)score / (float)scoreToWin);
        }
    }

    public void SetScore(int newScore) => score = newScore;
    public void SetState(GameState newState) => state = newState;

    public static Action onStartStage, onStageWin, onGameOver;
    public static Action onScoreChanged;
    public static Action onStageWinSequenceFinished, onGameOverSequenceFinished;
    public static Action onStageLoaded;

    protected override bool IsDontDestroyOnLoad() => true;

    protected override void Init()
    {
        GoToMainMenu();

        // TODO remove after adding proper stage selection
        //yield return new WaitForSeconds(0.5f);
        //PlayStage(0);
    }

    #region GAME LOGIC
    public void ProcessTurn(bool consumeMove = true)
    {
        if (state != GameState.Playing)
            return;

        // consume a move for playing this turn
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

    public void ConsumeMove()
    {
        movesRemaining = Mathf.Max(0, movesRemaining - 1);
    }
    #endregion

    #region GAME STATE
    public void InitializeMainMenu()
    {
        state = GameState.MainMenu;

        // play menu music
        MusicManager.Instance.PlayMusic(MusicManager.Instance.menuMusic, mainMenuMusicVolume, musicFadeTime);
    }

    public void GoToMainMenu()
    {
        state = GameState.Loading;

        // stop music & ambient
        MusicManager.Instance.StopMusic();
        MusicManager.Instance.StopAmbient();

        // load main menu scene
        SceneManager.LoadScene(SCENE_MAINMENU);

        InitializeMainMenu();
    }

    public void GameOver()
    {
        state = GameState.GameOver;
        onGameOver?.Invoke();

        // trigger game over sequence
        StartCoroutine(GameOverSequence());
    }

    private int GetUnusedMoves() => Mathf.Max(0, GetCurrentStage().maxMoves - movesRemaining);
    private void StageWin()
    {
        // get the amount of unused moves
        int unusedMoves = GetUnusedMoves();

        // give bonus points for winning with extra moves remaining
        int extraMovesBonusScore = unusedMoves * extraMoveScoreValue;
        AddScore(extraMovesBonusScore);

        state = GameState.Win;
        onStageWin?.Invoke();

        // trigger win sequence
        StartCoroutine(StageWinSequence());
    }

    private IEnumerator GameOverSequence()
    {
        Debug.Log("Game over!");

        // TODO:play game over effects

        // play game over sound
        AudioManager.Instance.PlaySound2DOneShot(gameOverSound);

        // lower music volume
        MusicManager.Instance.SetMusicVolume(stageOverMusicVolume);

        // stop ambient
        MusicManager.Instance.StopAmbient();

        yield return new WaitForSeconds(gameOverSequenceTime);

        // trigger event to show game over popup
        onGameOverSequenceFinished?.Invoke();
    }

    private IEnumerator StageWinSequence()
    {
        Debug.Log("Stage won!");

        // TODO: play game win effects

        // TODO: reward with coins

        // play win sound
        AudioManager.Instance.PlaySound2DOneShot(winSound, pitchVariation: 0.05f);

        // lower music volume
        MusicManager.Instance.SetMusicVolume(stageOverMusicVolume);

        yield return new WaitForSeconds(gameWinSequenceTime);

        // trigger event to show stage win popup
        onStageWinSequenceFinished?.Invoke();
    }
    #endregion

    #region STAGES
    public StageData GetCurrentStage() => stages[currentStageIndex];

    public void ReplayStage()
    {
        // TODO: do extra logic before restarting the level and stage
        PlayStage(currentStageIndex);
    }
    public void PlayFirstStage()
    {
        PlayStage(0);
    }
    public void PlayNextStage()
    {
        PlayStage(currentStageIndex + 1);
    }

    public void PlayStage(int stageIndex)
    {
        // TODO: add transition screen while loading scene

        // enter loading state
        state = GameState.Loading;

        // load the game scene
        SceneManager.LoadScene(SCENE_GAME);

        // load the stage
        LoadStage(stageIndex);

        // play gameplay music
        MusicManager.Instance.PlayMusic(MusicManager.Instance.gameMusic, fadeDuration: musicFadeTime);
        // play ambient
        MusicManager.Instance.PlayAmbient(MusicManager.Instance.gameAmbient);

        // enter playing state
        state = GameState.Playing;
        onStartStage?.Invoke();
    }

    private void LoadStage(int stageIndex)
    {
        if (stageIndex > stages.Length - 1)
            return;

        // update stage reference
        currentStageIndex = stageIndex;
        var stage = stages[stageIndex];

        // initialize stage data
        score = 0;
        movesRemaining = stage.maxMoves;
        scoreToWin = stage.requiredScoreToWin;

        // TODO: spawn stage background

        // note: board is loaded when starting the stage scene

        onStageLoaded?.Invoke();
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
