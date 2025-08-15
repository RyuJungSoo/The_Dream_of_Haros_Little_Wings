using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveGoMain : MonoBehaviour
{
    [Header("설정 UI 패널")]
    public GameObject settingUI;

    // 설정창 닫기 버튼
    public void CloseSettingUI()
    {
        if (settingUI != null) settingUI.SetActive(false);
    }

    // 저장 후 메인으로 이동
    public void SaveAndGoToTitle()
    {
        // 이어하기 상태로 저장 (턴/스탯/체력 전부 SaveManager가 세이브)
        if (SaveManager.Instance != null)
        {

            SaveManager.Instance.SaveGame();
        }


        // 설정창 닫기
        if (settingUI != null) settingUI.SetActive(false);

        // 메인 씬으로 이동
        SceneManager.LoadScene("MainMenu");
    }
}
