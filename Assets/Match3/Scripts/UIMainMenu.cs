using TMPro;
using UnityEngine;

public class UIMainMenu : MonoBehaviour
{
    [Header("References")]
    public GameObject panel;
    public TMP_Text coinsText;

    private void Start()
    {
        UpdatePlayerDataUI();
    }

    private void UpdatePlayerDataUI()
    {
        coinsText.text = Player.Instance.UserData.coins.ToString();
    }

    public void PlayStage(int stageIndex)
    {
        if (GameManager.Instance.State == GameState.MainMenu)
            GameManager.Instance.PlayStage(stageIndex);
    }
}
