using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingUIManager : MonoBehaviour
{
    [Header("설정 UI 패널")]
    public GameObject settingUI; // Setting_UI 패널 오브젝트

    private void Start()
    {
        // 게임 시작 시 설정창 비활성화
        if (settingUI != null)
            settingUI.SetActive(false);
    }


    /// 설정창 열기/닫기 토글 (설정 버튼에 연결)
    public void ToggleSettingUI()
    {
        if (settingUI == null) return;

        bool isActive = settingUI.activeSelf;
        settingUI.SetActive(!isActive);
    }

    /// 설정창 닫기 (Return 버튼에 연결)
    public void CloseSettingUI()
    {
        if (settingUI != null)
            settingUI.SetActive(false);
    }

    // 저장 후 타이틀로 이동하는 버튼
    public void SaveAndGoToTitle()
    {
        // 스탯 저장
        if (StatManager.Instance != null)
        {
            StatManager.Instance.SaveStatsToJson();
            Debug.Log("[SettingUIManager] 스탯 저장 완료");
        }

        // 씬 이동
        SceneManager.LoadScene("MainMenu");
    }

}