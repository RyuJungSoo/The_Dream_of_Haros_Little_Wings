using UnityEngine;
using UnityEngine.UI;

public class MainMenuResetButton : MonoBehaviour
{
    [Header("패널")]
    [SerializeField] private GameObject panelRoot;

    [Header("리셋/나가기 버튼")]
    [SerializeField] private Button resetButton;
    [SerializeField] private Button rejectButton;

    private void Reset()
    {
        if (panelRoot == null) panelRoot = gameObject;
        if (resetButton == null) resetButton = transform.Find("Reset_Button")?.GetComponent<Button>();
        if (rejectButton == null) rejectButton = transform.Find("Reject_Button")?.GetComponent<Button>();
    }

    private void Awake()
    {
        if (panelRoot == null) panelRoot = gameObject;

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

    private void OnClickReset()
    {
        // JsonResetUtility에 있는 함수 호출
        JsonResetUtility.ResetSceneDataJson();
        Close();
    }

    private void OnClickReject()
    {
        Close();
    }
}
