using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TipLogManager : MonoBehaviour
{
    public static TipLogManager instance;
    private static TextAsset CSV;

    [SerializeField]
    private List<string> TipLogs;
    [SerializeField]
    private List<string> JokeLogs;
    private int CurrentLogIndex = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(this.gameObject);

        CSV = Resources.Load<TextAsset>("Dialogue/Tip & Joke Log");
        SetData();
    }


    // Start is called before the first frame update
    void Start()
    {
        int flag = Random.Range(0, 2);
        if (flag == 0)
            SetTip();
        else
            SetJoke();
    }

    void SetData()
    {
        TipLogs = new List<string>();
        JokeLogs = new List<string>();

        string[] data = CSV.text.Split(new char[] { '\n' });
        for (int i = 0; i < data.Length; i++)
        {
            string[] row = data[i].Split(new char[] { ',' });
            if (row[0].Contains("Tip"))
                TipLogs.Add(row[1].Replace('\'', ','));
            else if (row[0].Contains("Joke"))
                JokeLogs.Add(row[1].Replace('\'', ','));

        }
    }

    void SetTip()
    {
        int index = Random.Range(0, TipLogs.Count);
        GetComponent<TextMeshProUGUI>().text = TipLogs[index];
    }

    void SetJoke()
    {
        int index = Random.Range(0, JokeLogs.Count);
        GetComponent<TextMeshProUGUI>().text = JokeLogs[index];
    }

}
