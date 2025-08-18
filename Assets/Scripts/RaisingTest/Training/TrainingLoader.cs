using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; //  포커스 해제용
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

    //  입력 잠금용(선택)
    [Header("입력 잠금용(선택)")]
    [SerializeField] private Button[] trainingButtons; // 개별 훈련 버튼들 넣어두면 로딩 중 비활성
    [SerializeField] private CanvasGroup controlsGroup; // 훈련 버튼들이 들어있는 부모 패널(있으면)

    private float timer = 0f;
    private bool isLoading = false;
    private StatType currentStat;

    // 진행 속도(마우스 좌클릭 시 가속)
    float baseSpeed = 1f;
    float boostSpeed = 3f;

    void Awake()
    {
        if (loadingPanel) loadingPanel.SetActive(false);
        if (loadingSpriteImage) loadingSpriteImage.raycastTarget = false;
        HideAllTexts();
    }

    public void StartTraining(StatType statType)
    {

        // 이 스크립트가 비활성이면 바로 활성화
        if (!isActiveAndEnabled)
        {
            gameObject.SetActive(true);
            enabled = true;
            // 활성화된 같은 프레임 내에서도 계속 진행
        }

        //  로딩 중 중복 호출 가드
        if (isLoading) return;

        //  버튼/컨테이너 입력 잠금
        SetInputsInteractable(false);
        //  Submit(스페이스/엔터)로 재실행 방지: 현재 선택된 UI 해제
        EventSystem.current?.SetSelectedGameObject(null);

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
            SoundManager.instance.PlaySFX(12, 0);
            successText?.SetActive(true);
            StatManager.Instance.IncreaseStat(currentStat); // 내부에서 체력 감소 포함
        }
        else
        {
            SoundManager.instance.PlaySFX(13, 0);
            failText?.SetActive(true);
            float cost = StatManager.Instance.GetStaminaCost(currentStat);
            StatManager.Instance.DecreaseStamina(cost);
        }

        UIManager.Instance.UpdateStatUI();
        GameManager.Instance.UseTurn();

        yield return new WaitForSeconds(1.5f);

        // 패널 닫기
        if (loadingPanel) loadingPanel.SetActive(false);
        HideAllTexts();

        //  입력 다시 켜기 + 상태 리셋
        SetInputsInteractable(true);
        isLoading = false;
    }

    int GetStaminaLevel(float ratio)
    {
        if (ratio >= 0.8f) return 1;
        else if (ratio >= 0.6f) return 2;
        else if (ratio >= 0.4f) return 3;
        else if (ratio >= 0.2f) return 4;
        else return 5;
    }

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

    //  공통 입력 잠금/해제
    private void SetInputsInteractable(bool enable)
    {
        if (trainingButtons != null)
        {
            foreach (var b in trainingButtons)
            {
                if (b) b.interactable = enable;
            }
        }
        if (controlsGroup)
        {
            controlsGroup.interactable = enable;   // 내부 Selectable 입력
            controlsGroup.blocksRaycasts = true;   // 마우스 클릭 차단 유지(필요시 false로 조정)
        }
    }
}
