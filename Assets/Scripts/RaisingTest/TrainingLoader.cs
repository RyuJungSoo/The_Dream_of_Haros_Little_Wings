using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TrainingLoader : MonoBehaviour
{
    [Header("로딩 패널 및 진행 바")]
    public GameObject loadingPanel;
    public Image progressBarFiller;
    public float loadingTime = 3f;

    [Header("하단 화면 텍스트")]
    public GameObject Stamina_loadingText;
    public GameObject FlightPower_loadingText;
    public GameObject Balance_loadingText;
    public GameObject Agility_loadingText;

    [Header("결과 텍스트")]
    public GameObject successText;
    public GameObject failText;

    public GameObject RestEndText;

    [Header("대사 TMP")]
    public TextMeshProUGUI dialogueText;

    [Header("로딩용 스프라이트 이미지")]
    public Image loadingSpriteImage;
    public Sprite staminaSprite;
    public Sprite flightpowerSprite;
    public Sprite balanceSprite;
    public Sprite agilitySprite;

    private float timer = 0f;
    private bool isLoading = false;
    private StatType currentStat;

    // 진행 속도(마우스 좌클릭 시 가속)
    float baseSpeed = 1f;
    float boostSpeed = 3f;

    void Awake()
    {
        if (loadingPanel) loadingPanel.SetActive(false);
        if (loadingSpriteImage) loadingSpriteImage.raycastTarget = false; // 이미지가 클릭을 가로채지 않도록
        HideAllTexts();
    }

    public void StartTraining(StatType statType)
    {
        Debug.Log($"[TrainingLoader] StartTraining 호출: {statType}");
        currentStat = statType;

        // 패널/텍스트 초기화
        if (loadingPanel && !loadingPanel.activeSelf) loadingPanel.SetActive(true);
        HideAllTexts();

        // 선택된 스탯 텍스트만 켜기
        switch (statType)
        {
            case StatType.Stamina_Stat:     Stamina_loadingText?.SetActive(true); break;
            case StatType.Flightpower_Stat: FlightPower_loadingText?.SetActive(true); break;
            case StatType.Balance_Stat:     Balance_loadingText?.SetActive(true); break;
            case StatType.Agility_Stat:     Agility_loadingText?.SetActive(true); break;
        }

        // 이미지/게이지 세팅
        UpdateLoadingSprite(statType);
        if (progressBarFiller) progressBarFiller.fillAmount = 0f;

        // 진행 시작
        timer = 0f;
        isLoading = true;
    }

    void UpdateLoadingSprite(StatType statType)
    {
        if (!loadingSpriteImage)
        {
            Debug.LogError("[TrainingLoader] loadingSpriteImage가 없음");
            return;
        }

        switch (statType)
        {
            case StatType.Stamina_Stat:     loadingSpriteImage.sprite = staminaSprite; break;
            case StatType.Flightpower_Stat: loadingSpriteImage.sprite = flightpowerSprite; break;
            case StatType.Balance_Stat:     loadingSpriteImage.sprite = balanceSprite; break;
            case StatType.Agility_Stat:     loadingSpriteImage.sprite = agilitySprite; break;
            default:
                Debug.LogWarning("[TrainingLoader] 알 수 없는 StatType: " + statType);
                break;
        }

        Debug.Log("[TrainingLoader] 로딩 이미지 변경: " + loadingSpriteImage.sprite?.name);
    }

    void Update()
    {
        if (!isLoading) return;

        float speedMultiplier = Input.GetMouseButton(0) ? boostSpeed : baseSpeed;
        timer += Time.deltaTime * speedMultiplier;

        if (progressBarFiller)
            progressBarFiller.fillAmount = Mathf.Clamp01(timer / loadingTime);

        if (timer >= loadingTime)
            CompleteTraining();
    }

    void CompleteTraining()
    {
        isLoading = false;
        StartCoroutine(ShowResult());
    }

    IEnumerator ShowResult()
    {
        // 진행 텍스트 숨김
        HideAllTexts();

        // 실패율 계산
        float currentStamina = StatManager.Instance.currentStamina;
        float maxStamina = StatManager.Instance.maxStamina;
        float failureRate = 1f - (currentStamina / maxStamina);
        float rand = Random.value;
        Debug.Log($"[Training] 실패율: {failureRate:P1}, 랜덤값: {rand:F2}");

        // 대사 출력
        float ratio = currentStamina / maxStamina;
        int staminaLevel = GetStaminaLevel(ratio);
        HpLogManager.instance.GetLogs(staminaLevel);
        dialogueText.text = HpLogManager.instance.GetSingleLog();

        // 결과 표시 및 실제 스탯 반영

    bool isSuccess = rand >= failureRate;
    if (isSuccess)
    {
        successText.SetActive(true);
        StatManager.Instance.IncreaseStat(currentStat); // 내부에서 체력 감소까지 처리됨
    }
    else
    {
        failText.SetActive(true);
        // 실패 시에도 체력 소모 
        float cost = StatManager.Instance.GetStaminaCost(currentStat);
        StatManager.Instance.DecreaseStamina(cost);
    }


        UIManager.Instance.UpdateStatUI();
        GameManager.Instance.UseTurn();

        yield return new WaitForSeconds(1.5f);

        // 패널 닫기(필요 시 결과 텍스트는 함께 꺼짐)
        if (loadingPanel) loadingPanel.SetActive(false);
        HideAllTexts();
    }

    int GetStaminaLevel(float ratio)
    {
        if (ratio >= 0.8f) return 1;
        else if (ratio >= 0.6f) return 2;
        else if (ratio >= 0.4f) return 3;
        else if (ratio >= 0.2f) return 4;
        else return 5;
    }

    // 공통적으로 모든 텍스트 끄는 메서드
    private void HideAllTexts()
    {
        Stamina_loadingText?.SetActive(false);
        FlightPower_loadingText?.SetActive(false);
        Balance_loadingText?.SetActive(false);
        Agility_loadingText?.SetActive(false);
        successText?.SetActive(false);
        failText?.SetActive(false);
        RestEndText?.SetActive(false);
    }
}
