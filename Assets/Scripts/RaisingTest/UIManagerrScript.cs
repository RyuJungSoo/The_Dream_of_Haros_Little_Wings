using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("스탯 수치 텍스트")]
    public TextMeshProUGUI staminaValueText;           // 체력 스탯 수치 표시 텍스트
    public TextMeshProUGUI flightpowerValueText;       // 비행력 스탯 수치 표시 텍스트
    public TextMeshProUGUI balanceValueText;           // 균형감 스탯 수치 표시 텍스트
    public TextMeshProUGUI agilityValueText;           // 민첩성 스탯 수치 표시 텍스트

    [Header("스탯 등급 텍스트")]
    public TextMeshProUGUI staminaGradeText;           // 체력 등급 표시 텍스트
    public TextMeshProUGUI flightpowerGradeText;       // 비행력 등급 표시 텍스트
    public TextMeshProUGUI balanceGradeText;           // 균형감 등급 표시 텍스트
    public TextMeshProUGUI agilityGradeText;           // 민첩성 등급 표시 텍스트

    [Header("스탯 증가 버튼")]
    public Button staminaButton;                       // 체력 훈련 버튼
    public Button flightpowerButton;                   // 비행력 훈련 버튼
    public Button balanceButton;                       // 균형감 훈련 버튼
    public Button agilityButton;                       // 민첩성 훈련 버튼

    [Header("선택 표시 오브젝트")]
    public GameObject chosenCheckStamina;              // 체력 선택 표시 UI
    public GameObject chosenCheckFlight;               // 비행력 선택 표시 UI
    public GameObject chosenCheckBalance;              // 균형감 선택 표시 UI
    public GameObject chosenCheckAgility;              // 민첩성 선택 표시 UI

    [Header("휴식 버튼")]
    public Button restButton;                          // 휴식 버튼

    [Header("턴수 확인")]
    public TextMeshProUGUI turnText;                   // 현재 턴수 텍스트

    [Header("체력 UI")]
    public Image staminaBarFiller;                     // 체력 게이지 바
    public float recoveryAmount = 30f;                 // 휴식 시 회복 체력

    [Header("주/보조 스탯 예상 텍스트")]
    public TextMeshProUGUI staminaMainText;            // 체력 주 스탯 예상 텍스트
    public TextMeshProUGUI staminaSubText;             // 체력 보조 스탯 예상 텍스트
    public TextMeshProUGUI flightpowerMainText;        // 비행력 주 스탯 예상 텍스트
    public TextMeshProUGUI flightpowerSubText;         // 비행력 보조 스탯 예상 텍스트
    public TextMeshProUGUI flightpowerSubText2;  
    public TextMeshProUGUI balanceMainText;            // 균형감 주 스탯 예상 텍스트
    public TextMeshProUGUI balanceSubText;             // 균형감 보조 스탯 예상 텍스트
    public TextMeshProUGUI agilityMainText;            // 민첩성 주 스탯 예상 텍스트
    public TextMeshProUGUI agilitySubText;             // 민첩성 보조 스탯 예상 텍스트

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 버튼별 마우스 오버 이벤트 설정 (주/보조 예상 스탯 표시)
        AddHoverEvents(staminaButton, chosenCheckStamina, staminaMainText, staminaSubText, "Stamina");
        AddHoverEventsFlightPower(flightpowerButton, chosenCheckFlight,flightpowerMainText, flightpowerSubText, flightpowerSubText2);

        AddHoverEvents(balanceButton, chosenCheckBalance, balanceMainText, balanceSubText, "Balance");
        AddHoverEvents(agilityButton, chosenCheckAgility, agilityMainText, agilitySubText, "Agility");

        // 버튼 클릭 리스너 등록
        staminaButton.onClick.AddListener(() => OnClickIncreaseStat(StatType.Stamina_Stat));
        flightpowerButton.onClick.AddListener(() => OnClickIncreaseStat(StatType.Flightpower_Stat));
        balanceButton.onClick.AddListener(() => OnClickIncreaseStat(StatType.Balance_Stat));
        agilityButton.onClick.AddListener(() => OnClickIncreaseStat(StatType.Agility_Stat));
        restButton.onClick.AddListener(OnClickRest);

        HideAllChosenChecks(); // 시작 시 UI 숨기기
    }

    // 마우스 오버 시 예상 스탯 UI 표시 처리
    private void AddHoverEvents(Button button, GameObject chosen, TextMeshProUGUI mainText, TextMeshProUGUI subText, string statName)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null) trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => {
            ShowChosenCheck(chosen);
            (string main, string sub) = StatManager.Instance.GetMainAndSubStatText(statName);
            mainText.text = $"<color=#FF0000>{main}</color>"; // 주 스탯 텍스트 빨간색
            subText.text = $"<color=#FFBA00>{sub}</color>";   // 보조 스탯 텍스트 주황색
            mainText.gameObject.SetActive(true);
            subText.gameObject.SetActive(true);
        });
        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => {
            HideAllChosenChecks();
            mainText.gameObject.SetActive(false);
            subText.gameObject.SetActive(false);
        });
        trigger.triggers.Add(entryExit);
    }

    private void AddHoverEventsFlightPower(Button button, GameObject chosen,
                                            TextMeshProUGUI mainText,
                                            TextMeshProUGUI subText1,
                                            TextMeshProUGUI subText2)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null) trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) =>
        {
            ShowChosenCheck(chosen);

            // ? 여기를 고정값 대신 StatManager에서 가져오도록 변경
            int main = StatManager.Instance.GetExpectedMainIncrease("Flightpower");
            int sub = StatManager.Instance.GetExpectedSubIncrease("Flightpower");

            mainText.text = $"<color=#FF0000>+{main}</color>";
            subText1.text = $"<color=#FFBA00>+{sub}</color>";
            subText2.text = $"<color=#FFBA00>+{sub}</color>"; // 같은 보조 스탯이면 동일값 사용

            mainText.gameObject.SetActive(true);
            subText1.gameObject.SetActive(true);
            subText2.gameObject.SetActive(true);
        });
        trigger.triggers.Add(entryEnter);

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) =>
        {
            HideAllChosenChecks();
            mainText.gameObject.SetActive(false);
            subText1.gameObject.SetActive(false);
            subText2.gameObject.SetActive(false);
        });
        trigger.triggers.Add(entryExit);
    }


    // 선택 효과 및 실패확률 UI 표시
    public void ShowChosenCheck(GameObject chosen)
    {
        HideAllChosenChecks();
        if (chosen != null)
        {
            chosen.SetActive(true);

            Transform parent = chosen.transform.parent;
            if (parent != null)
            {
                var failChance = parent.Find("Fail_Chance");
                if (failChance != null)
                {
                    var failUI = failChance.GetComponent<Fail_Chance_UI>();
                    if (failUI != null)
                    {
                        int failureRate = Mathf.Clamp(Mathf.RoundToInt(100f - StatManager.Instance.currentStamina), 0, 100);
                        failUI.UIUpdate(failureRate);
                        failChance.gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    // 선택 효과 및 예상치 UI 숨기기
    public void HideAllChosenChecks()
    {
        chosenCheckStamina.SetActive(false);
        chosenCheckFlight.SetActive(false);
        chosenCheckBalance.SetActive(false);
        chosenCheckAgility.SetActive(false);

        HideFailChance(chosenCheckStamina);
        HideFailChance(chosenCheckFlight);
        HideFailChance(chosenCheckBalance);
        HideFailChance(chosenCheckAgility);
    }

    // 실패 확률 UI 숨기기
    private void HideFailChance(GameObject chosen)
    {
        if (chosen == null) return;
        Transform parent = chosen.transform.parent;
        if (parent != null)
        {
            var failChance = parent.Find("Fail_Chance");
            if (failChance != null)
                failChance.gameObject.SetActive(false);
        }
    }

    public void OnClickIncreaseStat(StatType type)
    {
        if (!GameManager.Instance.IsTurnAvailable()) return;

        // 1. 훈련 예상값 먼저 생성
        StatManager.Instance.GenerateExpectedStatIncreases();

        // 2. 훈련 실패율 및 성공 여부 판정
        int failureRate = StatManager.Instance.GetTrainingFailureRate();
        int roll = Random.Range(0, 100); // 0~99
        float staminaCost = StatManager.Instance.GetStaminaCost(type);

        Debug.Log($"[훈련시도] 실패율: {failureRate}%, 랜덤값: {roll}");

        // 3. 체력은 무조건 먼저 감소
        StatManager.Instance.DecreaseStamina(staminaCost);

        // 4. 실패
        if (roll < failureRate)
        {
            Debug.LogWarning($"[훈련 실패] {type} 훈련이 실패했습니다. 스탯 증가 없음!");
        }
        else
        {
            // 5. 성공
            Debug.Log($"[훈련 성공] {type} 훈련에 성공했습니다!");
            StatManager.Instance.IncreaseStat(type);
        }

        // 6. 턴 소모 및 UI 갱신
        GameManager.Instance.UseTurn();
        UIManager.Instance.UpdateStatUI();
    }


    // 휴식 버튼 클릭 시 체력 회복 처리
    public void OnClickRest()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsTurnAvailable())
        {
            StatManager.Instance.currentStamina += recoveryAmount;
            if (StatManager.Instance.currentStamina > StatManager.Instance.maxStamina)
                StatManager.Instance.currentStamina = StatManager.Instance.maxStamina;

            GameManager.Instance.UseTurn();
            UpdateStatUI();
            HideAllChosenChecks();
        }
    }

    // 전체 스탯 UI 업데이트
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

    // 턴 수 UI 업데이트
    public void UpdateTurnText(int turn)
    {
        turnText.text = $"{turn} 턴";
    }

    // 체력 바 업데이트
    public void UpdateStaminaBar()
    {
        if (staminaBarFiller != null)
        {
            float fillAmount = StatManager.Instance.currentStamina / StatManager.Instance.maxStamina;
            staminaBarFiller.fillAmount = Mathf.Clamp01(fillAmount);
        }
    }
}