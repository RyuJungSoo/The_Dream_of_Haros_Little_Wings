using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;
    [SerializeField]
    private TextAsset CSV;

    //[SerializeField] string csv_FileName;

    Dictionary<int, Dialogue> dialogueDic = new Dictionary<int, Dialogue>();
    public static bool isFinish = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DialogueParser theParser = GetComponent<DialogueParser>();
            Dialogue[] dialogues = theParser.Parse(CSV);

            // 대사 첫번째를 인덱스 0부터 시작되게 만들면 직관적이지 않으므로
            for (int i = 0; i < dialogues.Length; i++)
            {
                dialogueDic.Add(i + 1, dialogues[i]);
            }
            isFinish = true;
        }
    }

    public Dialogue[] GetDialogues(int _StartNum, int _EndNum)
    {
        List<Dialogue> dialogeList = new List<Dialogue>();
        for (int i = 0; i <= _EndNum - _StartNum; i++)
        {
            dialogeList.Add(dialogueDic[_StartNum + i]);
        }
        return dialogeList.ToArray();
    }
}
