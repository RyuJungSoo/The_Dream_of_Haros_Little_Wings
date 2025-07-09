using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat_Data", menuName = "Scriptable Object/Stat_Data", order = int.MaxValue)]
public class Stat_Data : ScriptableObject
{
    [Header("�⺻ ������")]
    [SerializeField]
    private float Basic_Stamina = 20; // �⺻ ���¹̳�
    [SerializeField]
    private float Basic_FlightSpeed = 5f; // �⺻ ��� �ӵ�
    [SerializeField]
    private float Basic_Stamina_DecreaseSpeed = 5f; // �⺻ ���¹̳� ���� �ӵ�
    [SerializeField]
    private float Basic_Flight_Stamina_DecreaseSpeed = 6f; // ��� �� �⺻ ���¹̳� ���� �ӵ� (�׻� Basic_Stamina_DecreaseSpeed���� Ŀ�� ��)

    [Header("���¹̳� ����")]
    public int Stamina_Stat = 0;
    
    [Header("���� ����")]
    public int Flightpower_Stat = 0;
    
    [Header("������ ����")]
    public int Balance_Stat = 0;

    [Header("��ø�� ����")]
    public int Agility_Stat = 0;

    [Header("�Ʒú� ���� ���� ���ذ�")]
    public int StaminaTraining_StaminaGrowth = 4;  // ���׹̳� �Ʒý� ���� ���ذ�
    public int StaminaTraining_FlightpowerGrowth = 1;
    public int StaminaTraining_BalanceGrowth = 0;
    public int StaminaTraining_AgilityGrowth = 0;
    public int StaminaTraining_StaminaChange = -20;

    public int FlightpowerTraining_StaminaGrowth = 1;  // ���� �Ʒý� ���� ���ذ�
    public int FlightpowerTraining_FlightpowerGrowth = 4;
    public int FlightpowerTraining_BalanceGrowth = 0;
    public int FlightpowerTraining_AgilityGrowth = 1;
    public int FlightpowerTraining_StaminaChange = -25;

    public int BalanceTraining_StaminaGrowth = 0;   // ������ �Ʒý� ���� ���ذ�
    public int BalanceTraining_FlightpowerGrowth = 1;
    public int BalanceTraining_BalanceGrowth = 4;
    public int BalanceTraining_AgilityGrowth = 0;
    public int BalanceTraining_StaminaChange = -15;

    public int AgilityTraining_StaminaGrowth = 0;   // ��ø�� �Ʒý� ���� ���ذ�
    public int AgilityTraining_FlightpowerGrowth = 0;
    public int AgilityTraining_BalanceGrowth = 0;
    public int AgilityTraining_AgilityGrowth = 4;
    public int AgilityTraining_StaminaChange = -20;

    public int Rest_StaminaChange = +30;  // �Ʒø��� �޽� �� ���� ���ذ�


    // ���� ��ġ�� �ݿ��� �����͵�
    // ���¹̳� �ִ�(���¹̳� ���� ����)
    public float Stamina_Max { get { return Basic_Stamina + (20 + Stamina_Stat * 0.7f); } }
    // ���� ��� �ӵ� (���� ���� ����)
    public float Total_FlightSpeed { get { return Basic_FlightSpeed + Flightpower_Stat * 0.15f; } }
    // ���¹̳� ���� �ӵ�
    public float Total_Stamina_DecreaseSpeed { get { return (Basic_Stamina_DecreaseSpeed + Basic_Flight_Stamina_DecreaseSpeed) * (0.3f + (1 - 0.3f) * (1 - Balance_Stat / 180)); } }
    // ��ø�� ����
    //(-> ���Ϲ� ���� ����� �������� ���� ����� �ܺο��� �����ͼ� ����ϴ� �������� �ؾ� �� ��)
}
