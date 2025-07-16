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
        // HpStatusLevel에 체력 레벨이 전달되면 해당 레벨에 해당하는 대사들을 CSV 파일에서 Logs 리스트로 불러옴
        // 1단계 : 매우 건강함
        // 2단계 : 약간 지침
        // 3단계 : 피로 누적
        // 4단계 : 위험 경고
        // 5단계 : 한계 상태
        
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
        // Logs 리스트에 저장된 대사를 랜덤으로 가져와서 return
        int NewIndex = Random.Range(0, Logs.Count);
        if (NewIndex == CurrentLogIndex)
            NewIndex++;
        if (NewIndex >= Logs.Count)
            NewIndex = 0;

        CurrentLogIndex = NewIndex;
        return Logs[NewIndex];
    }
}
