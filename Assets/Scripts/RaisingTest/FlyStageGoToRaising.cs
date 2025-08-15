using UnityEngine;
using UnityEngine.SceneManagement;

public class FlyStageGoToRaising : MonoBehaviour
{
    [Header("이동할 육성 스테이지 씬 이름")]
    [SerializeField] private string raisingStageSceneName = "Raising_Stage";

    public void OnClick_RetryRaising()
    {
        SaveManager.Instance.ResetGame();
        SceneManager.LoadScene("Raising_Stage");


        // 씬 로드
        if (!string.IsNullOrWhiteSpace(raisingStageSceneName))
            SceneManager.LoadScene(raisingStageSceneName);
    }
}
