using UnityEngine;
using System.Collections;

public class SettingUIManager : MonoBehaviour
{
    public GameObject settingUI;

    // OnClick 이벤트에 연결
    public void Toggle()
    {
        if (!settingUI) return;
        StartCoroutine(ToggleNextFrame());
    }

    private IEnumerator ToggleNextFrame()
    {
        // 1프레임 대기
        yield return null;

        // 현재 활성 상태를 반전
        settingUI.SetActive(!settingUI.activeSelf);

        Debug.Log($"Toggle applied: activeSelf = {settingUI.activeSelf}");
    }
}
