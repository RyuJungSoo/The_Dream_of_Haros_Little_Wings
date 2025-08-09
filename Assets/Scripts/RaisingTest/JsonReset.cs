#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public static class DevResetSceneData
{
    [MenuItem("Tools/Save/JSON Reset (SceneData)")]
    private static void ResetAll()
    {
        var SceneDataPath = Path.Combine(Application.persistentDataPath, "SceneData.json");
        if (File.Exists(SceneDataPath))
        {
            File.Delete(SceneDataPath);
            Debug.Log($"[Reset] Deleted : {SceneDataPath}");
        }
        else
        {
            Debug.Log("[Reset] SceneData.json 없음");
        }
    }
}

public static class DevResetMenu
{
    // 프로젝트 기본 턴수(게임 매니저 인스턴스 없을 때 폴백)
    private const int DefaultMaxTurnFallback = 12;
    private const string TURN_KEY = "gm_current_turn";

    [MenuItem("Tools/Save/JSON Reset (Stats + HP + Turns)")]
    private static void ResetAll()
    {
        // 파일 삭제: 스탯 JSON + 통합 세이브 JSON
        var statPath = Path.Combine(Application.persistentDataPath, "stat_data.json");
        var savePath = Path.Combine(Application.persistentDataPath, "haroSave.v1.json");

        if (File.Exists(statPath)) { File.Delete(statPath); Debug.Log($"[Reset] Deleted: {statPath}"); }
        else Debug.Log("[Reset] stat_data.json 없음");

        if (File.Exists(savePath)) { File.Delete(savePath); Debug.Log($"[Reset] Deleted: {savePath}"); }
        else Debug.Log("[Reset] haroSave.v1.json 없음");

        // 실행 중이면: 스탯 0 초기화 + 체력 풀로 저장
        if (StatManager.Instance != null)
        {
            // 스탯/체력 리셋 + JSON 재기록 + UI 갱신
            StatManager.Instance.ResetStatsAndSaveFullStamina();
            Debug.Log("[Reset] StatManager: 스탯 0 + 풀피 저장 완료");
        }
        else
        {
            Debug.LogWarning("[Reset] StatManager 인스턴스 없음(플레이 중이 아님). 다음 실행 때 기본값으로 시작");
        }

        // 턴수 초기화
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetTurnsToMax(syncUI: true); // UI까지 반영
            Debug.Log($"[Reset] GameManager: 턴수 {GameManager.Instance.GetCurrentTurn()}로 리필");
        }
        else
        {
            // 인스턴스 없으면 PlayerPrefs로 폴백(다음 실행 시 적용됨)
            var refill = DefaultMaxTurnFallback;
            PlayerPrefs.SetInt(TURN_KEY, refill);
            PlayerPrefs.Save();
            Debug.LogWarning($"[Reset] GameManager 인스턴스 없음 → PlayerPrefs로 턴 {refill} 저장");
        }

        Debug.Log("[Reset] JSON Reset 완료");
    }
}
#endif
