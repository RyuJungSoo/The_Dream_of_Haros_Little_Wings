using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stat_Data2 : MonoBehaviour
{
    public Button increase_Stamina_Button;
    public Button increase_FlightSpeed_Button;
    public Button increase_Balance_Button;
    public Button increase_Agility_Button;
    public Stat_Data statData;

    public TextMeshProUGUI StaminaText;
    public TextMeshProUGUI FlightSpeedText;
    public TextMeshProUGUI BalanceText;
    public TextMeshProUGUI AgilityText;

    void Start()
    {
        increase_Stamina_Button.onClick.AddListener(OnIncrease_Stamina_StatClick);
        increase_FlightSpeed_Button.onClick.AddListener(OnIncrease_Flightpower_StatClick);
        increase_Balance_Button.onClick.AddListener(OnIncrease_Balance_StatClick);
        increase_Agility_Button.onClick.AddListener(OnIncrease_Agility_StatClick);

        ResetStats();
        UpdateStatUIText();
    }

    public void ResetStats()
    {
        statData.Stamina_Stat = 0;
        statData.Flightpower_Stat = 0;
        statData.Balance_Stat = 0;
        statData.Agility_Stat = 0;
    }

    void OnIncrease_Stamina_StatClick()
    {
        statData.Stamina_Stat += 4;
        statData.Flightpower_Stat += 1;
        Debug.Log("스태미나4, 비행력1 증가!");
        ApplyStatEffect();
    }

    void OnIncrease_Flightpower_StatClick()
    {
        statData.Stamina_Stat += 1;
        statData.Flightpower_Stat += 4;
        statData.Agility_Stat += 1;
        Debug.Log("스태미나1, 비행력4, 민첩성1 증가!");
        ApplyStatEffect();
    }

    void OnIncrease_Balance_StatClick()
    {
        statData.Flightpower_Stat += 1;
        statData.Balance_Stat += 4;
        Debug.Log("비행력1, 균형감4 증가!");
        ApplyStatEffect();
    }

    void OnIncrease_Agility_StatClick()
    {
        statData.Balance_Stat += 1;
        statData.Agility_Stat += 4;
        Debug.Log("균형감1, 민첩성4 증가!");
        ApplyStatEffect();
    }

    void ApplyStatEffect()
    {
        float staminaMax = statData.Stamina_Max;
        float flightSpeed = statData.Total_FlightSpeed;
        float staminaDrain = statData.Total_Stamina_DecreaseSpeed;

        Debug.Log("[능력치 반영 결과]");
        Debug.Log($"▶ 스태미나 최대치: {staminaMax}");
        Debug.Log($"▶ 비행 속도: {flightSpeed}");
        Debug.Log($"▶ 스태미나 감소 속도: {staminaDrain}");

        UpdateStatUIText(); 
    }

    public string GetStatGrade(int value)
    {
        if (value < 20) return "D";
        else if (value < 40) return "D+";
        else if (value < 60) return "C";
        else if (value < 80) return "C+";
        else if (value < 100) return "B";
        else if (value < 120) return "B+";
        else if (value < 140) return "A";
        else if (value < 160) return "A+";
        else return "S";
    }

    void UpdateStatUIText()
    {
        if (StaminaText == null || FlightSpeedText == null || BalanceText == null || AgilityText == null)
        {
            Debug.LogError("TextMeshProUGUI가 연결xx.");
            return;
        }

        StaminaText.text = $"스태미나: {GetStatGrade(statData.Stamina_Stat)}  {statData.Stamina_Stat} / 180";
        FlightSpeedText.text = $"비상력: {GetStatGrade(statData.Flightpower_Stat)}  {statData.Flightpower_Stat} / 180";
        BalanceText.text = $"균형감: {GetStatGrade(statData.Balance_Stat)}  {statData.Balance_Stat} / 180";
        AgilityText.text = $"민첩성: {GetStatGrade(statData.Agility_Stat)}  {statData.Agility_Stat} / 180";
    }
}
