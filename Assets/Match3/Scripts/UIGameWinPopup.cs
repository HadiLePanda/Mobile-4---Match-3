using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameWinPopup : MonoBehaviour
{
    [Header("References")]
    public UIWindow window;
    public TMP_Text scoreText;
    public Button replayButton;
    public Button homeButton;
    // TODO: stars

    private void OnEnable()
    {
        GameManager.onStageWin += OnStageWin;
    }
    private void OnDisable()
    {
        GameManager.onStageWin -= OnStageWin;
    }

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        // automatically close the window if not in the proper state
        if (window.IsOpen &&
            GameManager.Instance.State != GameState.Win)
        {
            window.Close();
        }
    }

    private void OnStageWin()
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
        if (GameManager.Instance.State != GameState.Win)
            return;

        GameManager.Instance.ReplayStage();
    }

    public void GoToMainMenu()
    {
        if (GameManager.Instance.State != GameState.Win)
            return;

        GameManager.Instance.GoToMainMenu();
    }
}
