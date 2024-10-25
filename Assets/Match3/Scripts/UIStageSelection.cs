using UnityEngine;

public class UIStageSelection : MonoBehaviour
{
    [Header("References")]
    public UIWindow window;
    public StageSelectButton stageSelectButtonPrefab;
    public Transform stageButtonsParent;

    private void Start()
    {
        // clear the list of buttons
        foreach (Transform child in stageButtonsParent)
            Destroy(child.gameObject);

        PopulateStageSelectionButtons();
    }

    private void PopulateStageSelectionButtons()
    {
        // get the list of stages
        StageData[] stages = GameManager.Instance.Stages;

        // insert stage selection buttons
        for (int i = 0; i < stages.Length; i++)
        {
            // spawn a stage select button
            StageSelectButton stageSelectButtonInstance = Instantiate(stageSelectButtonPrefab, stageButtonsParent.transform);

            // make the button point to its respective stage
            stageSelectButtonInstance.SetStageIndex(i);
        }
    }
}
