using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat_Data", menuName = "Scriptable Object/Stat_Data", order = int.MaxValue)]
public class Stat_Data : ScriptableObject
{
    [Header("기본 데이터")]
    [SerializeField]
    private float Basic_Stamina = 20; // 기본 스태미나
    [SerializeField]
    private float Basic_FlightSpeed = 5f; // 기본 상승 속도
    [SerializeField]
    private float Basic_Stamina_DecreaseSpeed = 5f; // 기본 스태미나 감소 속도
    [SerializeField]
    private float Basic_Flight_Stamina_DecreaseSpeed = 6f; // 상승 시 기본 스태미나 감소 속도 (항상 Basic_Stamina_DecreaseSpeed보다 커야 함)

    [Header("스태미나 설정")]
    public int Stamina_Stat = 0;
    
    [Header("비상력 설정")]
    public int Flightpower_Stat = 0;
    
    [Header("균형감 설정")]
    public int Balance_Stat = 0;

    [Header("민첩성 설정")]
    public int Agility_Stat = 0;


    // 스탯 수치값 반영한 데이터들
    // 스태미나 최댓값(스태미나 스탯 관련)
    public float Stamina_Max { get { return Basic_Stamina + (20 + Stamina_Stat * 0.7f); } }
    // 최종 상승 속도 (비상력 스탯 관련)
    public float Total_FlightSpeed { get { return Basic_FlightSpeed + Flightpower_Stat * 0.15f; } }
    // 스태미나 감소 속도
    public float Total_Stamina_DecreaseSpeed { get { return (Basic_Stamina_DecreaseSpeed + Basic_Flight_Stamina_DecreaseSpeed) * (0.3f + (1 - 0.3f) * (1 - Balance_Stat / 180)); } }
    // 민첩성 판정
    //(-> 낙하물 보정 계수와 스테이지 보정 계수를 외부에서 가져와서 계산하는 방향으로 해야 할 듯)
}
