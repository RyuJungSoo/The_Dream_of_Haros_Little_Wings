using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

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
    public TextMeshProUGUI dialogueText;

    [Header("훈련중 로딩 바")]
    public TrainingLoader loader;

    [Header("체력바 컨트롤러")]
    public StaminaBarController staminaBarController;

    [Header("말풍선 오브젝트")]
    public GameObject speechBubbleObject;
    public TextMeshProUGUI speechBubbleText;

    private Coroutine speechCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (speechBubbleObject != null)
            speechBubbleObject.SetActive(false);
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
        loader.StartTraining(StatType.Balance_Stat);
    }

    public void OnClickTrainAgility()
    {
        loader.StartTraining(StatType.Agility_Stat);
    }

    // ✅ 일정 시간 표시되는 말풍선 함수 추가
    public void ShowSpeechBubble(string message, float duration)
    {
        if (speechCoroutine != null)
        {
            StopCoroutine(speechCoroutine);
        }
        speechCoroutine = StartCoroutine(SpeechBubbleRoutine(message, duration));
    }

    IEnumerator SpeechBubbleRoutine(string message, float duration)
    {
        if (speechBubbleObject != null && speechBubbleText != null)
        {
            speechBubbleText.text = message;
            speechBubbleObject.SetActive(true);
            yield return new WaitForSeconds(duration);
            speechBubbleObject.SetActive(false);
        }
    }

    // 말풍선 수동으로 끄기
    public void HideSpeechBubble()
    {
        if (speechCoroutine != null)
        {
            StopCoroutine(speechCoroutine);
            speechCoroutine = null;
        }

        if (speechBubbleObject != null)
        {
            speechBubbleObject.SetActive(false);
        }
    }
}