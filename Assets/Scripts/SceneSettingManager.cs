using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSettingManager : MonoBehaviour
{
    public static SceneSettingManager Instance;
    public string PreviousSceneName { get; private set; } = ""; // 이전 씬 이름 저장용 변수
    public string CurrentSceneName { get; private set; } = ""; // 현재 씬 이름 저장용 변수
    public bool isStageClear = false; // 비행 스태이지 클리어 여부

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) // 씬 전환 시 발생되는 이벤트
    {
        PreviousSceneName = CurrentSceneName;
        CurrentSceneName = scene.name;

        Debug.Log($"씬 변경됨: 이전 = {PreviousSceneName}, 현재 = {CurrentSceneName}");

        if (CurrentSceneName == "MainMenu")
        {
            SoundManager.instance.PlayBGM(0, false);
            DatabaseManager.instance.SetDialogues(0);
        }
        else if (CurrentSceneName == "DialogueScene")
        {
            Dialogue_Setting dialogue_Setting = FindObjectOfType<Dialogue_Setting>();

            SoundManager.instance.PlayBGM(2, false);
            DatabaseManager.instance.SetDialogues(0);
            dialogue_Setting.ShowDialogue();
            dialogue_Setting.DialogueSetting();

        }
        else if (CurrentSceneName == "Raising_Stage")
            SoundManager.instance.PlayBGM(3, false);
        else if (CurrentSceneName == "Stage1")
            SoundManager.instance.PlayBGM(0, true);
        else if (CurrentSceneName == "Stage2")
            SoundManager.instance.PlayBGM(1, true);
    }

}
