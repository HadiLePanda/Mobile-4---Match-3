using UnityEngine;

public class UIMainMenu : MonoBehaviour
{
    [Header("References")]
    public GameObject panel;

    public void PlayStage(int stageIndex)
    {
        if (GameManager.Instance.State == GameState.MainMenu)
            GameManager.Instance.PlayStage(stageIndex);
    }
}
