using UnityEngine;

public class TrainButtonHandler : MonoBehaviour
{
    public StatType statType;
    public TrainingLoader loader; // Inspector에서 연결할 것

    public void OnClickTrain()
    {
        Debug.Log("[TrainButtonHandler] OnClickTrain 호출됨");

        if (!GameManager.Instance.IsTurnAvailable())
        {
            Debug.LogWarning("턴이 부족해서 훈련 불가");
            return;
        }

        StatManager.Instance.GenerateExpectedStatIncreases();

        if (loader == null)
        {
            Debug.LogError("[TrainButtonHandler] Loader가 연결되어 있지 않습니다!");
            return;
        }

        loader.StartTraining(statType); // ✅ 싱글턴이 아니므로 Instance 사용 ❌
    }
}
