
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlyStageGoToRaising : MonoBehaviour
{
    [Header("이동할 육성 스테이지 씬 이름")]
    [SerializeField] private string raisingStageSceneName = "Raising_Stage";

    // 소프트 리셋: 런타임 상태만 초기화(파일은 안 지움)
    // 스탯 0 + 풀 스태미나, 턴 최대 → 저장 반영
    public void OnClick_ResetStats_Soft()
    {
        var sm = StatManager.Instance;
        var gm = GameManager.Instance;

        if (sm != null) sm.ResetStatsAndSaveFullStamina();
        if (gm != null) gm.ResetTurnsToMax(syncUI: true);

        if (SaveManager.Instance != null)
            SaveManager.Instance.SaveGame();

        Debug.Log("[UI] Soft Reset 완료");
    }

    // 하드 리셋: 관련 json 파일까지 삭제 후 초기화
    // stat_data.json / haroSave.v1.json 삭제 + 런타임 초기화 → 새로 저장
    public void OnClick_ResetStats_Hard()
    {
        JsonResetUtility.ResetStatsHpTurnsJsonAndState();

        if (SaveManager.Instance != null)
            SaveManager.Instance.SaveGame();

        Debug.Log("[UI] Hard Reset 완료");
    }

    // 육성 스테이지로 이동
    // 이동 전에 현재 상태 저장(게임 + 씬데이터)
    public void OnClick_GoToRaisingStage()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame();
            SaveManager.Instance.SaveSceneDataJson();
        }

        if (string.IsNullOrWhiteSpace(raisingStageSceneName))
        {
            Debug.LogError("[UI] 육성 스테이지 씬 이름 비어있음");
            return;
        }

        SceneManager.LoadScene(raisingStageSceneName);
        Debug.Log($"[UI] 씬 이동: {raisingStageSceneName}");
    }
}
