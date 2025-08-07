using UnityEngine;

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

    /// 게임 종료 버튼 기능 (Quit 버튼에 연결)
    public void QuitGame()
    {
        Debug.Log("게임 종료");

#if UNITY_EDITOR
        // 에디터 모드에서는 플레이 종료
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 빌드 후 실행 시 게임 종료
        Application.Quit();
#endif
    }
}
