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
        UIManager.Instance.UpdateTurnText(currentTurn);
    }

    // 턴 사용
    public void UseTurn()
    {
        if (currentTurn > 0)
        {
            currentTurn--;
            if (UIManager.Instance != null)
                UIManager.Instance.UpdateTurnText(currentTurn);
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
