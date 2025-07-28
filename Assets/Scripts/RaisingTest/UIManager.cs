// ============================
// UIManager.cs
// ============================
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    [Header("선택 표시 오브젝트")]
    public GameObject chosenCheckStamina;
    public GameObject chosenCheckFlight;
    public GameObject chosenCheckBalance;
    public GameObject chosenCheckAgility;

    [Header("턴 및 체력")]
    public TextMeshProUGUI turnText;
    public Image staminaBarFiller;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateStatUI()
    {
        staminaValueText.text = $"{StatManager.Instance.Stamina_Stat} / 180";
        flightpowerValueText.text = $"{StatManager.Instance.Flightpower_Stat} / 180";
        balanceValueText.text = $"{StatManager.Instance.Balance_Stat} / 180";
        agilityValueText.text = $"{StatManager.Instance.Agility_Stat} / 180";

        staminaGradeText.text = $"스태미나 : {StatManager.Instance.GetStaminaGrade()}";
        flightpowerGradeText.text = $"비상력 : {StatManager.Instance.GetFlightpowerGrade()}";
        balanceGradeText.text = $"균형감 : {StatManager.Instance.GetBalanceGrade()}";
        agilityGradeText.text = $"민첩성 : {StatManager.Instance.GetAgilityGrade()}";

        UpdateTurnText(GameManager.Instance.GetCurrentTurn());
        UpdateStaminaBar();
    }

    public void UpdateTurnText(int turn)
    {
        turnText.text = $"{turn} 턴";
    }



    public void ShowChosenCheck(GameObject chosen)
    {
        HideAllChosenChecks();
        if (chosen != null) chosen.SetActive(true);
    }

    public void HideAllChosenChecks()
    {
        chosenCheckStamina.SetActive(false);
        chosenCheckFlight.SetActive(false);
        chosenCheckBalance.SetActive(false);
        chosenCheckAgility.SetActive(false);
    }

  
        public void UpdateStaminaBar()
    {
        if (staminaBarFiller != null)
        {
            float fillAmount = StatManager.Instance.currentStamina / StatManager.Instance.maxStamina;

            // 🔽 시각적으로만 덜 줄어들게 보이도록 조절 (예: 실제의 0.5배)
            float visualScale = 0.5f; // 0.5로 하면 체력 50이 25처럼 보임 (느리게 닳는 느낌)
            fillAmount *= visualScale;

            staminaBarFiller.fillAmount = Mathf.Clamp01(fillAmount);
        }
    }

}
