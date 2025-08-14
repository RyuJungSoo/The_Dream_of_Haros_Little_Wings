using System;
using System.IO;
using System.Collections;
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

    public string currentStage; // "Stage1","Stage2","Raising_Stage","None"
    public bool isStage1_Clear;
    public bool isStage2_Clear;

    public bool isStage1RaisingFinished;
    public bool isStage2RaisingFinished;

    public int version = 1;
}

[DefaultExecutionOrder(-50)]
public class SaveManager : MonoBehaviour
{
    [Header("SceneData 동기화")]
    [SerializeField] private SceneSettingSaver sceneSettingSaver; // 비워두면 자동 탐색

    public static SaveManager Instance { get; private set; }
    public string FilePath { get; private set; } // haroSave.v1.json

    private const int DefaultMaxTurn = 12;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        FilePath = Path.Combine(Application.persistentDataPath, "haroSave.v1.json");
        Debug.Log($"[SaveManager] Save Path = {FilePath}");
    }

    private void Start()
    {
        // 한 프레임 뒤에 초기 저장/로드(오브젝트 생성 타이밍 이슈 회피)
        StartCoroutine(InitAfterFirstFrame());
    }

    private IEnumerator InitAfterFirstFrame()
    {
        yield return null;
        EnsureInitialGameSave();
        EnsureInitialSceneData();
    }

    // ----------------- Game Save / Load -----------------
    public void SaveGame()
    {
        var sm  = StatManager.Instance;
        var gm  = GameManager.Instance;
        var ssm = SceneSettingManager.Instance; // 없어도 OK

        var data = new SaveGameData
        {
            currentTurn = gm ? gm.GetCurrentTurn() : 0,
            maxTurn     = gm ? gm.maxTurn : 0,

            staminaStat      = sm ? sm.Stamina_Stat     : 0,
            flightpowerStat  = sm ? sm.Flightpower_Stat : 0,
            balanceStat      = sm ? sm.Balance_Stat     : 0,
            agilityStat      = sm ? sm.Agility_Stat     : 0,

            currentStamina = sm ? sm.currentStamina : 0f,
            maxStamina     = sm ? sm.maxStamina     : 0f,

            currentStage = (ssm && !ssm.isStage1_Clear) ? "Stage1"
                        : (ssm &&  ssm.isStage1_Clear && !ssm.isStage2_Clear) ? "Stage2"
                        : "None",

            isStage1_Clear = ssm && ssm.isStage1_Clear,
            isStage2_Clear = ssm && ssm.isStage2_Clear,

            isStage1RaisingFinished = false,
            isStage2RaisingFinished = false,
            version = 1
        };

        try
        {
            EnsureDirOf(FilePath);
            File.WriteAllText(FilePath, JsonUtility.ToJson(data, true));
            Debug.Log("[SaveManager] Game 저장 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Game 저장 실패: {e.Message}");
        }

        // 게임 저장 시 씬데이터도 함께 저장
        SaveSceneDataJson();
    }

    public bool LoadGame()
    {
        if (!File.Exists(FilePath))
        {
            Debug.LogWarning("[SaveManager] Game 저장 파일 없음");
            return false;
        }

        try
        {
            var data = JsonUtility.FromJson<SaveGameData>(File.ReadAllText(FilePath));

            var sm = StatManager.Instance;
            var gm = GameManager.Instance;

            if (sm)
            {
                sm.Stamina_Stat     = data.staminaStat;
                sm.Flightpower_Stat = data.flightpowerStat;
                sm.Balance_Stat     = data.balanceStat;
                sm.Agility_Stat     = data.agilityStat;

                sm.maxStamina     = Mathf.Max(data.maxStamina > 0 ? data.maxStamina : 100f, sm.GetStaminaMax());
                sm.currentStamina = Mathf.Clamp(data.currentStamina, 0f, sm.maxStamina);
                sm.NotifyStatsChanged();
            }

            if (gm)
            {
                int savedMax = data.maxTurn > 0 ? data.maxTurn : DefaultMaxTurn;
                int savedCur = Mathf.Clamp(data.currentTurn, 0, savedMax);
                gm.maxTurn = savedMax;
                gm.SetCurrentTurn(savedCur, syncUI: true);
            }

            Debug.Log("[SaveManager] Game 로드 완료");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Game 로드 실패: {e.Message}");
            return false;
        }
    }

    public void ResetSave()
    {
        try
        {
            if (File.Exists(FilePath)) File.Delete(FilePath);
            Debug.Log("[SaveManager] Game 세이브 삭제");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Game 세이브 삭제 실패: {e.Message}");
        }
    }

    //  SceneData Save / Load / Reset 
    public void SaveSceneDataJson()
    {
        var saver = GetSaver();
        if (!saver) { Debug.LogWarning("[SaveManager] SceneSettingSaver 없음 → SceneData 저장 불가"); return; }

        saver.SaveSceneData();
        Debug.Log("[SaveManager] SceneData 저장 완료");
    }

    public void LoadSceneDataJson()
    {
        var saver = GetSaver();
        if (!saver) { Debug.LogWarning("[SaveManager] SceneSettingSaver 없음 → SceneData 로드 불가"); return; }

        saver.LoadSceneData();
        Debug.Log("[SaveManager] SceneData 로드 시도");
    }

    public void ResetSceneDataJsonFull()
    {
        var saver = GetSaver();
        if (saver) saver.ResetSave();
        else JsonResetUtility.ResetSceneDataJson(); // 기본 경로 삭제 폴백

        Debug.Log("[SaveManager] SceneData 초기화 완료");
    }

    // 자동 초기 저장/로드 
    private void EnsureInitialGameSave()
    {
        if (!File.Exists(FilePath)) { SaveGame(); }
        else { LoadGame(); }
    }

    private void EnsureInitialSceneData()
    {
        var saver = GetSaver();
        if (!saver) { Debug.LogWarning("[SaveManager] SceneSettingSaver 없음 → SceneData 자동 처리 생략"); return; }

        if (!File.Exists(saver.FilePath)) { saver.SaveSceneData(); }
        else { saver.LoadSceneData(); }
    }

    //  앱 일시정지/종료 자동 저장 
    private void OnApplicationPause(bool pause)
    {
        if (pause) { SaveGame(); SaveSceneDataJson(); }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
        SaveSceneDataJson();
    }

    //  내부 유틸 
    private SceneSettingSaver GetSaver()
    {
        if (sceneSettingSaver) return sceneSettingSaver;
        if (SceneSettingSaver.Instance) { sceneSettingSaver = SceneSettingSaver.Instance; return sceneSettingSaver; }

#if UNITY_2023_1_OR_NEWER
        sceneSettingSaver = FindFirstObjectByType<SceneSettingSaver>(FindObjectsInactive.Include);
#else
        sceneSettingSaver = FindObjectOfType<SceneSettingSaver>(true);
#endif
        return sceneSettingSaver;
    }

    private static void EnsureDirOf(string path)
    {
        var dir = Path.GetDirectoryName(path) ?? Application.persistentDataPath;
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
    }
}
