using UnityEngine;
using System.Collections.Generic;

public enum StatType
{
    Stamina_Stat,         // 체력
    Flightpower_Stat,     // 비행력
    Balance_Stat,         // 균형감
    Agility_Stat          // 민첩성
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
            maxStamina = Mathf.Max(100f, GetStaminaMax());  // 최소값 보장
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
        int candidate = Random.Range(1, 4);
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

    // ✅ 새로 수정된 실패율 계산 (체력 비율 기반 연속 함수)
    public int GetFailureRateByStamina()
    {
        float rate = currentStamina / maxStamina;
        float failRate = Mathf.Lerp(80f, 0f, rate);  // 체력 낮을수록 최대 80% 실패율
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
                    Flightpower_Stat = Mathf.Max(0, Flightpower_Stat + sub);
                    break;
                case StatType.Flightpower_Stat:
                    Flightpower_Stat += main;
                    Stamina_Stat = Mathf.Max(0, Stamina_Stat + sub);
                    Agility_Stat = Mathf.Max(0, Agility_Stat + sub);
                    break;
                case StatType.Balance_Stat:
                    Balance_Stat += main;
                    Flightpower_Stat = Mathf.Max(0, Flightpower_Stat + sub);
                    break;
                case StatType.Agility_Stat:
                    Agility_Stat += main;
                    Balance_Stat = Mathf.Max(0, Balance_Stat + sub);
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

    public float GetStaminaMax()
    {
        float baseValue = statData.GetBasicStamina() + (20 + Stamina_Stat * 0.7f);
        return baseValue * statData.staminaMultiplier;
    }

    public float GetFlightSpeed()
    {
        float speed = statData.GetBasicFlightSpeed() + Flightpower_Stat * 0.15f;
        return speed * statData.flightSpeedMultiplier;
    }

    public float GetStaminaDrainSpeed()
    {
        float baseDrain = (statData.GetBasicStaminaDecreaseSpeed() + statData.GetBasicFlightStaminaDecreaseSpeed()) *
                          (0.3f + (1 - 0.3f) * (1 - Balance_Stat / 180f));
        return baseDrain * statData.staminaDrainMultiplier;
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

    //하로 대사 관련
    public string GetStaminaStatusMessage()
    {
        float rate = currentStamina / maxStamina * 100f;

        if (rate >= 80f)
        {
            return "…응, 지금은 꽤 괜찮아. 뭐든 할 수 있을 것 같아… 아마도.";
        }
        else if (rate >= 60f)
        {
            return "…조금 피곤하지만, 아직은 버틸 수 있어.";
        }
        else if (rate >= 40f)
        {
            return "…계속해도 괜찮을까… 실수하지 않으려면 조심해야 해…";
        }
        else if (rate >= 20f)
        {
            return "…힘이 안 들어… 실수할지도 몰라… 쉬는 게 낫지 않을까…?";
        }
        else
        {
            return "…미안해… 더는… 안 될 것 같아… 나… 쓰러질지도…";
        }
    }

}
