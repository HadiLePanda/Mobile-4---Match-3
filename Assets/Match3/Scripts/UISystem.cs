using UnityEngine;

public class UISystem : MonoBehaviour
{
    [Header("References")]
    public GameObject pausePanel;

    public void PauseMenu()
    {
        pausePanel.SetActive(true);
    }

    public void ClosePauseMenu()
    {
        pausePanel.SetActive(false);
    }
}
