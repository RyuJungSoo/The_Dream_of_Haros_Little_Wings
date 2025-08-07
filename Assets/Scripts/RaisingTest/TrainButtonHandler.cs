using UnityEngine;

public class TrainButtonHandler : MonoBehaviour
{
    public StatType statType;
    public TrainingLoader loader; 

       public void OnClickTrain()
    {
        Debug.Log($"[TrainButtonHandler] {statType} 버튼 클릭됨");

        // SFX 7번 
        SoundManager.instance.PlaySFX(7, 0f);

        if (!GameManager.Instance.IsTurnAvailable())
        {
            Debug.LogWarning("턴이 부족합니다.");
            return;
        }

        StatManager.Instance.GenerateExpectedStatIncreases();

        if (loader == null)
        {
            Debug.LogError("TrainingLoader가 연결되어 있지 않음");
            return;
        }

        loader.StartTraining(statType);
    }
}

