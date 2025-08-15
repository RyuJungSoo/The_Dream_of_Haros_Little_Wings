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

    // 턴수가 0일 때 딜레이 후 팝업
    [Header("Turn 0 Prompt Delay")]
    [SerializeField] private float depletedPromptDelay = 1.5f;   // 00초 조절 
    [Tooltip("이 오브젝트가 활성화돼 있는 동안은 알림을 보류합니다. (선택) 로딩 패널을 연결하세요")]
    [SerializeField] private GameObject waitWhileActive;       // 선택: 로딩패널 등
    private Coroutine delayedPromptCo;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

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

    // 즉시 팝업
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

        var cg = panel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
    }

    // 딜레이 버전 호출
    public void PromptWhenTurnsDepleted()
    {
        PromptWhenTurnsDepleted(depletedPromptDelay);
    }

    public void PromptWhenTurnsDepleted(float delaySeconds)
    {
        if (delayedPromptCo != null) StopCoroutine(delayedPromptCo);
        delayedPromptCo = StartCoroutine(CoPromptAfterDelay(delaySeconds));
    }

    private IEnumerator CoPromptAfterDelay(float delaySeconds)
    {
        // waitWhileActive가 켜져 있으면 꺼질 때까지 대기
        if (waitWhileActive != null)
        {
            while (waitWhileActive.activeInHierarchy) yield return null;
        }

        yield return new WaitForSecondsRealtime(delaySeconds);

        // 대기 중에 턴이 회복되었으면 취소
        if (GameManager.Instance != null && GameManager.Instance.IsTurnAvailable())
        {
            delayedPromptCo = null;
            yield break;
        }

        // 이미 창이 떠 있으면 취소
        if (panel != null && panel.activeSelf)
        {
            delayedPromptCo = null;
            yield break;
        }

        PromptAndRoute();
        delayedPromptCo = null;
    }

    public void CancelPendingPrompt()
    {
        if (delayedPromptCo != null)
        {
            StopCoroutine(delayedPromptCo);
            delayedPromptCo = null;
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
            SaveManager.Instance.SaveGame(); // 저장 
            SceneManager.LoadScene("Stage1");
            yield break;
        }

        if (!ssm.isStage1_Clear)
        {
            SaveManager.Instance.SaveGame(); // 저장 
            ssm.ChangeScene("Stage1");
        }
        else if (!ssm.isStage2_Clear)
        {
            SaveManager.Instance.SaveGame(); // 저장 
            ssm.ChangeScene("Stage2");
        }
        else Debug.Log("[FlightstageAlarmRouter] 모든 스테이지 클리어");
    }
}
