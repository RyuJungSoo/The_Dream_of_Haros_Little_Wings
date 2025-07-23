using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat_Data", menuName = "Scriptable Object/Stat_Data", order = int.MaxValue)]
public class Stat_Data : ScriptableObject
{
    [Header("기본 수치")]
    [SerializeField] private float basicStamina = 20f;                         // 기본 스태미나
    [SerializeField] private float basicFlightSpeed = 5f;                      // 기본 비행 속도
    [SerializeField] private float basicStaminaDecreaseSpeed = 5f;            // 기본 지상 소모 속도
    [SerializeField] private float basicFlightStaminaDecreaseSpeed = 6f;      // 기본 비행 소모 속도

    [Header("배율 (Multiplier)")]
    public float staminaMultiplier = 1.0f;
    public float flightSpeedMultiplier = 1.0f;
    public float staminaDrainMultiplier = 1.0f;

    public object Flightpower_Stat { get; internal set; }
    public float Total_Stamina_DecreaseSpeed { get; internal set; }

    // ====== Getter Methods for StatManager to Use ======

    public float GetBasicStamina()
    {
        return basicStamina;
    }

    public float GetBasicFlightSpeed()
    {
        return basicFlightSpeed;
    }

    public float GetBasicStaminaDecreaseSpeed()
    {
        return basicStaminaDecreaseSpeed;
    }

    public float GetBasicFlightStaminaDecreaseSpeed()
    {
        return basicFlightStaminaDecreaseSpeed;
    }

    internal void ResetStats()
    {
        throw new NotImplementedException();
    }
}
