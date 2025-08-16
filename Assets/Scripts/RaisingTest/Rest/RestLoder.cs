using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RestLoader : MonoBehaviour
{
    [Header("로딩 패널 및 진행 바")]
    public GameObject loadingPanel;
    public Image progressBarFiller;
    public float loadingTime = 3f;

    [Header("로딩 중 텍스트 (훈련 + 휴식 포함)")]
    public GameObject Rest_loadingText;
    public GameObject Stamina_loadingText;
    public GameObject FlightPower_loadingText;
    public GameObject Balance_loadingText;
    public GameObject Agility_loadingText;

    [Header("결과 텍스트")]
    public GameObject successText;
    public GameObject failText;

    [Header("로딩 중 결과 텍스트")]
    public GameObject RestEndText;

    [Header("하로 대사 출력용 TMP")]
    public TextMeshProUGUI dialogueText;

    [Header("로딩 중 스프라이트 이미지")]
    public Image loadingSpriteImage;
    public Sprite restSprite;

    private float timer = 0f;
    private bool isLoading = false;

    float baseSpeed = 1f;
    float boostSpeed = 3f;

    private void Awake()
    {
        // 시작 시 모든 텍스트 끄기
        HideAllTexts();
        loadingPanel.SetActive(false);
    }


    /// 휴식 시작
    public void StartRest()
    {
        // 이 스크립트가 비활성이면 바로 활성화
        if (!isActiveAndEnabled)
        {
            gameObject.SetActive(true);
            enabled = true;
            // 활성화된 같은 프레임 내에서도 계속 진행
        }

        Debug.Log("[RestLoader] StartRest 호출됨");

        // 로딩 이미지가 클릭을 가로채지 않도록 차단
        if (loadingSpriteImage) loadingSpriteImage.raycastTarget = false;

        // 패널이랑 UI 세팅 중간에 리턴 하면 안됨
        HideAllTexts();                 // 모든 텍스트 비활성
        Rest_loadingText?.SetActive(true); // 휴식중 텍스트만 활성

        if (!loadingPanel.activeSelf) loadingPanel.SetActive(true);
        progressBarFiller.fillAmount = 0f;
        loadingSpriteImage.sprite = restSprite;

        // 같은 클릭에서 바로 진행되도록 플래그 세팅
        timer = 0f;
        isLoading = true;
    }


    /// 모든 훈련/휴식 텍스트 + 결과 텍스트 끄기
    private void HideAllTexts()
    {
        Stamina_loadingText?.SetActive(false);
        FlightPower_loadingText?.SetActive(false);
        Balance_loadingText?.SetActive(false);
        Agility_loadingText?.SetActive(false);
        Rest_loadingText?.SetActive(false);
        successText?.SetActive(false);
        failText?.SetActive(false);
        RestEndText?.SetActive(false);
    }

    void Update()
    {
        if (!isLoading) return;

        float speedMultiplier = Input.GetMouseButton(0) ? boostSpeed : baseSpeed;

        timer += Time.deltaTime * speedMultiplier;
        progressBarFiller.fillAmount = timer / loadingTime;

        if (timer >= loadingTime)
        {
            CompleteRest();
        }
    }

    void CompleteRest()
    {
        isLoading = false;
        StartCoroutine(ShowRestLog());
    }

    IEnumerator ShowRestLog()
    {
        HideAllTexts(); // 로딩 텍스트 끄기

        float currentStamina = StatManager.Instance.currentStamina;
        float maxStamina = StatManager.Instance.maxStamina;
        float ratio = currentStamina / maxStamina;

        int staminaLevel = GetStaminaLevel(ratio);
        HpLogManager.instance.GetLogs(staminaLevel);
        string haroDialogue = HpLogManager.instance.GetSingleLog();
        dialogueText.text = haroDialogue;

        // 휴식 완료 텍스트 표시
        if (RestEndText != null)
            RestEndText.SetActive(true);

        if (GameManager.Instance != null)
            GameManager.Instance.UseTurn();

        yield return new WaitForSeconds(1.5f);

        // 완료 후 패널 끄기
        loadingPanel.SetActive(false);
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
}
