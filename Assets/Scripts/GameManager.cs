using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    [Header("턴 설정")]
    public int maxTurn = 12;
    private int currentTurn;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitGame();
    }

    // 게임 시작 시 턴 초기화
    public void InitGame()
    {
        currentTurn = maxTurn;
        Debug.Log($"[GameManager] InitGame 실행됨 - 현재 턴: {currentTurn}");
        Debug.Log($"[TrainButtonHandler] 현재 턴 수: {GameManager.Instance.currentTurn}");

        StatManager.Instance.GenerateExpectedStatIncreases();  // ① 게임 시작 시 첫 예측 스탯 생성
        UIManager.Instance.UpdateTurnText(currentTurn);
        UIManager.Instance.UpdateStatUI();                     // ② 예측값 UI에 반영
    }

    public void UseTurn()
    {
        if (currentTurn > 0)
        {
            currentTurn--;

            // 👉 다음 턴 주/보조 예상값 생성
            StatManager.Instance.GenerateExpectedStatIncreases();

            // 👉 UI 갱신
            UIManager.Instance.UpdateTurnText(currentTurn);
            UIManager.Instance.UpdateStatUI();
        }
        else
        {
            Debug.LogWarning("턴이 0입니다. 더 이상 행동할 수 없습니다.");
        }
    }


    // 현재 턴 가져오기
    public int GetCurrentTurn()
    {
        return currentTurn;
    }

    // 턴이 남아있는지 확인
    public bool IsTurnAvailable()
    {
        return currentTurn > 0;
    }
        

}
