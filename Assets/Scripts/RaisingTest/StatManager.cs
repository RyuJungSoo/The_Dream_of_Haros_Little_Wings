using UnityEngine;

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
    private void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 스탯 초기화
    public void ResetStats()
    {
        Stamina_Stat = 0;
        Flightpower_Stat = 0;
        Balance_Stat = 0;
        Agility_Stat = 0;
    }

    // 스탯 증가 처리

    public void IncreaseStat(StatType type)
    {
        switch (type)
        {
            case StatType.Stamina_Stat:
                Stamina_Stat += Random.Range(0, 6); // 주 스탯
                Flightpower_Stat = Mathf.Max(0, Flightpower_Stat + Random.Range(-3, 4)); // 보조
                break;

            case StatType.Flightpower_Stat:
                Flightpower_Stat += Random.Range(0, 6);
                Stamina_Stat = Mathf.Max(0, Stamina_Stat + Random.Range(-3, 4));
                Agility_Stat = Mathf.Max(0, Agility_Stat + Random.Range(-3, 4));
                break;

            case StatType.Balance_Stat:
                Balance_Stat += Random.Range(0, 6);
                Flightpower_Stat = Mathf.Max(0, Flightpower_Stat + Random.Range(-3, 4));
                break;

            case StatType.Agility_Stat:
                Agility_Stat += Random.Range(0, 6);
                Balance_Stat = Mathf.Max(0, Balance_Stat + Random.Range(-3, 4));
                break;
        }
    }

    // 최대 스태미나 계산
    public float GetStaminaMax()
    {
        float baseValue = statData.GetBasicStamina() + (20 + Stamina_Stat * 0.7f);
        return baseValue * statData.staminaMultiplier;
    }

    // 총 비행 속도 계산
    public float GetFlightSpeed()
    {
        float speed = statData.GetBasicFlightSpeed() + Flightpower_Stat * 0.15f;
        return speed * statData.flightSpeedMultiplier;
    }

    // 스태미나 감소 속도 계산
    public float GetStaminaDrainSpeed()
    {
        float baseDrain = (statData.GetBasicStaminaDecreaseSpeed() + statData.GetBasicFlightStaminaDecreaseSpeed()) *
                          (0.3f + (1 - 0.3f) * (1 - Balance_Stat / 180f));
        return baseDrain * statData.staminaDrainMultiplier;
    }

    // 등급 반환 (예: D~S 등)
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

    public string GetStaminaGrade() => GetGrade(Stamina_Stat);
    public string GetFlightpowerGrade() => GetGrade(Flightpower_Stat);
    public string GetBalanceGrade() => GetGrade(Balance_Stat);
    public string GetAgilityGrade() => GetGrade(Agility_Stat);
    public float Total_FlightSpeed => GetFlightSpeed();
    public float Stamina_Max => GetStaminaMax();

   
        public void DecreaseStamina(float amount)
    {
        currentStamina -= amount;
        if (currentStamina < 0)
            currentStamina = 0;
    }



}

