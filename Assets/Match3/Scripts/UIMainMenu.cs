using TMPro;
using UnityEngine;

public class UIMainMenu : MonoBehaviour
{
    [Header("References")]
    public GameObject panel;
    public TMP_Text coinsText;
    public UIStageSelection stageSelectionWindow;

    private void Start()
    {
        UpdatePlayerDataUI();
    }

    private void UpdatePlayerDataUI()
    {
        coinsText.text = Player.Instance.UserData.coins.ToString();
    }

    public void OpenStageSelectionWindow()
    {
        if (!stageSelectionWindow.window.IsOpen)
            stageSelectionWindow.window.Open();
    }

    public void PlayStage(int stageIndex)
    {
        if (GameManager.Instance.State == GameState.MainMenu)
            GameManager.Instance.PlayStage(stageIndex);
    }
}
