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

    [Header("로딩 화면 텍스트")]
    public GameObject loadingText;

    [Header("하로 대사 출력용 TMP")]
    public TextMeshProUGUI dialogueText;

    [Header("로딩 중 스프라이트 이미지")]
    public Image loadingSpriteImage;
    public Sprite restSprite;

    [Header("남은 훈련 텍스트 잔상 제거용")]
    public GameObject successText;
    public GameObject failText;
    private float timer = 0f;
    private bool isLoading = false;

    float baseSpeed = 1f;
    float boostSpeed = 3f;

    public void StartRest()
    {
        Debug.Log("[RestLoader] StartRest 호출됨");

        // 이전 훈련의 성공/실패 텍스트 잔상 제거용
        if (successText != null) successText.SetActive(false);
        if (failText != null) failText.SetActive(false);

        loadingPanel.SetActive(true);
        loadingText.SetActive(true);
        progressBarFiller.fillAmount = 0f;
        loadingSpriteImage.sprite = restSprite;

        timer = 0f;
        isLoading = true;
    }


    void UpdateLoadingSprite()
    {
        if (loadingSpriteImage == null)
        {
            Debug.LogError("[RestLoader] loadingSpriteImage 연결 안됨!");
            return;
        }

        loadingSpriteImage.sprite = restSprite;
        Debug.Log("[RestLoader] 로딩 이미지 설정됨: " + loadingSpriteImage.sprite?.name);
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
        loadingText.SetActive(false);

        float currentStamina = StatManager.Instance.currentStamina;
        float maxStamina = StatManager.Instance.maxStamina;
        float ratio = currentStamina / maxStamina;

        int staminaLevel = GetStaminaLevel(ratio);
        HpLogManager.instance.GetLogs(staminaLevel);
        string haroDialogue = HpLogManager.instance.GetSingleLog();
        dialogueText.text = haroDialogue;

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
