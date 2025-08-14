using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

public static class JsonResetUtility
{
    private const int DefaultMaxTurnFallback = 12;
    private const string TURN_KEY = "gm_current_turn";

    // ===== 기존 유지 =====
    public static void ResetSceneDataJson()
    {
        var path = ResolvePrimarySceneDataPath();
        if (File.Exists(path)) { File.Delete(path); Debug.Log($"[Reset] Deleted : {path}"); }
        else Debug.Log($"[Reset] SceneData.json 없음 (path: {path})");
    }

    public static void ResetStatsHpTurnsJsonAndState()
    {
        var statPath = Path.Combine(Application.persistentDataPath, "stat_data.json");
        var savePath = Path.Combine(Application.persistentDataPath, "haroSave.v1.json");

        if (File.Exists(statPath)) { File.Delete(statPath); Debug.Log($"[Reset] Deleted: {statPath}"); }
        else Debug.Log("[Reset] stat_data.json 없음");

        if (File.Exists(savePath)) { File.Delete(savePath); Debug.Log($"[Reset] Deleted: {savePath}"); }
        else Debug.Log("[Reset] haroSave.v1.json 없음");

        if (StatManager.Instance != null)
        {
            StatManager.Instance.ResetStatsAndSaveFullStamina();
            Debug.Log("[Reset] StatManager: 스탯 0 + 풀피 저장 완료");
        }
        else Debug.LogWarning("[Reset] StatManager 인스턴스 없음(플레이 중이 아님).");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetTurnsToMax(syncUI: true);
            Debug.Log($"[Reset] GameManager: 턴수 {GameManager.Instance.GetCurrentTurn()}로 리필");
        }
        else
        {
            PlayerPrefs.SetInt(TURN_KEY, DefaultMaxTurnFallback);
            PlayerPrefs.Save();
            Debug.LogWarning($"[Reset] GameManager 인스턴스 없음 → PlayerPrefs {DefaultMaxTurnFallback}");
        }

        Debug.Log("[Reset] JSON Reset 완료");
    }

    public static void SaveSceneDataWith(SceneSettingSaver saver = null)
    {
        if (saver == null) saver = FindSaver();
        if (saver == null) { Debug.LogWarning("[SceneData] SceneSettingSaver 못 찾음"); return; }

        saver.SaveSceneData();
        Debug.Log("[SceneData] SaveSceneData 호출 완료");
    }

    public static void ResetAndSaveSceneData(SceneSettingSaver saver = null)
    {
        ResetSceneDataJson();
        SaveSceneDataWith(saver);
    }

    // ====== 새로 추가: 다중 경로 지원 ======

    /// <summary>
    /// SceneData 저장 후, 원본 파일을 지정한 다른 경로들로 복사(미러링)합니다.
    /// </summary>
    public static void SaveSceneDataWithAllPaths(SceneSettingSaver saver = null, params string[] extraPaths)
    {
        if (saver == null) saver = FindSaver();
        if (saver == null) { Debug.LogWarning("[SceneData] Saver 없음 → 저장 불가"); return; }

        saver.SaveSceneData();

        // 원본 경로 확인
        var primary = ResolvePrimarySceneDataPath(saver);
        if (!File.Exists(primary))
        {
            Debug.LogWarning($"[SceneData] SaveSceneData 호출했지만 원본 파일이 없음: {primary}");
            return;
        }

        // 미러 대상 경로들
        foreach (var dst in UniqueValidPaths(primary, extraPaths))
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dst) ?? Application.persistentDataPath);
                File.Copy(primary, dst, overwrite: true);
                Debug.Log($"[SceneData] 미러 복사 완료 → {dst}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SceneData] 미러 복사 실패 → {dst} : {e.Message}");
            }
        }
    }

    /// <summary>
    /// 모든 경로(원본 + extraPaths)의 SceneData 파일을 일괄 삭제합니다.
    /// </summary>
    public static void ResetSceneDataJsonAll(SceneSettingSaver saver = null, params string[] extraPaths)
    {
        var primary = ResolvePrimarySceneDataPath(saver);
        foreach (var p in UniqueValidPaths(primary, extraPaths))
        {
            try
            {
                if (File.Exists(p))
                {
                    File.Delete(p);
                    Debug.Log($"[Reset] Deleted: {p}");
                }
                else Debug.Log($"[Reset] 없음: {p}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Reset] 삭제 실패 → {p} : {e.Message}");
            }
        }
    }

    /// <summary>
    /// 삭제 후 저장 + 미러 복사까지 한 번에.
    /// </summary>
    public static void ResetAndSaveSceneDataAll(SceneSettingSaver saver = null, params string[] extraPaths)
    {
        ResetSceneDataJsonAll(saver, extraPaths);
        SaveSceneDataWithAllPaths(saver, extraPaths);
    }

    // ===== 내부 유틸 =====

    private static SceneSettingSaver FindSaver()
    {
#if UNITY_2023_1_OR_NEWER
        return Object.FindFirstObjectByType<SceneSettingSaver>(FindObjectsInactive.Include);
#else
        return Object.FindObjectOfType<SceneSettingSaver>(true);
#endif
    }

    /// <summary>
    /// Saver가 경로를 들고 있으면 그걸, 없으면 기본(persistent/SceneData.json)
    /// </summary>
    private static string ResolvePrimarySceneDataPath(SceneSettingSaver saver = null)
    {
        if (saver == null) saver = FindSaver();

        // 공개 프로퍼티 SavePath 우선
        var pi = saver?.GetType().GetProperty("SavePath", BindingFlags.Instance | BindingFlags.Public);
        if (pi != null && pi.PropertyType == typeof(string))
        {
            var v = pi.GetValue(saver) as string;
            if (!string.IsNullOrEmpty(v)) return v;
        }

        // 자주 쓰는 필드명 시도
        if (saver != null)
        {
            string[] fields = { "savePath", "filePath", "path", "_savePath", "_filePath" };
            foreach (var name in fields)
            {
                var fi = saver.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (fi != null && fi.FieldType == typeof(string))
                {
                    var v = fi.GetValue(saver) as string;
                    if (!string.IsNullOrEmpty(v)) return v;
                }
            }
        }

        // 폴백
        return Path.Combine(Application.persistentDataPath, "SceneData.json");
    }

    private static IEnumerable<string> UniqueValidPaths(string primary, params string[] extra)
    {
        var set = new HashSet<string>();
        if (!string.IsNullOrEmpty(primary)) set.Add(primary);

        if (extra != null)
        {
            foreach (var p in extra)
            {
                if (string.IsNullOrWhiteSpace(p)) continue;
                // 동일 경로 중복 제거
                if (!set.Contains(p)) set.Add(p);
            }
        }

        // 원본(primary)은 첫 번째, 나머지는 이후(원본은 복사 대상에서 제외)
        foreach (var p in set)
        {
            if (p == primary) continue;
            yield return p;
        }
    }

    
}
