using UnityEngine;

public class TrainButtonHandler : MonoBehaviour
{
    private const float V2 = 0f;
    public StatType statType;
    public TrainingLoader loader; 

       public void OnClickTrain()
    {
        // 버튼 클릭 시 SFX 7번 재생
        if (SoundManager.instance != null)
            SoundManager.instance.PlaySFX(7, 0f); // (SFX 번호, 볼륨)

        Debug.Log($"[TrainButtonHandler] {statType} 버튼 클릭됨");
    if (loader == null) { Debug.LogError("TrainingLoader 미연결"); return; }

        //두번 클릭 문제 관련
        // 로더/컴포넌트가 꼭 켜져 있어야 첫 클릭부터 Update()가 돌게
        if (!loader.gameObject.activeInHierarchy) loader.gameObject.SetActive(true);
        if (!loader.enabled) loader.enabled = true;

        StatManager.Instance.GenerateExpectedStatIncreases();
        loader.StartTraining(statType);   // 여기서 패널만 켜고 isLoading=true 설정


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

