
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

    [Header("턴")]
    public TextMeshProUGUI turnText;
    [Header("하로 체력")]
    public Image staminaBarFiller;

    [Header("하로 대사")]
    public TextMeshProUGUI dialogueText; // 하로 대사 출력용 텍스트 

    [Header("훈련중 로딩 바")]
    public TrainingLoader loader; // Inspector에 연결


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

        staminaGradeText.text = $"스태미나: {StatManager.Instance.GetStaminaGrade()}";
        flightpowerGradeText.text = $"비행력: {StatManager.Instance.GetFlightpowerGrade()}";
        balanceGradeText.text = $"균형감: {StatManager.Instance.GetBalanceGrade()}";
        agilityGradeText.text = $"민첩성: {StatManager.Instance.GetAgilityGrade()}";


        UpdateStaminaBar(); // 체력 게이지 바 업데이트


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

    //체력바가 시각적으로 너무 닳아서 따로 조정 -> StaminaBarController.cs
    public StaminaBarController staminaBarController;

    public void UpdateStaminaBar()
    {
        staminaBarController.UpdateBar(StatManager.Instance.currentStamina, StatManager.Instance.maxStamina);
    }




    public void OnClickTrainStamina()
    {
        loader.StartTraining(StatType.Stamina_Stat);
    }

    public void OnClickTrainFlightPower()
    {
        loader.StartTraining(StatType.Flightpower_Stat);
    }

    public void OnClickTrainBalance()
    {
        loader.StartTraining(StatType.Flightpower_Stat);
    }
    
      public void OnClickTrainAgility()
    {
        loader.StartTraining(StatType.Flightpower_Stat);
    }



}
