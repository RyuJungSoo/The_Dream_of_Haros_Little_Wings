using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueParser : MonoBehaviour
{
    public Dialogue[] Parse(TextAsset CSV)
    {
        List<Dialogue> dialogueList = new List<Dialogue>(); // 대사 리스트 생성
        TextAsset csvData = CSV;
        //TextAsset csvData = Resources.Load<TextAsset>(_CSVFileName); // csv파일 가져옴. 

        string[] data = csvData.text.Split(new char[] { '\n' }); // 엔터 기준으로 쪼갬.

        for (int i = 1; i < data.Length;)
        {
            string[] row = data[i].Split(new char[] { ',' });

            Dialogue dialogue = new Dialogue(); // 대사 리스트 생성

            dialogue.name = row[1];
            //Debug.Log(row[1]);

            // 한 캐릭터의 모든 대사들을 contextList에 저장
            List<string> contextList = new List<string>();
            // 한 캐릭터의 모든 Sprite_ID들을 SpriteID_List에 저장
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
