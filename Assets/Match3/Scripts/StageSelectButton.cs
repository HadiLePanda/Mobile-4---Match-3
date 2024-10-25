using TMPro;
using UnityEngine;

public class StageSelectButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text stageNumberText;

    [Header("Debug")]
    [SerializeField, ReadOnly] private int stageIndex;

    public void PlayStage()
    {
        if (GameManager.Instance.State == GameState.Loading)
            return;

        if (!GameManager.Instance.IsValidStageIndex(stageIndex))
        {
            Debug.LogError($"Tried to load an invalid stage index through a stage selection button ({stageIndex})");
            return;
        }

        GameManager.Instance.PlayStage(stageIndex);
    }

    public void SetStageIndex(int stageIndex)
    {
        this.stageIndex = stageIndex;
        UpdateStageNumberUI();
    }

    public void UpdateStageNumberUI()
    {
        stageNumberText.text = (stageIndex + 1).ToString();
    }
}
