using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;

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

    [Header("입력 잠금용(선택)")]
    [SerializeField] private Button restButton;        // 휴식 버튼 참조(있으면 연결)
    [SerializeField] private CanvasGroup controlsGroup; // 휴식/훈련 버튼들이 들어있는 부모 패널(있으면 연결)

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

                //  로딩 중 재호출 가드
        if (isLoading) return;

        //  버튼/컨테이너 입력 막기
        if (restButton) restButton.interactable = false;
        if (controlsGroup)
        {
            controlsGroup.interactable  = false; // 내부 Selectable 입력 차단
            controlsGroup.blocksRaycasts = true; // 마우스 클릭도 막기
        }

        //  스페이스/엔터로 Submit 재클릭 방지: 현재 선택된 UI 해제
        EventSystem.current?.SetSelectedGameObject(null);

        Debug.Log("[RestLoader] StartRest 호출됨");

        // 로딩 이미지가 클릭을 가로채지 않도록 차단
        if (loadingSpriteImage) loadingSpriteImage.raycastTarget = false;

        // 패널이랑 UI 세팅 중간에 리턴 하면 안됨
        HideAllTexts();                 // 모든 텍스트 비활성
        ColorUtility.TryParseHtmlString("#A6583F", out Color newColor);
        progressBarFiller.color = newColor;
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
        HideAllTexts();

        float currentStamina = StatManager.Instance.currentStamina;
        float maxStamina = StatManager.Instance.maxStamina;
        float ratio = currentStamina / maxStamina;

        int staminaLevel = GetStaminaLevel(ratio);
        HpLogManager.instance.GetLogs(staminaLevel);
        dialogueText.text = HpLogManager.instance.GetSingleLog();

        if (RestEndText != null) RestEndText.SetActive(true);

        if (GameManager.Instance != null)
            GameManager.Instance.UseTurn(); // 로딩 끝났을 때 1턴 소모

        yield return new WaitForSeconds(1.5f);

        // 완료 후 패널 끄기 + 텍스트 숨김
        loadingPanel.SetActive(false);
        HideAllTexts();

        //  입력 다시 켜기
        if (restButton) restButton.interactable = true;
        if (controlsGroup)
        {
            controlsGroup.interactable  = true;
            controlsGroup.blocksRaycasts = true; // 보통 true 유지(뒤 클릭 막기). 필요시 false로 조절
        }

        //  상태 리셋
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
}
