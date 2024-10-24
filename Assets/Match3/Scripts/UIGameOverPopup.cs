using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameOverPopup : MonoBehaviour
{
    [Header("References")]
    public UIWindow window;
    public TMP_Text scoreText;
    public Button replayButton;
    public Button homeButton;

    private void OnEnable()
    {
        GameManager.onGameOver += OnGameOver;
    }
    private void OnDisable()
    {
        GameManager.onGameOver -= OnGameOver;
    }

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        // automatically close the window if not in the proper state
        if (window.IsOpen &&
            GameManager.Instance.State != GameState.GameOver)
        {
            window.Close();
        }
    }

    private void OnGameOver()
    {
        UpdateUI();
        window.Open();

        // TODO: play animation sequences
    }

    private void UpdateUI()
    {
        scoreText.text = GameManager.Instance.Score.ToString();
    }

    public void Replay()
    {
        if (GameManager.Instance.State != GameState.GameOver)
            return;

        GameManager.Instance.ReplayStage();
    }

    public void ReturnToMainMenu()
    {
        if (GameManager.Instance.State != GameState.GameOver)
            return;

        GameManager.Instance.GoToMainMenu();
    }
}
