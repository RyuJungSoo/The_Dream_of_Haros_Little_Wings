using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class StatHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("선택 UI 관련")]
    public GameObject ChoesnCheck;      // 테두리 오브젝트 (켜고 끌 대상)
    public GameObject FailChance;       // 실패율 오브젝트 (켜고 끌 대상)

    [Header("예상 수치 출력용")]
    public string statName;             // 예: "Stamina", "Flightpower"
    public TextMeshProUGUI mainText;    // 주스탯 예상 텍스트
    public TextMeshProUGUI subText1;    // 보조스탯1 예상 텍스트
    public TextMeshProUGUI subText2;    // 보조스탯2 (비상력용만 사용)

    [Header("하로 대사 출력용")]
    public TextMeshProUGUI dialogueText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 선택 체크 및 실패율 켜기
        ChoesnCheck?.SetActive(true);
        FailChance?.SetActive(true);

        // 주/보조 스탯 예상 수치 표시
        var (main, sub) = StatManager.Instance.GetMainAndSubStatText(statName);
        mainText.text = $"<color=#FF0000>{main}</color>";
        mainText.gameObject.SetActive(true);

        string subOnlyNumber = System.Text.RegularExpressions.Regex.Replace(sub, @"\D", "");

        if (subOnlyNumber != "0")
        {
            subText1.text = $"<color=#FFBA00>{sub}</color>";
            subText1.gameObject.SetActive(true);

            if (subText2 != null)
            {
                subText2.text = $"<color=#FFBA00>{sub}</color>";
                subText2.gameObject.SetActive(true);
            }
        }
        else
        {
            subText1.gameObject.SetActive(false);
            if (subText2 != null)
                subText2.gameObject.SetActive(false);
        }

        // 실패율 UI 갱신
        var failUI = FailChance?.GetComponent<Fail_Chance_UI>();
        if (failUI != null)
        {
            int failureRate = Mathf.Clamp(Mathf.RoundToInt(100f - StatManager.Instance.currentStamina), 0, 100);
            failUI.UIUpdate(failureRate);
        }

        // ✅ 하로 대사 출력
        int staminaLevel = GetStaminaLevelByRatio(StatManager.Instance.currentStamina / StatManager.Instance.maxStamina);
        HpLogManager.instance.GetLogs(staminaLevel);
        string haroDialogue = HpLogManager.instance.GetSingleLog();
        dialogueText.text = haroDialogue;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // UI 요소 숨기기
        ChoesnCheck?.SetActive(false);
        FailChance?.SetActive(false);

        mainText.gameObject.SetActive(false);
        subText1.gameObject.SetActive(false);

        if (subText2 != null)
            subText2.gameObject.SetActive(false);

        // ✅ 하로 대사 초기화 (선택)
        if (dialogueText != null)
            dialogueText.text = "";
    }

    // ✅ 스태미나 비율에 따라 레벨 반환
    private int GetStaminaLevelByRatio(float ratio)
    {
        if (ratio >= 0.8f) return 1;
        else if (ratio >= 0.6f) return 2;
        else if (ratio >= 0.4f) return 3;
        else if (ratio >= 0.2f) return 4;
        else return 5;
    }
}
