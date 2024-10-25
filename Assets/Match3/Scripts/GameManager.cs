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
    
    [SerializeField, ReadOnly] private int currentStageIndex;

    public GameState State => state;
    public int CurrentStageIndex => currentStageIndex;
    public int Score => SessionManager.Instance.Score;
    public int MovesRemaining => SessionManager.Instance.MovesRemaining;
    
    public void SetState(GameState newState) => state = newState;

    public static Action onStartStage, onStageWin, onGameOver;
    public static Action onStageWinSequenceFinished, onGameOverSequenceFinished;
    public static Action onStageLoaded;

    protected override bool IsDontDestroyOnLoad() => true;

    protected override void Init()
    {
        // load data
        UserData userData = SaveManager.Instance.LoadUserData();
        Player.Instance.LoadUserData(userData);

        // load main menu scene
        GoToMainMenu();
    }

    #region GAME LOGIC
    public void ProcessTurn(bool consumeMove = true)
    {
        if (state != GameState.Playing)
            return;

        // consume a move for playing this turn
        if (consumeMove &&
            SessionManager.Instance.MovesRemaining > 0)
        {
            ConsumeMove();
        }

        // we reached the stage win score, stage won
        if (Score >= GetCurrentStage().requiredScoreToWin)
        {
            StageWin();
        }
        // we don't have any moves left, game over
        else if (MovesRemaining == 0)
        {
            GameOver();
        }
    }

    public void ConsumeMove()
    {
        SessionManager.Instance.ConsumeMove();
    }
    #endregion

    #region GAME STATE
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

    public void InitializeMainMenu()
    {
        state = GameState.MainMenu;

        // play menu music
        MusicManager.Instance.PlayMusic(MusicManager.Instance.menuMusic, mainMenuMusicVolume, musicFadeTime);
    }

    public void GameOver()
    {
        state = GameState.GameOver;

        // trigger game over sequence
        StartCoroutine(GameOverSequence());
    }

    private int GetUnusedMoves() => Mathf.Max(0, GetCurrentStage().maxMoves - MovesRemaining);

    private void StageWin()
    {
        // get the amount of unused moves
        int unusedMoves = GetUnusedMoves();

        // give bonus points for winning with extra moves remaining
        int extraMovesBonusScore = unusedMoves * extraMoveScoreValue;
        SessionManager.Instance.AddScore(extraMovesBonusScore);

        state = GameState.Win;

        // trigger win sequence
        StartCoroutine(StageWinSequence());
    }

    private IEnumerator GameOverSequence()
    {
        // TODO:play game over effects

        // play game over sound
        AudioManager.Instance.PlaySound2DOneShot(gameOverSound);

        // lower music volume
        MusicManager.Instance.SetMusicVolume(stageOverMusicVolume);

        // stop ambient
        MusicManager.Instance.StopAmbient();

        onGameOver?.Invoke();

        yield return new WaitForSeconds(gameOverSequenceTime);

        // trigger event to show game over popup
        onGameOverSequenceFinished?.Invoke();
    }

    private IEnumerator StageWinSequence()
    {
        // update coins earned amount
        SessionManager.Instance.OnStageWin();

        // give to the player the coins earned in this session
        Player player = Player.Instance;
        int sessionCoinsEarned = SessionManager.Instance.SessionCoins;
        player.AddCoins(sessionCoinsEarned);

        // save the player coins
        SaveManager.Instance.SaveCoins(player.UserData.coins);
        PlayerPrefs.Save();

        // TODO: play game win effects

        // play win sound
        AudioManager.Instance.PlaySound2DOneShot(winSound, pitchVariation: 0.05f);

        // lower music volume
        MusicManager.Instance.SetMusicVolume(stageOverMusicVolume);

        onStageWin?.Invoke();

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
        // note: session values are initialized when scene loads
        // note: board is initialized when scene loads

        // TODO: spawn stage background

        onStageLoaded?.Invoke();
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
