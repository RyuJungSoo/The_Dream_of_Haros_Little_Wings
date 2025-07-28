using UnityEngine;

public class TrainButtonHandler : MonoBehaviour
{
    public StatType statType;

    public void OnClickTrain() // <- 반드시 public
    {
        if (!GameManager.Instance.IsTurnAvailable()) return;

        StatManager.Instance.GenerateExpectedStatIncreases();

        float cost = StatManager.Instance.GetStaminaCost(statType);
        StatManager.Instance.DecreaseStamina(cost);

        // 체력 기반 실패율 계산
        float currentStamina = StatManager.Instance.currentStamina;
        float maxStamina = StatManager.Instance.maxStamina;
        float failureRate = 1f - (currentStamina / maxStamina); // 체력이 높을수록 실패율 낮음 (0~1)

        float chance = Random.value; // 0.0f ~ 1.0f

        if (chance < failureRate)
        {
            Debug.Log($"{statType} 훈련 실패 (실패율 {failureRate * 100f:F1}%)");
        }
        else
        {
            Debug.Log($"{statType} 훈련 성공");
            StatManager.Instance.IncreaseStat(statType);
        }

        GameManager.Instance.UseTurn();
        UIManager.Instance.UpdateStatUI();
    }
}
