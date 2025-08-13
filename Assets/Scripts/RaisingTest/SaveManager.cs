using System;
using System.IO;
using System.Reflection;
using UnityEngine;

[Serializable]
public class SaveGameData
{
    public int currentTurn;
    public int maxTurn;

    public int staminaStat;
    public int flightpowerStat;
    public int balanceStat;
    public int agilityStat;

    public float currentStamina;
    public float maxStamina;

    public string currentStage; // "Stage1", "Stage2", "Raising_Stage"
    public bool isStage1_Clear;
    public bool isStage2_Clear;

    public bool isStage1RaisingFinished;
    public bool isStage2RaisingFinished;

    public int version = 1;
}

public class SaveManager : MonoBehaviour
{
    [Header("SceneData 동기화")]
    [SerializeField] private SceneSettingSaver sceneSettingSaver;    // 비워두면 자동 탐색
    [SerializeField] private string sceneDataAltPath = "";           // 보조 경로(비우면 persistent/Alt/SceneData.json)

    public static SaveManager Instance { get; private set; }

    public string FilePath { get; private set; }                     // haroSave.v1.json 경로

    // 진행상태 캐시(SSM 못 고치니 내부에서 보관)
    public bool Stage1Clear  { get; private set; }
    public bool Stage2Clear  { get; private set; }
    public string CurrentStageName { get; private set; } = "None";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            FilePath = Path.Combine(Application.persistentDataPath, "haroSave.v1.json");
            Debug.Log($"[SaveManager] Haro Save Path = {FilePath}");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ========================= 통합 저장(JSON) ===============
    public void SaveGame(string note = null)
    {
        var sm  = StatManager.Instance;
        var gm  = GameManager.Instance;
        var ssm = SceneSettingManager.Instance;

        var data = new SaveGameData
        {
            currentTurn = gm != null ? gm.GetCurrentTurn() : 0,
            maxTurn     = gm != null ? gm.maxTurn : 0,

            staminaStat      = sm != null ? sm.Stamina_Stat     : 0,
            flightpowerStat  = sm != null ? sm.Flightpower_Stat : 0,
            balanceStat      = sm != null ? sm.Balance_Stat     : 0,
            agilityStat      = sm != null ? sm.Agility_Stat     : 0,

            currentStamina = sm != null ? sm.currentStamina : 0f,
            maxStamina     = sm != null ? sm.maxStamina     : 0f,

            currentStage = (ssm != null && !ssm.isStage1_Clear) ? "Stage1"
                        : (ssm != null &&  ssm.isStage1_Clear && !ssm.isStage2_Clear) ? "Stage2"
                        : "None",

            isStage1_Clear = ssm != null && ssm.isStage1_Clear,
            isStage2_Clear = ssm != null && ssm.isStage2_Clear,

            isStage1RaisingFinished = false,
            isStage2RaisingFinished = false,
            version = 1
        };

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath) ?? Application.persistentDataPath);
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(FilePath, json);

            Stage1Clear = data.isStage1_Clear;
            Stage2Clear = data.isStage2_Clear;
            CurrentStageName = data.currentStage;

            Debug.Log($"[SaveManager] 저장 완료 → {FilePath}\n{json}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] 저장 실패: {e.Message}");
        }
    }

    public bool LoadGame()
    {
        if (!File.Exists(FilePath))
        {
            Debug.LogWarning("[SaveManager] 저장 파일 없음");
            return false;
        }

        try
        {
            var json = File.ReadAllText(FilePath);
            var data = JsonUtility.FromJson<SaveGameData>(json);

            var sm = StatManager.Instance;
            var gm = GameManager.Instance;

            if (sm != null)
            {
                sm.Stamina_Stat     = data.staminaStat;
                sm.Flightpower_Stat = data.flightpowerStat;
                sm.Balance_Stat     = data.balanceStat;
                sm.Agility_Stat     = data.agilityStat;

                sm.maxStamina     = Mathf.Max(100f, sm.GetStaminaMax());
                sm.currentStamina = Mathf.Clamp(data.currentStamina, 0f, sm.maxStamina);
                sm.NotifyStatsChanged();
            }

            if (gm != null)
            {
                gm.maxTurn = 12;
                gm.SetCurrentTurn(12, syncUI: true);
            }

            Stage1Clear = data.isStage1_Clear;
            Stage2Clear = data.isStage2_Clear;
            CurrentStageName = string.IsNullOrEmpty(data.currentStage) ? "None" : data.currentStage;

            Debug.Log($"[SaveManager] 로드 완료 ← {FilePath}\n{json}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] 로드 실패: {e.Message}");
            return false;
        }
    }

    public void ResetSave()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
                Debug.Log($"[SaveManager] 세이브 삭제: {FilePath}");
            }
            Stage1Clear = Stage2Clear = false;
            CurrentStageName = "None";
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] 세이브 삭제 실패: {e.Message}");
        }
    }

    public void ResetStatsHpTurns()
    {
        JsonResetUtility.ResetStatsHpTurnsJsonAndState();
    }

    public void ResetSceneData()
    {
        JsonResetUtility.ResetSceneDataJson();
    }

    // ========================= 씬데이터: 두 경로 동기화 ==============

    /// <summary>씬데이터 저장: Saver 경로(Primary)에 저장 후 Alt로 미러 복사</summary>
    public void SaveSceneData_Both()
    {
        var saver   = GetSaver();
        var primary = ResolvePrimarySceneDataPath(saver);
        var alt     = ResolveAltSceneDataPath();

        if (saver != null)
        {
            saver.SaveSceneData();
            Debug.Log("[SaveManager] SceneSettingSaver.SaveSceneData 호출");
        }
        else
        {
            Debug.LogWarning("[SaveManager] SceneSettingSaver를 찾지 못했습니다.");
        }

        if (!File.Exists(primary))
        {
            Debug.LogWarning($"[SaveManager] Primary SceneData가 존재하지 않습니다: {primary}");
            return;
        }

        TryCopy(primary, alt, overwrite: true, tag: "Mirror → ALT");
    }

    /// <summary>씬데이터 로드: Primary 없으면 Alt → Primary 복구 후 Saver.LoadSceneData()</summary>
    public void LoadSceneData_Both()
    {
        var saver   = GetSaver();
        var primary = ResolvePrimarySceneDataPath(saver);
        var alt     = ResolveAltSceneDataPath();

        if (!File.Exists(primary) && File.Exists(alt))
            TryCopy(alt, primary, overwrite: true, tag: "Recover ALT → PRIMARY");

        if (saver != null)
        {
            saver.LoadSceneData();
            Debug.Log("[SaveManager] SceneSettingSaver.LoadSceneData 호출");
        }
        else
        {
            Debug.LogWarning("[SaveManager] SceneSettingSaver 없음 → Saver 기반 로드 생략");
        }
    }

    /// <summary>씬데이터 리셋: Saver.ResetSave() + Primary/Alt 파일 모두 삭제</summary>
    public void ResetSceneData_Both()
    {
        var saver   = GetSaver();
        var primary = ResolvePrimarySceneDataPath(saver);
        var alt     = ResolveAltSceneDataPath();

        if (saver != null)
        {
            saver.ResetSave();
            Debug.Log("[SaveManager] SceneSettingSaver.ResetSave 호출");
        }
        else
        {
            Debug.LogWarning("[SaveManager] SceneSettingSaver 없음 → Saver 리셋 생략");
        }

        TryDelete(primary);
        TryDelete(alt);
    }

    // 기존 메서드도 두 경로 리셋을 쓰고 싶으면 이렇게 래핑
    public void ResetSceneDataCompletely()
    {
        ResetSceneData_Both();
    }

    /// <summary>통합 저장 + 씬데이터(두 경로) 저장을 한 번에</summary>
    public void SaveAllWithSceneData()
    {
        SaveGame();
        SaveSceneData_Both();
    }

    // ========================= 내부 ====================

    private SceneSettingSaver GetSaver()
    {
        if (sceneSettingSaver != null) return sceneSettingSaver;
#if UNITY_2023_1_OR_NEWER
        sceneSettingSaver = FindFirstObjectByType<SceneSettingSaver>(FindObjectsInactive.Include);
#else
        sceneSettingSaver = FindObjectOfType<SceneSettingSaver>(true);
#endif
        return sceneSettingSaver;
    }

    // Saver가 들고 있는 실제 Primary 경로(없으면 persistent/SceneData.json)
    private string ResolvePrimarySceneDataPath(SceneSettingSaver saver)
    {
        if (saver != null)
        {
            var pi = saver.GetType().GetProperty("SavePath", BindingFlags.Instance | BindingFlags.Public);
            if (pi != null && pi.PropertyType == typeof(string))
            {
                var v = pi.GetValue(saver) as string;
                if (!string.IsNullOrEmpty(v)) return v;
            }

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
        return Path.Combine(Application.persistentDataPath, "SceneData.json");
    }

    // Alt 경로(인스펙터 비워두면 persistent/Alt/SceneData.json)
    private string ResolveAltSceneDataPath()
    {
        if (!string.IsNullOrWhiteSpace(sceneDataAltPath))
            return sceneDataAltPath;

        var altDir = Path.Combine(Application.persistentDataPath, "Alt");
        return Path.Combine(altDir, "SceneData.json");
    }

    private void TryCopy(string src, string dst, bool overwrite, string tag)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(dst) ?? Application.persistentDataPath);
            File.Copy(src, dst, overwrite);
            Debug.Log($"[SaveManager] {tag}: {src} -> {dst}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Copy 실패({tag}) : {e.Message}");
        }
    }

    private void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"[SaveManager] Deleted: {path}");
            }
            else
            {
                Debug.Log($"[SaveManager] 없음: {path}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] 삭제 실패: {path} / {e.Message}");
        }
    }
}
