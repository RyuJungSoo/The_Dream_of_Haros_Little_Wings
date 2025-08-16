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
#if UNITY_2023_1_OR_NEWER
        sceneSettingSaver = sceneSettingSaver ?? FindFirstObjectByType<SceneSettingSaver>(FindObjectsInactive.Include);
#else
        sceneSettingSaver = sceneSettingSaver ?? FindObjectOfType<SceneSettingSaver>(true);
#endif
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

    public void Open()  => panelRoot?.SetActive(true);
    public void Close() => panelRoot?.SetActive(false);

    public void OnClickReset()
    {
        SetInteractable(false);
        SaveManager.Instance.ResetGame();

        Debug.Log("[MainMenuResetButton] 전체 데이터 리셋 완료 (스탯 0 / 체력 풀 / 턴 풀)");
        Close();
        SetInteractable(true);
    }

    public void OnClickReject()
    {
        Close();
    }

    private void SetInteractable(bool v)
    {
        if (resetButton)  resetButton.interactable  = v;
        if (rejectButton) rejectButton.interactable = v;
    }
}
