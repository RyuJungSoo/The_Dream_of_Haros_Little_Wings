using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSettingManager : MonoBehaviour
{
    public static SceneSettingManager Instance;
    public string PreviousSceneName { get; private set; } = ""; // 이전 씬 이름 저장용 변수
    public string CurrentSceneName { get; private set; } = ""; // 현재 씬 이름 저장용 변수
    public bool isStage1_Clear { get; private set; } = false; // 스테이지1 클리어 여부
    public bool isStage2_Clear { get; private set; } = false; // 스테이지2 클리어 여부

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // sceneLoaded 이벤트 등록
            CurrentSceneName = SceneManager.GetActiveScene().name;
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ChangeScene(string SceneName) // 씬 전환 함수
    {
        SceneManager.LoadScene(SceneName);
    }

    public void SetisStageClear(int stageNum, bool value) // 스테이지 클리어 여부 Set
    {
        if (stageNum == 1)
            isStage1_Clear = value;
        else if (stageNum == 2)
            isStage2_Clear = value;
        else
            return;
    }

    public bool isStageAllClear() // 모든 스테이지 클리어 여부 
    {
        if (isStage1_Clear && isStage2_Clear)
            return true;
        else
            return false;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) // 씬 전환 시 발생되는 이벤트
    {
        PreviousSceneName = CurrentSceneName;
        CurrentSceneName = scene.name;

        Debug.Log($"씬 변경됨: 이전 = {PreviousSceneName}, 현재 = {CurrentSceneName}");

        if (CurrentSceneName == "MainMenu") // 메인 메뉴 씬 전환 시
        {
            SoundManager.instance.PlayBGM(0, false);
            DatabaseManager.instance.SetDialogues(0);
        }
        else if (CurrentSceneName == "DialogueScene") // 대화 씬 전환 시
        {
            Dialogue_Setting dialogue_Setting = FindObjectOfType<Dialogue_Setting>();

            SoundManager.instance.PlayBGM(2, false);

            if (isStage1_Clear && !(isStageAllClear())) // 스테이지 1을 클리어한 경우
            {
                Debug.Log("OK");
                DatabaseManager.instance.SetDialogues(1); // 스테이지 2 입장 전 대화 진행
            }

            else if (isStage2_Clear && isStageAllClear()) // 이전 씬이 스테이지 2인 경우
            {
                DatabaseManager.instance.SetDialogues(2); // 스테이지 3 입장 전 대화 진행
            }
            else
                DatabaseManager.instance.SetDialogues(0); // 이외의 경우에는 스테이지 1 입장 전 대화 진행
            dialogue_Setting.ShowDialogue(); // 해당 대화 CSV 파일 세팅
            dialogue_Setting.DialogueSetting(); // 대화 시작

        }
        else if (CurrentSceneName == "Raising_Stage") // 육성 스테이지 씬 전환 시
        {
            SoundManager.instance.PlayBGM(3, false);
            SaveManager.Instance.LoadGame();
            UIManager.Instance.UpdateTurnText(GameManager.Instance.CurrentTurn); 
            UIManager.Instance.UpdateStaminaBar(); 
            UIManager.Instance.UpdateStatUI();
        }


        else if (CurrentSceneName == "Stage1") // 스테이지 1 씬 전환 시
            SoundManager.instance.PlayBGM(0, true);

        else if (CurrentSceneName == "Stage2") // 스테이지 2 씬 전환 시
            SoundManager.instance.PlayBGM(1, true);
    }

}
