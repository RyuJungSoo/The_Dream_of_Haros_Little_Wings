using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("스탯 수치 텍스트")]
    public TextMeshProUGUI staminaValueText;
    public TextMeshProUGUI flightpowerValueText;
    public TextMeshProUGUI balanceValueText;
    public TextMeshProUGUI agilityValueText;

    [Header("스탯 등급 텍스트")]
    public TextMeshProUGUI staminaGradeText;
    public TextMeshProUGUI flightpowerGradeText;
    public TextMeshProUGUI balanceGradeText;
    public TextMeshProUGUI agilityGradeText;

    [Header("스탯 증가 버튼")]
    public Button staminaButton;
    public Button flightpowerButton;
    public Button balanceButton;
    public Button agilityButton;

    [Header("휴식 버튼")]
    public Button restButton;

    [Header("턴수 확인")]
    public TextMeshProUGUI turnText;

    [Header("체력 UI")]
    public Image staminaBarFiller;
    public float staminaCostPerTraining = 10f;
    public float recoveryAmount = 20f;

    [Header("실패율 UI")]
    public GameObject Stamina_failureRatePanel;
    public GameObject FlightSpeed_failureRatePanel;
    public GameObject Balance_failureRatePanel;
    public GameObject Aglilty_failureRatePanel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        staminaButton.onClick.AddListener(() => OnClickIncreaseStat(StatType.Stamina_Stat));
        flightpowerButton.onClick.AddListener(() => OnClickIncreaseStat(StatType.Flightpower_Stat));
        balanceButton.onClick.AddListener(() => OnClickIncreaseStat(StatType.Balance_Stat));
        agilityButton.onClick.AddListener(() => OnClickIncreaseStat(StatType.Agility_Stat));
        restButton.onClick.AddListener(OnClickRest);

        AddHoverEvents(staminaButton, Stamina_failureRatePanel);
        AddHoverEvents(flightpowerButton, FlightSpeed_failureRatePanel);
        AddHoverEvents(balanceButton, Balance_failureRatePanel);
        AddHoverEvents(agilityButton, Aglilty_failureRatePanel);

        UpdateStatUI();
    }

    private void AddHoverEvents(Button button, GameObject panel)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null) trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => { ShowFailureRate(panel); });
        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => { HideAllFailureRates(); });
        trigger.triggers.Add(entryExit);
    }

    public void ShowFailureRate(GameObject panel)
    {
        float rate = CalculateFailureRate();
        int rateInt = Mathf.Clamp(Mathf.RoundToInt(rate), 0, 100);
        Fail_Chance_UI ui = panel.GetComponent<Fail_Chance_UI>();
        if (ui != null)
        {
            ui.UIUpdate(rateInt);
            panel.SetActive(true);
        }
    }

    public void HideAllFailureRates()
    {
        Stamina_failureRatePanel.SetActive(false);
        FlightSpeed_failureRatePanel.SetActive(false);
        Balance_failureRatePanel.SetActive(false);
        Aglilty_failureRatePanel.SetActive(false);
    }

    private float CalculateFailureRate()
    {
        float stamina = StatManager.Instance.currentStamina;
        float failureRate = 100f - stamina;
        return Mathf.Clamp(failureRate, 0f, 100f);
    }

    public void OnClickIncreaseStat(StatType type)
    {
        if (!GameManager.Instance.IsTurnAvailable())
        {
            Debug.LogWarning("턴이 부족하여 훈련할 수 없습니다.");
            return;
        }

        if (StatManager.Instance.currentStamina < staminaCostPerTraining)
        {
            Debug.LogWarning("체력이 부족하여 훈련할 수 없습니다.");
            return;
        }

        StatManager.Instance.currentStamina -= staminaCostPerTraining;
        StatManager.Instance.IncreaseStat(type);
        GameManager.Instance.UseTurn();
        UpdateStatUI();
    }

    public void OnClickRest()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsTurnAvailable())
        {
            StatManager.Instance.ResetStats();

            StatManager.Instance.currentStamina += recoveryAmount;
            if (StatManager.Instance.currentStamina > StatManager.Instance.maxStamina)
                StatManager.Instance.currentStamina = StatManager.Instance.maxStamina;

            GameManager.Instance.UseTurn();
            UpdateStatUI();
        }
        else
        {
            Debug.LogWarning("턴이 부족하여 휴식할 수 없습니다.");
        }
    }

    public void UpdateStatUI()
    {
        staminaValueText.text = $"{StatManager.Instance.Stamina_Stat} / 180";
        flightpowerValueText.text = $"{StatManager.Instance.Flightpower_Stat} / 180";
        balanceValueText.text = $"{StatManager.Instance.Balance_Stat} / 180";
        agilityValueText.text = $"{StatManager.Instance.Agility_Stat} / 180";

        staminaGradeText.text = StatManager.Instance.GetStaminaGrade();
        flightpowerGradeText.text = StatManager.Instance.GetFlightpowerGrade();
        balanceGradeText.text = StatManager.Instance.GetBalanceGrade();
        agilityGradeText.text = StatManager.Instance.GetAgilityGrade();

        UpdateTurnText(GameManager.Instance.GetCurrentTurn());
        UpdateStaminaBar();
    }

    public void UpdateTurnText(int turn)
    {
        turnText.text = $"{turn} 턴";
    }

    public void UpdateStaminaBar()
    {
        if (staminaBarFiller != null)
        {
            float fillAmount = StatManager.Instance.currentStamina / StatManager.Instance.maxStamina;
            staminaBarFiller.fillAmount = Mathf.Clamp01(fillAmount);
        }
    }
}
