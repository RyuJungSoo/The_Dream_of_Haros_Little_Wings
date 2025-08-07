using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Dialogue_Setting : MonoBehaviour
{
    [SerializeField]
    Dialogue[] dialogues;
    private int Dialogue_idx = 0;
    private int Context_idx = 0;
    private string SpriteFolder_Name;
    public bool isTextTypeOver = true;
    private bool isBackgroundCoroutineStart = false;

    [SerializeField]
    private TextMeshProUGUI Name_UI;
    [SerializeField]
    private TextMeshProUGUI Context_UI;
    [SerializeField]
    private Image Sprite_UI;
    [SerializeField]
    private GameObject LoadingBackground;
    [SerializeField]
    private GameObject[] EndingUIObjects;

    /*int name_index = 0;
    int contexts_index = 0;
    
    string context;
    bool isFinish = true;*/


    public void ShowDialogue()//Dialogue[] p_dialogues // dialogue 데이터 가져오기
    {
        dialogues = GetComponent<InteractionEvent>().GetDialogues();
        SpriteFolder_Name = GetComponent<InteractionEvent>().GetName();
        //SpriteFolder_Name = GetComponent<InteractionEvent>().name;
    }

    // Start is called before the first frame update
    void Start()
    {
        ShowDialogue();
        //isTextTypeOver = false;
        //DialogueSetting();
        //GetUISprite("Scene_1_1");
    }

    private void Update()
    {
        if (Input.GetButtonUp("Submit") && isTextTypeOver)
            DialogueSetting();
    }

    public void indexInitialize() // 인덱스 초기화
    {
        Dialogue_idx = 0;
        Context_idx = 0;
    }

    private Sprite GetUISprite(string SpriteCode) // 전달된 SpriteCode에 해당하는 경로에서 스프라이트 가져오기
    {
        Sprite_UI.sprite = Resources.Load<Sprite>("Sprite/UI/TalkUISprite/" + SpriteFolder_Name + "/" + SpriteCode);
        Debug.Log("Sprite/UI/TalkUISprite/" + SpriteFolder_Name + "/" + SpriteCode);
        return Resources.Load<Sprite>("Sprite/UI/TalkUISprite/" + SpriteFolder_Name + "/" + SpriteCode);
    }

    public void TalkUIUpdate(int Dialogue_idx, int Context_idx) // 대화창 UI 업데이트
    {
        if (Name_UI)
        {
            if (dialogues[Dialogue_idx].name == "")
                return;

            Name_UI.text = dialogues[Dialogue_idx].name;
        }
        if (Context_UI)
        {

            StartCoroutine(TypeText(dialogues[Dialogue_idx].contexts[Context_idx]));
        }
        //Context_UI.text = dialogues[Dialogue_idx].contexts[Context_idx];

        if (Sprite_UI)
        {
            // 엔터 키 값이면 값이 없기 때문에 리턴
            if (dialogues[Dialogue_idx].Sprite_ID[Context_idx] == "\r" || dialogues[Dialogue_idx].Sprite_ID[Context_idx] == "")
                return;

            string SpriteCode = dialogues[Dialogue_idx].name + "_" + dialogues[Dialogue_idx].Sprite_ID[Context_idx];
            GetUISprite(SpriteCode.Trim()); // SpriteCode 마지막에 엔터 키가 있어서 지워야 함.
        }
    }

    public void DialogueSetting()
    {
        // 모든 Dialogue를 출력한 후
        if (Dialogue_idx >= dialogues.Length)
        {
            if (!isBackgroundCoroutineStart)
            {
                isBackgroundCoroutineStart = true;
                StartCoroutine(SetBackground());
            }
            return;
        }

        Context_UI.text = "";
        if (Dialogue_idx < dialogues.Length && Context_idx < dialogues[Dialogue_idx].contexts.Length)
        {
            isTextTypeOver = false;
            TalkUIUpdate(Dialogue_idx, Context_idx);
            //Context_idx++;
        }
    }

    IEnumerator TypeText(string texttoType)
    {
        foreach (char letter in texttoType)
        {
            if (Input.GetButton("Submit") && !isTextTypeOver)
            {
                Context_UI.text = texttoType;
                Debug.Log("Skip");
                yield return new WaitForSeconds(0.5f); // 문자열이 초기화되지 않는 경우를 피하기 위한 대기
                break;
            }

            Context_UI.text += letter;
            yield return new WaitForSeconds(0.1f);

        }

        Context_idx++;
        isTextTypeOver = true;

        if (Context_idx >= dialogues[Dialogue_idx].contexts.Length)
        {
            Dialogue_idx++;
            Context_idx = 0;
        }
    }
    IEnumerator SetBackground()
    {
        float timer = 0f;
        float duration = 1f;

        if (LoadingBackground != null)
        {
            Color color = LoadingBackground.GetComponent<Image>().color;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                color.a = Mathf.Clamp01(timer / duration);
                LoadingBackground.GetComponent<Image>().color = color;
                yield return null;
            }
        }

        string sceneName = SceneSettingManager.Instance.CurrentSceneName;

        if (sceneName == "MainMenu") // 현재 메인 메뉴이면 
            SceneSettingManager.Instance.ChangeScene("DialogueScene"); // 대화 씬으로 전환
        else if (sceneName == "DialogueScene" && !(SceneSettingManager.Instance.isStageAllClear())) // 현재 대화 씬이고 모든 스테이지가 클리어되지 않았을 때
            SceneSettingManager.Instance.ChangeScene("Raising_Stage"); // 육성 스테이지로 전환
        else if (sceneName == "DialogueScene" && SceneSettingManager.Instance.isStageAllClear()) // 현재 대화 씬이고 모든 스테이지를 클리어했을 때
        {
            // 현재 모든 스테이지를 클리어했을 때 게임 클리어 UI 켜는 기능이 들어가야 함.
            StartCoroutine(SetEndingUIObjects());
        }
    }

    IEnumerator SetEndingUIObjects()
    {
        float timer = 0f;
        float duration = 1f;

        foreach (GameObject gameObject in EndingUIObjects)
        {
            if (gameObject != null)
            {
                if (gameObject.GetComponent<TextMeshProUGUI>() == null && gameObject.GetComponent<Button>())
                {
                    gameObject.SetActive(true);
                    yield break;
                }

                Color color = gameObject.GetComponent<TextMeshProUGUI>().color;

                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    color.a = Mathf.Clamp01(timer / duration);
                    gameObject.GetComponent<TextMeshProUGUI>().color = color;
                    yield return null;
                }

                timer = 0f;
                yield return new WaitForSeconds(0.5f);
            }
        }

    }
}
