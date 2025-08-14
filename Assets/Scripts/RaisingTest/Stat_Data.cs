using UnityEngine;

[CreateAssetMenu(fileName = "Stat_Data", menuName = "Scriptable Object/Stat_Data", order = int.MaxValue)]
public class Stat_Data : ScriptableObject
{
    [Header("기본 수치")]
    [SerializeField] private float basicStamina = 20f; // 하로의 기본 스태미나
    [SerializeField] private float basicFlightSpeed = 10f; // 하로의 기본 상승 속도
    [SerializeField] private float basicStaminaDecrease = 10f; // 하로 기본 감소 속도
    [SerializeField] private float basicStaminaDecreaseSpeed = 5f; // 지상에서의 스태미나 감소 속도
    [SerializeField] private float basicFlightStaminaDecreaseSpeed = 10f; // 비행 시 스태미나 증가 감소량

    [Header("계산 배율 (Multiplier)")]
    public float staminaMultiplier = 1.0f;
    public float flightSpeedMultiplier = 1.0f;
    public float staminaDrainMultiplier = 1.0f;

    [Header("QTE 발동 계수 (보정값)")]
    [Range(0f, 100f)]
    public float qteTriggerFactor = 20f; // QTE 발동 기본 확률 계수 (현재 자동 계산 방식 사용 시 미사용)

    // ====== Getter Methods ======
    public float GetBasicStamina() => basicStamina;
    public float GetBasicFlightSpeed() => basicFlightSpeed;
    public float GetBasicStaminaDecreaseSpeed() => basicStaminaDecreaseSpeed;
    public float GetBasicFlightStaminaDecreaseSpeed() => basicFlightStaminaDecreaseSpeed;
    public float GetQTETriggerFactor() => qteTriggerFactor;
    public float GetBasicStaminaDecrease() => basicStaminaDecrease;

} 
