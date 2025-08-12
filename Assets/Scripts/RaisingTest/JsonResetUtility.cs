using UnityEngine;
using System.IO;

public static class JsonResetUtility
{
    // GameManager의 키와 일치해야 함
    private const int DefaultMaxTurnFallback = 12;
    private const string TURN_KEY = "gm_current_turn";

    /// <summary>
    /// SceneData.json 삭제 (파일만)
    /// </summary>
    public static void ResetSceneDataJson()
    {
        var path = Path.Combine(Application.persistentDataPath, "SceneData.json");
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"[Reset] Deleted : {path}");
        }
        else
        {
            Debug.Log("[Reset] SceneData.json 없음");
        }
    }

    /// <summary>
    /// stat_data.json / haroSave.v1.json 삭제 + (실행 중이면) 상태 리셋
    /// </summary>
    public static void ResetStatsHpTurnsJsonAndState()
    {
        var statPath = Path.Combine(Application.persistentDataPath, "stat_data.json");
        var savePath = Path.Combine(Application.persistentDataPath, "haroSave.v1.json");

        if (File.Exists(statPath)) { File.Delete(statPath); Debug.Log($"[Reset] Deleted: {statPath}"); }
        else Debug.Log("[Reset] stat_data.json 없음");

        if (File.Exists(savePath)) { File.Delete(savePath); Debug.Log($"[Reset] Deleted: {savePath}"); }
        else Debug.Log("[Reset] haroSave.v1.json 없음");

        // 실행 중이면: 스탯/체력/턴 UI까지 즉시 반영
        if (StatManager.Instance != null)
        {
            // 사용자 프로젝트에 이미 있는 메서드 가정
            StatManager.Instance.ResetStatsAndSaveFullStamina();
            Debug.Log("[Reset] StatManager: 스탯 0 + 풀피 저장 완료");
        }
        else
        {
            Debug.LogWarning("[Reset] StatManager 인스턴스 없음(플레이 중이 아님). 다음 실행 때 기본값으로 시작");
        }

        if (GameManager.Instance != null)
        {
            // 사용자 프로젝트에 이미 있는 메서드 가정
            GameManager.Instance.ResetTurnsToMax(syncUI: true);
            Debug.Log($"[Reset] GameManager: 턴수 {GameManager.Instance.GetCurrentTurn()}로 리필");
        }
        else
        {
            // 플레이 중이 아니면 PlayerPrefs로 폴백 저장
            PlayerPrefs.SetInt(TURN_KEY, DefaultMaxTurnFallback);
            PlayerPrefs.Save();
            Debug.LogWarning($"[Reset] GameManager 인스턴스 없음 → PlayerPrefs로 턴 {DefaultMaxTurnFallback} 저장");
        }

        Debug.Log("[Reset] JSON Reset 완료");
    }
}
