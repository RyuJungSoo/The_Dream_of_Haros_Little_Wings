using UnityEngine;
using UnityEngine.UI;

public class MainMenuResetButton : MonoBehaviour
{
    [Header("패널")]
    [SerializeField] private GameObject panelRoot;

    [Header("리셋/나가기 버튼")]
    [SerializeField] private Button resetButton;
    [SerializeField] private Button rejectButton;

    private SceneSettingSaver sceneSettingSaver;

    private void Reset()
    {
        if (panelRoot == null) panelRoot = gameObject;
        if (resetButton == null) resetButton = transform.Find("Reset_Button")?.GetComponent<Button>();
        if (rejectButton == null) rejectButton = transform.Find("Reject_Button")?.GetComponent<Button>();
    }

    private void Awake()
    {
        if (sceneSettingSaver == null)
#if UNITY_2023_1_OR_NEWER
            sceneSettingSaver = FindFirstObjectByType<SceneSettingSaver>(FindObjectsInactive.Include);
#else
            sceneSettingSaver = FindObjectOfType<SceneSettingSaver>(true);
#endif

        // (선택) 자동 연결
        if (resetButton != null)
        {
            resetButton.onClick.RemoveListener(OnClickReset);
            resetButton.onClick.AddListener(OnClickReset);
        }
        if (rejectButton != null)
        {
            rejectButton.onClick.RemoveListener(OnClickReject);
            rejectButton.onClick.AddListener(OnClickReject);
        }
    }

    public void Open()  => panelRoot.SetActive(true);
    public void Close() => panelRoot.SetActive(false);

    // === 초기화/리셋 ===
    public void OnClickReset()   // <- public, 매개변수 없음
    {
        if (sceneSettingSaver != null)
        {
            try
            {
                sceneSettingSaver.ResetSave();
                Debug.Log("[MainMenuResetButton] SceneSettingSaver.ResetSave 호출 완료");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[MainMenuResetButton] ResetSave 예외: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("[MainMenuResetButton] SceneSettingSaver를 찾지 못했습니다. (MainMenu 씬 배치/이름 확인)");
        }

        JsonResetUtility.ResetStatsHpTurnsJsonAndState();
        Close();
    }

    public void OnClickReject()   // <- public, 매개변수 없음
    {
        Close();
    }
}
