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

    [Header("훈련 화면 텍스트")]
    public GameObject Stamina_loadingText;
    public GameObject FlightPower_loadingText;
    public GameObject Balance_loadingText;
    public GameObject Agility_loadingText;

    public GameObject successText;
    public GameObject failText;

    [Header("하로 대사 출력용 TMP")]
    public TextMeshProUGUI dialogueText;

    [Header("로딩 중 스프라이트 이미지")]
    public Image loadingSpriteImage;
    public Sprite staminaSprite;
    public Sprite flightpowerSprite;
    public Sprite balanceSprite;
    public Sprite agilitySprite;

    private float timer = 0f;
    private bool isLoading = false;
    private StatType currentStat;

    float baseSpeed = 1f;
    float boostSpeed = 3f;

    public void StartTraining(StatType statType)
    {
        Debug.Log($"[TrainingLoader] StartTraining: {statType}");

        currentStat = statType;
        UpdateLoadingSprite(statType);

        loadingPanel.SetActive(true);
        progressBarFiller.fillAmount = 0f;

        // 모든 텍스트 끄기
        Stamina_loadingText.SetActive(false);
        FlightPower_loadingText.SetActive(false);
        Balance_loadingText.SetActive(false);
        Agility_loadingText.SetActive(false);

        // 선택한 스탯만 켜기
        switch (statType)
        {
            case StatType.Stamina_Stat:
                Stamina_loadingText.SetActive(true);
                break;
            case StatType.Flightpower_Stat:
                FlightPower_loadingText.SetActive(true);
                break;
            case StatType.Balance_Stat:
                Balance_loadingText.SetActive(true);
                break;
            case StatType.Agility_Stat:
                Agility_loadingText.SetActive(true);
                break;
        }

        successText.SetActive(false);
        failText.SetActive(false);

        timer = 0f;
        isLoading = true;
    }

    void UpdateLoadingSprite(StatType statType)
    {
        if (loadingSpriteImage == null)
        {
            Debug.LogError("[TrainingLoader] loadingSpriteImage가 연결안됨.");
            return;
        }

        switch (statType)
        {
            case StatType.Stamina_Stat:
                loadingSpriteImage.sprite = staminaSprite;
                break;
            case StatType.Flightpower_Stat:
                loadingSpriteImage.sprite = flightpowerSprite;
                break;
            case StatType.Balance_Stat:
                loadingSpriteImage.sprite = balanceSprite;
                break;
            case StatType.Agility_Stat:
                loadingSpriteImage.sprite = agilitySprite;
                break;
            default:
                Debug.LogWarning("[TrainingLoader] 알 수 없는 StatType: " + statType);
                break;
        }

        Debug.Log("[TrainingLoader] 로딩 이미지 변경됨: " + loadingSpriteImage.sprite?.name);
    }

    void Update()
    {
        if (!isLoading) return;

        float speedMultiplier = Input.GetMouseButton(0) ? boostSpeed : baseSpeed;

        timer += Time.deltaTime * speedMultiplier;
        progressBarFiller.fillAmount = timer / loadingTime;

        if (timer >= loadingTime)
        {
            CompleteTraining();
        }
    }

    void CompleteTraining()
    {
        isLoading = false;
        StartCoroutine(ShowResult());
    }

    IEnumerator ShowResult()
    {
        // 텍스트 끄기
        Stamina_loadingText.SetActive(false);
        FlightPower_loadingText.SetActive(false);
        Balance_loadingText.SetActive(false);
        Agility_loadingText.SetActive(false);

        float currentStamina = StatManager.Instance.currentStamina;
        float maxStamina = StatManager.Instance.maxStamina;
        float failureRate = 1f - (currentStamina / maxStamina);
        float rand = Random.value;

        Debug.Log($"[Training] 실패율: {failureRate:P1}, 랜덤값: {rand:F2}");

        float ratio = currentStamina / maxStamina;
        int staminaLevel = GetStaminaLevel(ratio);

        HpLogManager.instance.GetLogs(staminaLevel);
        string haroDialogue = HpLogManager.instance.GetSingleLog();
        dialogueText.text = haroDialogue;

        bool isSuccess = rand >= failureRate;
        if (isSuccess)
        {
            successText.SetActive(true);
            StatManager.Instance.IncreaseStat(currentStat);
        }
        else
        {
            failText.SetActive(true);
        }

        UIManager.Instance.UpdateStatUI();
        GameManager.Instance.UseTurn();

        yield return new WaitForSeconds(1.5f);
        loadingPanel.SetActive(false);
    }

    int GetStaminaLevel(float ratio)
    {
        if (ratio >= 0.8f) return 1;
        else if (ratio >= 0.6f) return 2;
        else if (ratio >= 0.4f) return 3;
        else if (ratio >= 0.2f) return 4;
        else return 5;
    }
}
