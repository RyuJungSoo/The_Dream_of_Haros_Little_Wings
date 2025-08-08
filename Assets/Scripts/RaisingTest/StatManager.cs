using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System;

public enum StatType
{
    Stamina_Stat,
    Flightpower_Stat,
    Balance_Stat,
    Agility_Stat
}

[System.Serializable]
public class StatPersistData
{
    public int Stamina_Stat;
    public int Flightpower_Stat;
    public int Balance_Stat;
    public int Agility_Stat;
    public float currentStamina;
}

[DefaultExecutionOrder(-1000)]
public class StatManager : MonoBehaviour
{
    public static StatManager Instance;

    [Header("연결된 스탯 데이터")]
    public Stat_Data statData;

    [Header("현재 스탯 레벨")]
    public int Stamina_Stat = 0;
    public int Flightpower_Stat = 0;
    public int Balance_Stat = 0;
    public int Agility_Stat = 0;

    [Header("체력 정보")]
    public float currentStamina = 100f;
    public float maxStamina = 100f;

    [Header("주/보조 스탯 저장 변수")]
    private int expectedMainValue;
    private int expectedSubValue;
    private bool hasPendingExpected; // 이번 시도/턴의 예상값이 고정되어 있는지

    // 저장 경로 + UI 갱신 이벤트(선택)
    private string statSavePath;
    public event Action OnStatsChanged;

