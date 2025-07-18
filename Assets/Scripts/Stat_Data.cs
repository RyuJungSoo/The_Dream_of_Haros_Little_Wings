using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat_Data", menuName = "Scriptable Object/Stat_Data", order = int.MaxValue)]
public class Stat_Data : ScriptableObject
{
    [Header("기본 능력치")]
    [SerializeField]
    private float Basic_Stamina = 20; // 기본 스태미나 
    [SerializeField]
    private float Basic_FlightSpeed = 5f; // 기본 비행력
    [SerializeField]
    private float Basic_Stamina_DecreaseSpeed = 5f; // 기본 스태미나 감소 속도
    [SerializeField]
    private float Basic_Flight_Stamina_DecreaseSpeed = 6f; // 기본 비행력 감소 속도

    [Header("스태미나 스탯")]
    public int Stamina_Stat = 0;

    [Header("비상력 스탯")]
    public int Flightpower_Stat = 0;

    [Header("균형감 스탯")]
    public int Balance_Stat = 0;

    [Header("민첩성 스탯")]
    public int Agility_Stat = 0;

    // 최종 능력치에 곱해질 계수들

    [Header("계수 (Multiplier)")]
    public float staminaMultiplier = 1.0f;
    public float flightSpeedMultiplier = 1.0f;
    public float staminaDrainMultiplier = 1.0f;

    // 최대 스태미나 계산
    public float Stamina_Max
    {
        get
        {
            float baseValue = Basic_Stamina + (20 + Stamina_Stat * 0.7f);
            return baseValue * staminaMultiplier;
        }
    }

    // 최종 상승 속도 (비상력 스탯 관련)
    public float Total_FlightSpeed
    {
        get
        {
            float speed = Basic_FlightSpeed + Flightpower_Stat * 0.15f;
            return speed * flightSpeedMultiplier;
        }
    }

    // 스태미나 감소 속도
    public float Total_Stamina_DecreaseSpeed
    {
        get
        {
            float baseDrain = (Basic_Stamina_DecreaseSpeed + Basic_Flight_Stamina_DecreaseSpeed) *
                              (0.3f + (1 - 0.3f) * (1 - Balance_Stat / 180f));
            return baseDrain * staminaDrainMultiplier;
        }
    }

    internal void ResetStats()
    {
        throw new NotImplementedException();
    }

    // 민첩성: 낙하물 보정 계수와 스테이지 보정 계수를 외부에서 가져와서 계산하는 방향으로 해야 할 듯
}
