using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat_Data", menuName = "Scriptable Object/Stat_Data", order = int.MaxValue)]
public class Stat_Data : ScriptableObject
{
    [Header("�⺻ �ɷ�ġ")]
    [SerializeField]
    private float Basic_Stamina = 20; // �⺻ ���¹̳� 
    [SerializeField]
    private float Basic_FlightSpeed = 5f; // �⺻ �����
    [SerializeField]
    private float Basic_Stamina_DecreaseSpeed = 5f; // �⺻ ���¹̳� ���� �ӵ�
    [SerializeField]
    private float Basic_Flight_Stamina_DecreaseSpeed = 6f; // �⺻ ����� ���� �ӵ�

    [Header("���¹̳� ����")]
    public int Stamina_Stat = 0;

    [Header("���� ����")]
    public int Flightpower_Stat = 0;

    [Header("������ ����")]
    public int Balance_Stat = 0;

    [Header("��ø�� ����")]
    public int Agility_Stat = 0;

    // ���� �ɷ�ġ�� ������ �����

    [Header("��� (Multiplier)")]
    public float staminaMultiplier = 1.0f;
    public float flightSpeedMultiplier = 1.0f;
    public float staminaDrainMultiplier = 1.0f;

    // �ִ� ���¹̳� ���
    public float Stamina_Max
    {
        get
        {
            float baseValue = Basic_Stamina + (20 + Stamina_Stat * 0.7f);
            return baseValue * staminaMultiplier;
        }
    }

    // ���� ��� �ӵ� (���� ���� ����)
    public float Total_FlightSpeed
    {
        get
        {
            float speed = Basic_FlightSpeed + Flightpower_Stat * 0.15f;
            return speed * flightSpeedMultiplier;
        }
    }

    // ���¹̳� ���� �ӵ�
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

    // ��ø��: ���Ϲ� ���� ����� �������� ���� ����� �ܺο��� �����ͼ� ����ϴ� �������� �ؾ� �� ��
}