    private void Awake()
    {
        // 싱글톤 고정 + 씬 유지
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        statSavePath = Path.Combine(Application.persistentDataPath, "stat_data.json");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        if (statData == null)
        {
            Debug.LogError("[StatManager] statData가 할당되지 않았습니다!");
            LoadStatsFromJson(invokeEvent: true);
            return;
        }

        // 시작 시 1회 로드 -> 없으면 기본값으로 파일 생성
        LoadStatsFromJson(invokeEvent: false);

        // 파생값 보정
        maxStamina = Mathf.Max(100f, GetStaminaMax());
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        OnStatsChanged?.Invoke();

        RefillStamina(save: true, invokeEvent: false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 육성 씬으로 돌아오면 다시 로드해서 UI 싱크
        if (scene.name == "Raising_Stage") // ← 프로젝트 씬 이름에 맞게 수정
        {
            LoadStatsFromJson(invokeEvent: true);
            // 씬 들어올 때 이전 턴의 예상값이 남아있지 않도록 정리
            ClearExpected();
        }
    }

    // ===== JSON 저장/로드 =====
    public void SaveStatsToJson()
    {
        try
        {
            var data = new StatPersistData
            {
                Stamina_Stat = Stamina_Stat,
                Flightpower_Stat = Flightpower_Stat,
                Balance_Stat = Balance_Stat,
                Agility_Stat = Agility_Stat,
                currentStamina = currentStamina
            };

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(statSavePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[StatManager] 저장 실패: {e.Message}");
        }
    }

    public void LoadStatsFromJson(bool invokeEvent = true)
    {
        try
        {
            Debug.Log($"[StatManager] 로드 시도: {statSavePath}, Exists={File.Exists(statSavePath)}");

            if (!File.Exists(statSavePath))
            {
                // 기본값으로 초기화만 하고 저장은 하지 않음 (덮어쓰기 방지)
                Debug.LogWarning("[StatManager] 저장 파일 없음. 메모리값 유지(기본값)로 진행");
                // 여기서 SaveStatsToJson() 호출하지 않음
            }
            else
            {
                string json = File.ReadAllText(statSavePath);
                var data = JsonUtility.FromJson<StatPersistData>(json);

                Stamina_Stat = data.Stamina_Stat;
                Flightpower_Stat = data.Flightpower_Stat;
                Balance_Stat = data.Balance_Stat;
                Agility_Stat = data.Agility_Stat;
                currentStamina = data.currentStamina;
            }

            maxStamina = Mathf.Max(100f, GetStaminaMax());
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

            if (invokeEvent) OnStatsChanged?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"[StatManager] 로드 실패: {e.Message}");
        }
    }

    public void RefillStamina(bool save = true, bool invokeEvent = true)
    {
        // statData가 null일 수도 있으니 보정
        float calcMax = (statData != null) ? GetStaminaMax() : 100f;
        maxStamina = Mathf.Max(100f, calcMax);
        currentStamina = maxStamina;

        if (save) SaveStatsToJson();
        if (invokeEvent) OnStatsChanged?.Invoke();
    }


    public void ResetStats()
    {
        Stamina_Stat = 0;
        Flightpower_Stat = 0;
        Balance_Stat = 0;
        Agility_Stat = 0;

        maxStamina = Mathf.Max(100f, GetStaminaMax());
        currentStamina = maxStamina;

        SaveStatsToJson();
        OnStatsChanged?.Invoke();
    }

    // 예상값 재굴림 금지: 없을 때만 생성해서 고정
    public void GenerateExpectedStatIncreases()
    {
        EnsureExpectedReady(StatType.Stamina_Stat);
    }

    // 이번 시도에서 사용할 예상값을 1회만 생성
    public void EnsureExpectedReady(StatType type)
    {
        if (hasPendingExpected) return;

        expectedMainValue = UnityEngine.Random.Range(3, 6);  // [3,5]
        int candidate = UnityEngine.Random.Range(0, 4);
        bool hasZero = Stamina_Stat == 0 || Flightpower_Stat == 0 || Balance_Stat == 0 || Agility_Stat == 0;
        if (hasZero && candidate <= 0) candidate = UnityEngine.Random.Range(1, 4); // 1~3 보장

        expectedSubValue = candidate;
        hasPendingExpected = true;
    }

    // 이번 시도 종료 후 예상값 폐기
    public void ClearExpected()
    {
        expectedMainValue = 0;
        expectedSubValue = 0;
        hasPendingExpected = false;
    }

    private bool IsTrainingSuccessful()
    {
        float rate = currentStamina / maxStamina;
        float failChance = Mathf.Lerp(0f, 30f, 1f - rate);
        int roll = UnityEngine.Random.Range(0, 100);

        Debug.Log($"[훈련 판정] 체력비율: {rate:F2}, 실패율: {failChance:F1}%, 랜덤: {roll}");
        return roll >= failChance;
    }

    public int GetFailureRateByStamina()
    {
        float rate = currentStamina / maxStamina;
        float failRate = Mathf.Lerp(80f, 0f, rate);
        return Mathf.RoundToInt(failRate);
    }

    public bool TryTrainingDeterministic(out bool isSuccess)
    {
        int failRate = GetFailureRateByStamina();
        int roll = UnityEngine.Random.Range(0, 100);
        isSuccess = roll >= failRate;

        Debug.Log($"[훈련 판정] 체력: {currentStamina}, 실패율: {failRate}%, 랜덤값: {roll}, 결과: {(isSuccess ? "성공" : "실패")}");
        return isSuccess;
    }

    public float GetStaminaCost(StatType type)
    {
        return type switch
        {
            StatType.Stamina_Stat => 20f,
            StatType.Flightpower_Stat => 25f,
            StatType.Balance_Stat => 15f,
            StatType.Agility_Stat => 20f,
            _ => 10f
        };
    }

    public void IncreaseStat(StatType type)
    {
        // 이번 시도에서 사용할 예상값을 고정(이미 있으면 그대로 사용)
        EnsureExpectedReady(type);

        float staminaCost = GetStaminaCost(type);
        bool isSuccess = IsTrainingSuccessful();

        Debug.Log($"[StatManager] 훈련 결과: {(isSuccess ? "성공" : "실패")} (소모 체력: {staminaCost})");

        if (isSuccess)
        {
            int main = expectedMainValue;
            int sub = expectedSubValue;

            switch (type)
            {
                case StatType.Stamina_Stat:
                    Stamina_Stat += main;
                    if (sub > 0) Flightpower_Stat = Mathf.Max(0, Flightpower_Stat + sub);
                    break;

                case StatType.Flightpower_Stat:
                    Flightpower_Stat += main;
                    if (sub > 0)
                    {
                        Stamina_Stat = Mathf.Max(0, Stamina_Stat + sub);
                        Agility_Stat = Mathf.Max(0, Agility_Stat + sub);
                    }
                    break;

                case StatType.Balance_Stat:
                    Balance_Stat += main;
                    if (sub > 0) Flightpower_Stat = Mathf.Max(0, Flightpower_Stat + sub);
                    break;

                case StatType.Agility_Stat:
                    Agility_Stat += main;
                    if (sub > 0) Balance_Stat = Mathf.Max(0, Balance_Stat + sub);
                    break;
            }

            // 성공 시 파생값 갱신
            maxStamina = Mathf.Max(100f, GetStaminaMax());
        }
        else
        {
            Debug.LogWarning("[StatManager] 훈련 실패로 스탯 증가 없음!");
        }

        // 체력 소모는 성공/실패 공통
        DecreaseStamina(staminaCost);

        // 다음 시도를 위해 예상값 폐기
        ClearExpected();

        // 저장 + UI 갱신
        SaveStatsToJson();
        OnStatsChanged?.Invoke();
    }

    public void DecreaseStamina(float amount)
    {
        currentStamina -= amount;
        if (currentStamina < 0) currentStamina = 0;

        SaveStatsToJson();
        OnStatsChanged?.Invoke();
    }

    // 스태미나 최대값 공식 적용: 20 + (20 + 스탯 * 0.8) × 계수
    public float GetStaminaMax()
    {
        return (statData.GetBasicStamina() + (20f + Stamina_Stat * 0.8f)) * statData.staminaMultiplier;
    }

    // 비행 속도 공식 적용: 10 + 비상력 * 0.2 × 계수
    public float GetFlightSpeed()
    {
        return (statData.GetBasicFlightSpeed() + Flightpower_Stat * 0.2f) * statData.flightSpeedMultiplier;
    }

    // 스태미나 감소 공식 적용: (5 + 10) * (0.2 + 0.5 * (1 - 균형감 / 180)) × 계수
    public float GetStaminaDrainSpeed()
    {
        float factor = 0.2f + 0.5f * (1f - Balance_Stat / 180f);
        return (statData.GetBasicStaminaDecreaseSpeed() + statData.GetBasicFlightStaminaDecreaseSpeed()) * factor * statData.staminaDrainMultiplier;
    }

    // 민첩성 통과 확률: (5 + 민첩성 * 0.5 * 낙하물 계수) × 스테이지 계수
    public float GetAgilityPassRate(float dropFactor, float stageFactor)
    {
        return (5f + Agility_Stat * 0.5f * dropFactor) * stageFactor;
    }

    public string GetGrade(int stat)
    {
        if (stat >= 160) return "S";
        else if (stat >= 140) return "A+";
        else if (stat >= 120) return "A";
        else if (stat >= 100) return "B+";
        else if (stat >= 80) return "B";
        else if (stat >= 60) return "C+";
        else if (stat >= 40) return "C";
        else if (stat >= 20) return "D+";
        else return "D";
    }

    public string GetStaminaGrade() => GetGrade(Stamina_Stat);
    public string GetFlightpowerGrade() => GetGrade(Flightpower_Stat);
    public string GetBalanceGrade() => GetGrade(Balance_Stat);
    public string GetAgilityGrade() => GetGrade(Agility_Stat);

    public float Total_FlightSpeed => GetFlightSpeed();
    public float Stamina_Max => GetStaminaMax();

    public int GetExpectedMainIncrease(string statName) => expectedMainValue;
    public int GetExpectedSubIncrease(string statName) => expectedSubValue;

    public (string main, string sub) GetMainAndSubStatText(string statName)
    {
        // 필요 시 외부에서 EnsureExpectedReady를 먼저 호출하고 이 값을 UI에 바인드
        return ($"+{expectedMainValue}", $"+{expectedSubValue}");
    }

    public bool ShouldTriggerQTE(float dropFactor, float stageFactor)
    {
        float baseProbability = GetAgilityPassRate(dropFactor, stageFactor);
        float adjustedProbability = baseProbability * StatManager.Instance.statData.GetQTETriggerFactor() / 100f;

        int roll = UnityEngine.Random.Range(0, 100);
        Debug.Log($"[민첩성 판정] 계산된 확률: {adjustedProbability:F1}%, 롤값: {roll}");

        return roll < adjustedProbability;
    }
    
        public void ResetStatsAndSaveFullStamina()
    {
        // 스탯 초기화
        Stamina_Stat = 0;
        Flightpower_Stat = 0;
        Balance_Stat = 0;
        Agility_Stat = 0;

        // 최대체력 재계산(최소 100 보정 + statData null 대비)
        float calcMax = (statData != null) ? GetStaminaMax() : 100f;
        maxStamina = Mathf.Max(100f, calcMax);

        // 체력 풀로 채움
        currentStamina = maxStamina;

        // JSON에 기록
        SaveStatsToJson();

        // UI 갱신 이벤트
        OnStatsChanged?.Invoke();

        Debug.Log("[StatManager] Reset+FullStamina 저장 완료");
    }

}
