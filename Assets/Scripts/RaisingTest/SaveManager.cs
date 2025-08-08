using System;
using System.IO;
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

    // "Stage1", "Stage2", "Raising_Stage" 
    public string currentStage;

    public bool isStage1_Clear;
    public bool isStage2_Clear;

    public bool isStage1RaisingFinished;
    public bool isStage2RaisingFinished;

    public int version = 1;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public string FilePath { get; private set; }

    // SSM 못 고치니 진행상태는 여기서 보관해서 다른 매니저가 참조
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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>현재 매니저들의 상태를 한 방에 저장</summary>
    public void SaveGame(string note = null)
    {
        var sm  = StatManager.Instance;
        var gm  = GameManager.Instance;
        var ssm = SceneSettingManager.Instance;

        var data = new SaveGameData
        {
            currentTurn      = gm != null ? gm.GetCurrentTurn() : 0,
            maxTurn          = gm != null ? gm.maxTurn : 0,

            staminaStat      = sm != null ? sm.Stamina_Stat     : 0,
            flightpowerStat  = sm != null ? sm.Flightpower_Stat : 0,
            balanceStat      = sm != null ? sm.Balance_Stat     : 0,
            agilityStat      = sm != null ? sm.Agility_Stat     : 0,

            currentStamina   = sm != null ? sm.currentStamina   : 0f,
            maxStamina       = sm != null ? sm.maxStamina       : 0f,

            currentStage     = (ssm != null && !ssm.isStage1_Clear) ? "Stage1"
                               : (ssm != null &&  ssm.isStage1_Clear && !ssm.isStage2_Clear) ? "Stage2"
                               : "None",

            isStage1_Clear   = ssm != null && ssm.isStage1_Clear,
            isStage2_Clear   = ssm != null && ssm.isStage2_Clear,

            isStage1RaisingFinished = false,
            isStage2RaisingFinished = false,
            version = 1
        };

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath) ?? Application.persistentDataPath);
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(FilePath, json);

            // 내부 캐시도 같이 갱신
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

    /// <summary>세이브 파일을 읽어와 매니저들에 적용</summary>
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

            var sm  = StatManager.Instance;
            var gm  = GameManager.Instance;
            var ssm = SceneSettingManager.Instance;

            if (sm != null)
            {
                sm.Stamina_Stat     = data.staminaStat;
                sm.Flightpower_Stat = data.flightpowerStat;
                sm.Balance_Stat     = data.balanceStat;
                sm.Agility_Stat     = data.agilityStat;

                // 최대체력 재계산 후 현재체력 클램프
                sm.maxStamina     = Mathf.Max(100f, sm.GetStaminaMax());
                sm.currentStamina = Mathf.Clamp(data.currentStamina, 0f, sm.maxStamina);

                // 외부에서 이벤트 직접 호출 금지 → 래퍼 사용
                sm.NotifyStatsChanged();
            }

                if (gm != null)
        {
            gm.maxTurn = 12; // 로드 될때마다 턴수는 12로 고정 
            gm.SetCurrentTurn(12, syncUI: true); // ★ 현재 턴도 로드될때마다 12로 고정 그래야 2번째 육성때 훈련가능
        }   

            // SSM은 수정 못 하므로, 내부 캐시에만 보관
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

    // 선택: 저장 파일 리셋
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
}
