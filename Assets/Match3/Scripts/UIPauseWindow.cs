using UnityEngine;

public class UIPauseWindow : MonoBehaviour
{
    [Header("References")]
    public UIWindow window;

    public void Pause()
    {
        window.Open();
    }

    public void Resume()
    {
        window.Close();
    
    }

    public void Replay()
    {
        if (GameManager.Instance.State == GameState.Loading)
            return;

        GameManager.Instance.ReplayStage();
    
    }

    public void GoToMainMenu()
    {
        GameManager.Instance.GoToMainMenu();
    }
}
