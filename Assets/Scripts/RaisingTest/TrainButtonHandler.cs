using UnityEngine;

public class TrainButtonHandler : MonoBehaviour
{
    public StatType statType;
    public TrainingLoader loader; // Inspector에서 연결할 것

    public void OnClickTrain()
    {
        Debug.Log("[TrainButtonHandler] OnClickTrain 호출됨");

        // ✅ SoundManager null 체크
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlaySFX(7, 0f);
        }
        else
        {
            Debug.LogWarning("SoundManager.instance가 null입니다. 사운드 재생 생략됨");
        }

        // ✅ GameManager null 체크
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance가 null입니다.");
            return;
        }

        if (!GameManager.Instance.IsTurnAvailable())
        {
            Debug.LogWarning("턴이 부족");
            return;
        }

        // ✅ StatManager null 체크
        if (StatManager.Instance == null)
        {
            Debug.LogError("StatManager.Instance가 null입니다.");
            return;
        }

        StatManager.Instance.GenerateExpectedStatIncreases();

        // ✅ loader 연결 확인
        if (loader == null)
        {
            Debug.LogError("[TrainButtonHandler] Loader가 연결안됨");
            return;
        }

        loader.StartTraining(statType);
    }
}
