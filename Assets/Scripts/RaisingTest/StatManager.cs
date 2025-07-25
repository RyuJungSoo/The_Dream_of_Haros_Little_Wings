using UnityEngine;
using System.Collections.Generic;


// 스탯 종류 열거형 정의
public enum StatType
{
    Stamina_Stat,         // 체력
    Flightpower_Stat,     // 비행력
    Balance_Stat,         // 균형감
    Agility_Stat          // 민첩성

    
}

public class StatManager : MonoBehaviour
{
    
    private Dictionary<string, int> expectedMainIncreases = new Dictionary<string, int>();
    private Dictionary<string, int> expectedSubIncreases = new Dictionary<string, int>();
    public static StatManager Instance;

    [Header("연결된 스탯 데이터")]
    public Stat_Data statData;                       // 스탯 관련 기본값 및 배율 데이터를 담은 SO

    [Header("현재 스탯 레벨")]
    public int Stamina_Stat = 0;                     // 현재 체력 스탯
    public int Flightpower_Stat = 0;                 // 현재 비행력 스탯
    public int Balance_Stat = 0;                     // 현재 균형감 스탯
    public int Agility_Stat = 0;                     // 현재 민첩성 스탯

    [Header("체력 정보")]
    public float currentStamina = 100f;              // 현재 체력 수치
    public float maxStamina = 100f;                  // 최대 체력 수치

   [Header("주/보조 스탯 저장 변수")]
    private int expectedMainValue;
    private int expectedSubValue;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // 스탯 초기화
    public void ResetStats()
    {
        Stamina_Stat = 0;
        Flightpower_Stat = 0;
        Balance_Stat = 0;
        Agility_Stat = 0;
    }

    // 예상 증가값 저장용 딕셔너리
    private Dictionary<string, (int main, int sub)> expectedIncreases = new Dictionary<string, (int, int)>();

    public void GenerateExpectedStatIncreases()
    {
        expectedMainValue = Random.Range(3, 6);  // 3~5

        int candidate = Random.Range(1, 4);     // -3 ~ 3

        bool hasZeroStat = Stamina_Stat == 0 || Flightpower_Stat == 0 || Balance_Stat == 0 || Agility_Stat == 0;

        if (hasZeroStat && candidate <= 0)
        {
            candidate = Random.Range(1, 4);  // 1~3 사이로 재설정
        }

        expectedSubValue = candidate;
    }

        public int GetTrainingFailureRate()
    {
        float stamina = currentStamina;

        if (stamina < 20) return Random.Range(81, 101);   // 81~100%
        else if (stamina < 40) return Random.Range(61, 81); // 61~80%
        else if (stamina < 60) return Random.Range(41, 61); // 41~60%
        else if (stamina < 80) return Random.Range(21, 41); // 21~40%
        else return Random.Range(0, 21);                  // 0~20%
        }

    private bool IsTrainingSuccessful()
    {
        float rate = currentStamina / maxStamina;
        float failChance = Mathf.Lerp(0f, 30f, 1f - rate);  // 최대 실패율 30%

        int roll = Random.Range(0, 100);
        Debug.Log($"[훈련 판정] 체력 비율: {rate:F2}, 실패 확률: {failChance:F1}%, 랜덤값: {roll}");

        return roll >= failChance;
    }

    public int GetFailureRateByStamina()
    {
        float stamina = currentStamina;

        if (stamina >= 80f) return 0;
        else if (stamina >= 60f) return 20;
        else if (stamina >= 40f) return 40;
        else if (stamina >= 20f) return 60;
        else return 80;
    }

    public bool TryTrainingDeterministic(out bool isSuccess)
    {
        int failRate = GetFailureRateByStamina();
        int roll = Random.Range(0, 100);  // 여전히 확률은 있음

        isSuccess = roll >= failRate;
        Debug.Log($"[훈련 판정] 체력: {currentStamina}, 실패율: {failRate}%, 랜덤값: {roll}, 결과: {(isSuccess ? "성공" : "실패")}");

        return isSuccess;
    }





    public float GetStaminaCost(StatType type)
    {
        switch (type)
        {
            case StatType.Stamina_Stat: return 20f;
            case StatType.Flightpower_Stat: return 25f;
            case StatType.Balance_Stat: return 15f;
            case StatType.Agility_Stat: return 20f;
            default: return 10f;
        }
    }

    public void IncreaseStat(StatType type)
    {
        float staminaCost = GetStaminaCost(type);  // 체력 소모량
        bool isSuccess = IsTrainingSuccessful();   // 성공 여부

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
            Debug.LogWarning("[StatManager] 훈련 실패로 스탯은 증가하지 않음!");
        }

        DecreaseStamina(staminaCost); // 성공/실패 관계없이 체력 소모
    }




    // 체력 최대값 계산
    public float GetStaminaMax()
    {
        float baseValue = statData.GetBasicStamina() + (20 + Stamina_Stat * 0.7f);
        return baseValue * statData.staminaMultiplier;
    }

    // 비행 속도 계산
    public float GetFlightSpeed()
    {
        float speed = statData.GetBasicFlightSpeed() + Flightpower_Stat * 0.15f;
        return speed * statData.flightSpeedMultiplier;
    }

    // 체력 소모 속도 계산 (균형감이 높을수록 절약됨)
    public float GetStaminaDrainSpeed()
    {
        float baseDrain = (statData.GetBasicStaminaDecreaseSpeed() + statData.GetBasicFlightStaminaDecreaseSpeed()) *
                          (0.3f + (1 - 0.3f) * (1 - Balance_Stat / 180f));
        return baseDrain * statData.staminaDrainMultiplier;
    }

    // 스탯 등급 산정
    public string GetGrade(int stat)
    {
        if (stat >= 160 && stat <= 180) return "S";
        else if (stat >= 140) return "A+";
        else if (stat >= 120) return "A";
        else if (stat >= 100) return "B+";
        else if (stat >= 80) return "B";
        else if (stat >= 60) return "C+";
        else if (stat >= 40) return "C";
        else if (stat >= 20) return "D+";
        else return "D";
    }

    // 각 스탯별 등급 반환
    public string GetStaminaGrade() => GetGrade(Stamina_Stat);
    public string GetFlightpowerGrade() => GetGrade(Flightpower_Stat);
    public string GetBalanceGrade() => GetGrade(Balance_Stat);
    public string GetAgilityGrade() => GetGrade(Agility_Stat);

    // 편의용 프로퍼티
    public float Total_FlightSpeed => GetFlightSpeed();
    public float Stamina_Max => GetStaminaMax();

    // 체력 감소 처리
    public void DecreaseStamina(float amount)
    {
        currentStamina -= amount;
        if (currentStamina < 0)
            currentStamina = 0;
    }

    public int GetExpectedMainIncrease(string statName)
    {
        return expectedMainValue;
    }

    public int GetExpectedSubIncrease(string statName)
    {
        return expectedSubValue;
    }


    public (string main, string sub) GetMainAndSubStatText(string statName)
    {
        return (
            $"+{GetExpectedMainIncrease(statName)}",
            $"+{GetExpectedSubIncrease(statName)}"
        );
    }

} 