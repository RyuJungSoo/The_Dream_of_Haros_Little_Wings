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
