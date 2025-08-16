using UnityEngine;
using UnityEngine.UI;

public class FlyStageGoToRaising: MonoBehaviour
{
 

    [Header("리셋 버튼")]
    [SerializeField] private Button resetButton;


    private void Awake()
    {
        if (resetButton != null)
        {
            resetButton.onClick.RemoveListener(OnClick_RetryRaising);
            resetButton.onClick.AddListener(OnClick_RetryRaising);
        }
    }


    public void OnClick_RetryRaising()
    {
        SetInteractable(false);

        if (SaveManager.Instance != null)
        {
            // 전체 데이터 리셋 + 다음 육성 첫 입장 
            SaveManager.Instance.ResetGame();
            Debug.Log("[MainMenuResetButton] 전체 데이터 리셋 완료 (스탯 0 / 체력 풀 / 턴 풀)");
        }
        else
        {
            Debug.LogWarning("[MainMenuResetButton] SaveManager.Instance가 없습니다.");
        }
        SceneSettingManager.Instance.ChangeScene("Raising_Stage");

        SetInteractable(true);
    }

    private void SetInteractable(bool v)
    {
        if (resetButton) resetButton.interactable = v;
    }
}
