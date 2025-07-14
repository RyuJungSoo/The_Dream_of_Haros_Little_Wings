using UnityEngine;
using UnityEngine.UI;

public class Stat_Data2 : MonoBehaviour
{
    public Button increase_Stamina_Button; // 스태미나 스탯 증가 버튼
    public Button increase_FlightSpeed_Button; // 비행력 스탯 증가 버튼
    public Button increase_Balance_Button; // 균형감 스탯 증가 버튼
    public Button increase_Agility_Button; // 민첩성 스탯 증가 버튼 
    public Stat_Data statData; // Stat_Data ScriptableObject 연결
   

    void Start()
    {
        increase_Stamina_Button.onClick.AddListener(OnIncrease_Stamina_StatClick); // 버튼을 누르면 OnIncreaseAStatClick함수를 실행해라
        increase_FlightSpeed_Button.onClick.AddListener(OnIncrease_Flightpower_StatClick); // 버튼을 누르면 OnIncreaseAStatClick함수를 실행해라
        increase_Balance_Button.onClick.AddListener(OnIncrease_Balance_StatClick); // 버튼을 누르면 OnIncreaseAStatClick함수를 실행해라
        increase_Agility_Button.onClick.AddListener(OnIncrease_Agility_StatClick); // 버튼을 누르면 OnIncreaseAStatClick함수를 실행해라
    
    }

// 4가지 버튼 클릭후 스탯 성장 기준값에 따라 스탯 점수가 올라감 
    void OnIncrease_Stamina_StatClick()
    {
        // 스탯 증가 조건 처리
        statData.Stamina_Stat += 4;
        statData.Flightpower_Stat += 1;
        Debug.Log("스태미나4, 비행력1 증가!");

        // 스탯 증가에 따른 능력치 변화 처리
        ApplyStatEffect();
    }

    void OnIncrease_Flightpower_StatClick()
    {
        // 스탯 증가 조건 처리
        statData.Stamina_Stat += 1;
        statData.Flightpower_Stat += 4;
        statData.Agility_Stat += 1;
        Debug.Log("스태미나1, 비행력4, 민첩성1 증가!");

        // 스탯 증가에 따른 능력치 변화 처리
        ApplyStatEffect();
    }
        void OnIncrease_Balance_StatClick()
    {
  
        statData.Flightpower_Stat += 1;
        statData.Balance_Stat += 4;

        Debug.Log("비행력1, 균형감4 증가!");
        // 스탯 증가에 따른 능력치 변화 처리
        ApplyStatEffect();
    }
        void OnIncrease_Agility_StatClick()
    {
        // 스탯 증가 조건 처리
        statData.Balance_Stat += 1;
        statData.Agility_Stat += 4;
        Debug.Log("스태미나1,민첩성4 증가!");

        // 스탯 증가에 따른 능력치 변화 처리
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
    }
}



