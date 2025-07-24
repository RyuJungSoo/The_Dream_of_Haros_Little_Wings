using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Dialogue_Setting : MonoBehaviour
{
    [SerializeField]
    Dialogue[] dialogues;
    private int Dialogue_idx = 0;
    private int Context_idx = 0;
    private string SpriteFolder_Name;

    [SerializeField]
    private TextMeshProUGUI Name_UI;
    [SerializeField]
    private TextMeshProUGUI Context_UI;
    [SerializeField]
    private Image Sprite_UI;
    

    /*int name_index = 0;
    int contexts_index = 0;
    
    string context;
    bool isFinish = true;*/


    public void ShowDialogue(Dialogue[] p_dialogues)
    {
        dialogues = p_dialogues;
        SpriteFolder_Name = GetComponent<InteractionEvent>().name;
    }

    // Start is called before the first frame update
    void Start()
    {
        ShowDialogue(GetComponent<InteractionEvent>().GetDialogues());
        //GetUISprite("Scene_1_1");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            DialogueSetting();
    }

    public void indexInitialize()
    {
        Dialogue_idx = 0;
        Context_idx = 0;
    }

    private Sprite GetUISprite(string SpriteCode)
    {
        //Debug.Log("Sprite/UI/TalkUISprite/" + SpriteFolder_Name + "/" + SpriteCode);
        //Debug.Log(Resources.Load<Sprite>("Sprite/UI/TalkUISprite/" + SpriteFolder_Name + "/" + SpriteCode) == null);
        Sprite_UI.sprite = Resources.Load<Sprite>("Sprite/UI/TalkUISprite/" + SpriteFolder_Name + "/" + SpriteCode);
        return Resources.Load<Sprite>("Sprite/UI/TalkUISprite/" + SpriteFolder_Name + "/" + SpriteCode);
    }

    public void TalkUISetting(int Dialogue_idx, int Context_idx)
    {
        if(Name_UI)
            Name_UI.text = dialogues[Dialogue_idx].name;
        if(Context_UI)
            Context_UI.text = dialogues[Dialogue_idx].contexts[Context_idx];
        if (Sprite_UI)
        {
            // 엔터 키 값이면 값이 없기 때문에 리턴
            if (dialogues[Dialogue_idx].Sprite_ID[Context_idx] == "\r")
                return;

            string SpriteCode = dialogues[Dialogue_idx].name + "_" + dialogues[Dialogue_idx].Sprite_ID[Context_idx];
            GetUISprite(SpriteCode.Trim()); // SpriteCode 마지막에 엔터 키가 있어서 지워야 함.
        }
    }

    public void DialogueSetting()
    {
        // 모든 Dialogue를 출력한 후
        if (Dialogue_idx >= dialogues.Length)
            return;

        if (Dialogue_idx < dialogues.Length && Context_idx < dialogues[Dialogue_idx].contexts.Length)
        {
            TalkUISetting(Dialogue_idx, Context_idx);
            Context_idx++;
        }

        if (Context_idx >= dialogues[Dialogue_idx].contexts.Length)
        {
            Dialogue_idx++;
            Context_idx = 0;
        }
    }
}
