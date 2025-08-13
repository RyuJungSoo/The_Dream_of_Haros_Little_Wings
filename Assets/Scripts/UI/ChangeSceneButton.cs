using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneButton : MonoBehaviour
{
    [SerializeField]
    private string SceneName;
    [SerializeField]
    private Dialogue_Setting dialogue_setting;
    [SerializeField]
    private bool isUsable;


    private void Start()
    {
        if (dialogue_setting == null)
            return;

        if (SceneSettingManager.Instance != null)
        {
            if (SceneSettingManager.Instance.CurrentSceneName == "DialogueScene" && SceneSettingManager.Instance.isStageAllClear())
            {
                Debug.Log("OK");
                isUsable = false;
            }
        }
    }

    public void ChangeScene()
    {

        if (SceneSettingManager.Instance != null && SceneSettingManager.Instance.CurrentSceneName == "DialogueScene" && SceneSettingManager.Instance.isStageAllClear()) // 현재 대화 씬이고 모든 스테이지가 클리어되었을 때
        {
            if(isUsable)
                SceneManager.LoadScene(SceneName);

            if (dialogue_setting != null)
            {
                dialogue_setting.SetEndingUIObjects_Function();
                isUsable = true;
            }
        }
        else if (isUsable)
            SceneManager.LoadScene(SceneName);
    }
}
