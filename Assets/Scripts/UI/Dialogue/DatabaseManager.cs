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

            // ��� ù��°�� �ε��� 0���� ���۵ǰ� ����� ���������� �����Ƿ�
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
