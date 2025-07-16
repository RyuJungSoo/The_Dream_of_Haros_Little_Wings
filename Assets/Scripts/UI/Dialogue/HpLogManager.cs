using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpLogManager : MonoBehaviour
{
    public static HpLogManager instance;
    private static TextAsset CSV;

    [SerializeField]
    private List<string> Logs;
    private int CurrentLogIndex = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        CSV = Resources.Load<TextAsset>("Dialogue/Haro_Hp_Log");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            GetSingletLog();
    }

    public void GetLogs(int HpStatusLevel)
    {
        // HpStatusLevel�� ü�� ������ ���޵Ǹ� �ش� ������ �ش��ϴ� ������ CSV ���Ͽ��� Logs ����Ʈ�� �ҷ���
        // 1�ܰ� : �ſ� �ǰ���
        // 2�ܰ� : �ణ ��ħ
        // 3�ܰ� : �Ƿ� ����
        // 4�ܰ� : ���� ���
        // 5�ܰ� : �Ѱ� ����
        
        if (HpStatusLevel > 5)
            return;

        Logs = new List<string>();

        string[] data = CSV.text.Split(new char[] { '\n' });
        for (int i = 1 + 5 * (HpStatusLevel - 1); i < 1 + 5 * HpStatusLevel; i++)
        {
            string[] row = data[i].Split(new char[] { ',' });
            Logs.Add(row[1].Replace('\'', ','));
        }
    }

    public string GetSingletLog()
    {
        // Logs ����Ʈ�� ����� ��縦 �������� �����ͼ� return
        int NewIndex = Random.Range(0, Logs.Count);
        if (NewIndex == CurrentLogIndex)
            NewIndex++;
        if (NewIndex >= Logs.Count)
            NewIndex = 0;

        CurrentLogIndex = NewIndex;
        return Logs[NewIndex];
    }
}
