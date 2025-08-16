using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Diagnostics;

[DefaultExecutionOrder(-900)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("턴 설정")]
    public int maxTurn = 15;
    public int CurrentTurn { get; set; }



    public static class TurnTrace
    {
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Log(string msg)
        {
            UnityEngine.Debug.Log($"[TurnTrace] {msg}\n{new StackTrace(1, true)}");
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }


    }

    private void OnDestroy()
    {
        if (Instance == this)
        {

            Instance = null; // 도메인 리로드 옵션 꺼짐 대비
        }
    }

    private void Start()
    {
        //비행씬->메인->육성씬:스탯,체력 로드, 턴0
        if (GameManager.Instance != null && GameManager.Instance.GetCurrentTurn() == 0)
        {
            StageChangeAlarm.Instance?.PromptAndRoute();

        }

        if (SoundManager.instance != null)
            SoundManager.instance.PlayBGM(3, false);
    }





    private IEnumerator DeferredSyncUI()
    {
        // 한 프레임 대기: 다른 컴포넌트(Start) 이후 UI 최종 정합 맞춤
        yield return null;
        StatManager.Instance?.GenerateExpectedStatIncreases();
        SyncUI();
    }



    private void SyncUI()
    {
        UIManager.Instance?.UpdateTurnText(CurrentTurn);
        UIManager.Instance?.UpdateStatUI();
    }

    public int GetCurrentTurn() => CurrentTurn;



    // SetCurrentTurn에서도 PlayerPrefs 저장 제거
    public void SetCurrentTurn(int value, bool syncUI = true, string reason = "unknown")
    {
        int before = CurrentTurn;
        CurrentTurn = Mathf.Clamp(value, 0, maxTurn);

        // PlayerPrefs 저장 제거
        // SaveTurn();

        TurnTrace.Log($"SetCurrentTurn: {before} -> {CurrentTurn} / max={maxTurn}, reason={reason}");

        if (syncUI) SyncUI();
    }

    public bool IsTurnAvailable() => CurrentTurn > 0;

    public void UseTurn()
    {
        if (CurrentTurn > 0)
        {
            CurrentTurn--;


            StatManager.Instance?.GenerateExpectedStatIncreases();
            SyncUI();

            if (CurrentTurn == 0)
                HandleTurnsDepleted();
        }
        else
        {
            UnityEngine.Debug.LogWarning("[GM] 턴이 0임");
            HandleTurnsDepleted();
        }
    }

    private void HandleTurnsDepleted()
    {
        if (CurrentTurn <= 0)
        {
            UnityEngine.Debug.Log("[GameManager] 턴 소진됨");

            if (StageChangeAlarm.Instance != null)
            {
                StageChangeAlarm.Instance.PromptWhenTurnsDepleted(1.5f);
            }
        }
    }

    public void ResetTurnsToMax(bool syncUI = true)
    {
        SetCurrentTurn(maxTurn, syncUI, "ResetTurnsToMax");
        UnityEngine.Debug.Log($"[GM] Turns refilled to {CurrentTurn}");
    }

    private void SaveCurrentStats()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame();
            return;
        }

    }

}
