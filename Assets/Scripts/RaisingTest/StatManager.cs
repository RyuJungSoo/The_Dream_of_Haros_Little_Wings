using UnityEngine;
using System.Collections.Generic;

public enum StatType
{
    Stamina_Stat,
    Flightpower_Stat,
    Balance_Stat,
    Agility_Stat
}

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

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (statData != null)
        {
            maxStamina = Mathf.Max(100f, GetStaminaMax());
            currentStamina = maxStamina;
        }
        else
        {
            Debug.LogError("[StatManager] statData가 할당되지 않았습니다!");
        }
    }

    public void ResetStats()
    {
        Stamina_Stat = 0;
        Flightpower_Stat = 0;
        Balance_Stat = 0;
        Agility_Stat = 0;
    }

    public void GenerateExpectedStatIncreases()
    {
        expectedMainValue = Random.Range(3, 6);
        int candidate = Random.Range(0, 4);
        bool hasZero = Stamina_Stat == 0 || Flightpower_Stat == 0 || Balance_Stat == 0 || Agility_Stat == 0;

        if (hasZero && candidate <= 0)
            candidate = Random.Range(1, 4);

        expectedSubValue = candidate;
    }

    private bool IsTrainingSuccessful()
    {
        float rate = currentStamina / maxStamina;
        float failChance = Mathf.Lerp(0f, 30f, 1f - rate);
        int roll = Random.Range(0, 100);

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
        int roll = Random.Range(0, 100);
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
        }
        else
        {
            Debug.LogWarning("[StatManager] 훈련 실패로 스탯 증가 없음!");
        }

        DecreaseStamina(staminaCost);
    }

    public void DecreaseStamina(float amount)
    {
        currentStamina -= amount;
        if (currentStamina < 0) currentStamina = 0;
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
        return ($"+{GetExpectedMainIncrease(statName)}", $"+{GetExpectedSubIncrease(statName)}");
    }

    public bool ShouldTriggerQTE(float dropFactor, float stageFactor)
    {
        float baseProbability = GetAgilityPassRate(dropFactor, stageFactor);
        float adjustedProbability = baseProbability * StatManager.Instance.statData.GetQTETriggerFactor() / 100f;

        int roll = Random.Range(0, 100);
        Debug.Log($"[민첩성 판정] 계산된 확률: {adjustedProbability:F1}%, 롤값: {roll}");

        return roll < adjustedProbability;
    }
}
