using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using System.Collections; 
public enum StatType
{
    Stamina_Stat,
    Flightpower_Stat,
    Balance_Stat,
    Agility_Stat
}

[Serializable]
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
    private bool hasPendingExpected;

    private string statSavePath;

    // 외부에서 += / -= 로만 구독 가능. 호출은 내부에서만.
    public event Action OnStatsChanged;

    // 외부(다른 스크립트)에서 UI 갱신 유도할 때 이 메서드만 호출
    public void NotifyStatsChanged() => OnStatsChanged?.Invoke();

    private void Awake()
    {
        Instance = this;

        statSavePath = Path.Combine(Application.persistentDataPath, "stat_data.json");

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

        private void Start()
    {
        if (statData == null)
        {
            Debug.LogError("[StatManager] statData가 할당되지 않았습니다!");
            LoadStatsFromJson(invokeEvent: true);
            return;
        }

        LoadStatsFromJson(invokeEvent: false);

        maxStamina = Mathf.Max(100f, GetStaminaMax());
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        // UI가 이미 있다면 즉시, 아니면 다음 프레임에
        NotifyStatsChanged();
        StartCoroutine(DeferredRebindUI());
    }

    private IEnumerator DeferredRebindUI()
    {
        yield return null; // UIManager 생성 보장
        RebindUIIfNeeded();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Raising_Stage")
        {
            LoadStatsFromJson(invokeEvent: false);
            ClearExpected();

            // 씬 전환 직후, 다음 프레임에 UI 재바인딩 + 동기화
            StartCoroutine(DeferredRebindUI());

            // 예상값 1회 생성(호버/실제증가 일치 보장, +0 방지)
            GenerateExpectedStatIncreases();
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

            if (File.Exists(statSavePath))
            {
                string json = File.ReadAllText(statSavePath);
                var data = JsonUtility.FromJson<StatPersistData>(json);

                Stamina_Stat = data.Stamina_Stat;
                Flightpower_Stat = data.Flightpower_Stat;
                Balance_Stat = data.Balance_Stat;
                Agility_Stat = data.Agility_Stat;
                currentStamina = data.currentStamina;
            }
            else
            {
                Debug.LogWarning("[StatManager] 저장 파일 없음. 메모리 기본값 유지");
            }

            maxStamina = Mathf.Max(100f, GetStaminaMax());
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

            if (invokeEvent) NotifyStatsChanged();
        }
        catch (Exception e)
        {
            Debug.LogError($"[StatManager] 로드 실패: {e.Message}");
        }
    }

    public void RefillStamina(bool save = true, bool invokeEvent = true)
    {
        float calcMax = (statData != null) ? GetStaminaMax() : 100f;
        maxStamina = Mathf.Max(100f, calcMax);
        currentStamina = maxStamina;

        if (save) SaveStatsToJson();
        if (invokeEvent) NotifyStatsChanged();
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
        NotifyStatsChanged();
    }

    // 예상값 재굴림 금지: 없을 때만 생성
    public void GenerateExpectedStatIncreases()
    {
        EnsureExpectedReady(StatType.Stamina_Stat);
    }

    public void EnsureExpectedReady(StatType type)
    {
        if (hasPendingExpected) return;

        expectedMainValue = UnityEngine.Random.Range(3, 6);  // [3,5]
        int candidate = UnityEngine.Random.Range(0, 4);
        bool hasZero = Stamina_Stat == 0 || Flightpower_Stat == 0 || Balance_Stat == 0 || Agility_Stat == 0;
        if (hasZero && candidate <= 0) candidate = UnityEngine.Random.Range(1, 4); // 1~3

        expectedSubValue = candidate;
        hasPendingExpected = true;
    }

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

            maxStamina = Mathf.Max(100f, GetStaminaMax());
        }
        else
        {
            Debug.LogWarning("[StatManager] 훈련 실패로 스탯 증가 없음!");
        }

        DecreaseStamina(staminaCost);

        ClearExpected();

        SaveStatsToJson();
        NotifyStatsChanged();
    }

    public void DecreaseStamina(float amount)
    {
        currentStamina -= amount;
        if (currentStamina < 0) currentStamina = 0;

        SaveStatsToJson();
        NotifyStatsChanged();
    }

    // 파생값 계산
    public float GetStaminaMax()
    {
        return (statData.GetBasicStamina() + (20f + Stamina_Stat * 0.8f)) * statData.staminaMultiplier;
    }

    public float GetFlightSpeed()
    {
        return (statData.GetBasicFlightSpeed() + Flightpower_Stat * 0.2f) * statData.flightSpeedMultiplier;
    }

    public float GetStaminaDrainSpeed()
    {
        float baseDecrease = 10f; // 하로 기본 스태미나 감소 속도(고정)
        float factor = 0.2f + (1f - Balance_Stat / 180f); // 균형감 반영
        return baseDecrease * factor * statData.staminaDrainMultiplier; // 상승 시엔 호출부에서 ×2.5
    }


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
        => ($"+{expectedMainValue}", $"+{expectedSubValue}");

    public bool ShouldTriggerQTE(float dropFactor, float stageFactor)
    {
        float baseProbability = GetAgilityPassRate(dropFactor, stageFactor);
        float adjustedProbability = baseProbability * statData.GetQTETriggerFactor() / 100f;

        int roll = UnityEngine.Random.Range(0, 100);
        Debug.Log($"[민첩성 판정] 계산된 확률: {adjustedProbability:F1}%, 롤값: {roll}");

        return roll < adjustedProbability;
    }

    public void ResetStatsAndSaveFullStamina()
    {
        Stamina_Stat = 0;
        Flightpower_Stat = 0;
        Balance_Stat = 0;
        Agility_Stat = 0;

        float calcMax = (statData != null) ? GetStaminaMax() : 100f;
        maxStamina = Mathf.Max(100f, calcMax);
        currentStamina = maxStamina;

        SaveStatsToJson();
        NotifyStatsChanged();

        Debug.Log("[StatManager] Reset+FullStamina 저장 완료");
    }


    //첫 시작시 +0만 되는 문제
    // UI가 예상 증가값을 볼 때 쓰는 안전한 미리보기
    public void PeekExpectedIncrease(StatType type, out int main, out int sub)
    {
        EnsureExpectedReady(type);   // 없으면 1회만 생성해서 고정
        main = expectedMainValue;
        sub = expectedSubValue;
    }

    // (선택) UI에서 문자열로 바로 쓰고 싶으면 이 메서드도 Ensure 포함시키자
    public (string main, string sub) GetMainAndSubStatText(StatType type)
    {
        EnsureExpectedReady(type);
        return ($"+{expectedMainValue}", $"+{expectedSubValue}");
    }
    
        private void RebindUIIfNeeded()
    {
        var ui = UIManager.Instance;
        if (ui == null) return;

        // 중복 구독 방지 후 재구독
        OnStatsChanged -= ui.UpdateStatUI;
        OnStatsChanged += ui.UpdateStatUI;

        // 최초 1회 즉시 동기화
        ui.UpdateStatUI();
        // 턴 텍스트도 맞춰주고 싶으면:
        if (GameManager.Instance != null)
            ui.UpdateTurnText(GameManager.Instance.GetCurrentTurn());
    }
}
