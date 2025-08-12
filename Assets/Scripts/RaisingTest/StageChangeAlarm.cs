using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class StageChangeAlarm : MonoBehaviour
{
    public static StageChangeAlarm Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject panel;               // Flightstage Alarm_UI
    [SerializeField] private TextMeshProUGUI messageText;    // Text (TMP)
    [SerializeField] private Button nextButton;              // Next_Button

    [Header("Message")]
    [TextArea] public string defaultMessage = "턴이 0입니다.\n비행 스테이지로 이동할까요?";

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 비어있으면 이름으로 자동 할당 시도
        AutoAssignIfNull();

        if (panel != null) panel.SetActive(false);

        // 버튼 와이어링
        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(() =>
            {
                Hide();
                StartCoroutine(WaitAndRouteNextStage());
            });
        }
    }

    private void AutoAssignIfNull()
    {
        if (panel == null)
        {
            // 이 스크립트를 패널에 붙였으면 그냥 자기 자신
            // 아니면 씬에서 이름으로 찾아보기
            var t = transform.Find("Flightstage Alarm_UI");
            if (t != null) panel = t.gameObject;
            else
            {
                var found = GameObject.Find("Flightstage Alarm_UI");
                if (found != null) panel = found;
            }
        }

        if (panel != null)
        {
            if (messageText == null)
            {
                var t = panel.transform.Find("Text (TMP)");
                if (t != null) messageText = t.GetComponent<TextMeshProUGUI>();
            }
            if (nextButton == null)
            {
                var t = panel.transform.Find("Next_Button");
                if (t != null) nextButton = t.GetComponent<Button>();
            }
        }
    }

    // 팝업 띄움
    public void PromptAndRoute(string customMessage = null)
    {
        if (panel == null)
        {
            Debug.LogError("[FlightstageAlarmRouter] panel이 비어 있습니다. Flightstage Alarm_UI를 연결하세요.", this);
            return;
        }
        if (messageText != null)
            messageText.text = string.IsNullOrEmpty(customMessage) ? defaultMessage : customMessage;

        panel.SetActive(true);

        // 혹시 CanvasGroup으로 숨겨졌다면 보정
        var cg = panel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
    }

    private void Hide()
    {
        if (panel != null) panel.SetActive(false);
    }

    private IEnumerator WaitAndRouteNextStage()
    {
        for (int i = 0; i < 10; i++)
        {
            if (SceneSettingManager.Instance != null) break;
            yield return null;
        }

        var ssm = SceneSettingManager.Instance ?? Object.FindObjectOfType<SceneSettingManager>();
        if (ssm == null)
        {
            Debug.LogWarning("[FlightstageAlarmRouter] SceneSettingManager 없음 → \"Stage1\" 폴백");
            SceneManager.LoadScene("Stage1");
            yield break;
        }

        if (!ssm.isStage1_Clear)       ssm.ChangeScene("Stage1");
        else if (!ssm.isStage2_Clear)  ssm.ChangeScene("Stage2");
        else                           Debug.Log("[FlightstageAlarmRouter] 모든 스테이지 클리어");
    }
}
