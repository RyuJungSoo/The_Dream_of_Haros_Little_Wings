using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueParser : MonoBehaviour
{
    public Dialogue[] Parse(TextAsset CSV)
    {
        List<Dialogue> dialogueList = new List<Dialogue>(); // ��� ����Ʈ ����
        TextAsset csvData = CSV;
        //TextAsset csvData = Resources.Load<TextAsset>(_CSVFileName); // csv���� ������. 

        string[] data = csvData.text.Split(new char[] { '\n' }); // ���� �������� �ɰ�.

        for (int i = 1; i < data.Length;)
        {
            string[] row = data[i].Split(new char[] { ',' });

            Dialogue dialogue = new Dialogue(); // ��� ����Ʈ ����

            dialogue.name = row[1];
            //Debug.Log(row[1]);

            // �� ĳ������ ��� ������ contextList�� ����
            List<string> contextList = new List<string>();
            // �� ĳ������ ��� Sprite_ID���� SpriteID_List�� ����
            List<string> SpriteID_List = new List<string>();

            do
            {
                contextList.Add(row[2].Replace('\'',','));
                SpriteID_List.Add(row[3]);
                //Debug.Log(row[2]);
                if (++i < data.Length)
                {
                    row = data[i].Split(new char[] { ',' });
                }
                else
                {
                    break;
                }
            } while (row[0].ToString() == "");


            dialogue.contexts = contextList.ToArray();
            dialogue.Sprite_ID = SpriteID_List.ToArray();
            dialogueList.Add(dialogue);
        }

        return dialogueList.ToArray();
    }
}
