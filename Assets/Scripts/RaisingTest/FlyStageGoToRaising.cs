using UnityEngine;
using UnityEngine.SceneManagement;

public class FlyStageGoToRaising : MonoBehaviour
{
    [Header("이동할 육성 스테이지 씬 이름")]
    [SerializeField] private string raisingStageSceneName = "Raising_Stage";

    public void OnClick_RetryRaising()
    {
        SaveManager.Instance.ResetGame();

        Debug.Log("[MainMenuResetButton] 전체 데이터 리셋 완료 (스탯 0 / 체력 풀 / 턴 풀)");

        SceneManager.LoadScene("Raising_Stage");

    }
}
