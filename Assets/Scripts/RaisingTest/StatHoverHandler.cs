using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class StatHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject ChoesnCheck;      // 테두리 오브젝트 (켜고 끌 대상)
    public GameObject FailChance;       // 실패율 오브젝트 (켜고 끌 대상)
    public string statName;             // 예: "Stamina", "Flightpower"
    public TextMeshProUGUI mainText;    // 주스탯 예상 텍스트
    public TextMeshProUGUI subText1;    // 보조스탯1 예상 텍스트
    public TextMeshProUGUI subText2;    // (선택) 보조스탯2 (비상력용)

    public void OnPointerEnter(PointerEventData eventData)
    {
        ChoesnCheck?.SetActive(true);
        FailChance?.SetActive(true);

        var (main, sub) = StatManager.Instance.GetMainAndSubStatText(statName);
        mainText.text = $"<color=#FF0000>{main}</color>";
        subText1.text = $"<color=#FFBA00>{sub}</color>";
        mainText.gameObject.SetActive(true);
        subText1.gameObject.SetActive(true);

        if (subText2 != null)
        {
            subText2.text = $"<color=#FFBA00>{sub}</color>";
            subText2.gameObject.SetActive(true);
        }

        // 실패율 UI 갱신
        var failUI = FailChance?.GetComponent<Fail_Chance_UI>();
        if (failUI != null)
        {
            int failureRate = Mathf.Clamp(Mathf.RoundToInt(100f - StatManager.Instance.currentStamina), 0, 100);
            failUI.UIUpdate(failureRate);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ChoesnCheck?.SetActive(false);
        FailChance?.SetActive(false);

        mainText.gameObject.SetActive(false);
        subText1.gameObject.SetActive(false);

        if (subText2 != null)
            subText2.gameObject.SetActive(false);
    }
}
