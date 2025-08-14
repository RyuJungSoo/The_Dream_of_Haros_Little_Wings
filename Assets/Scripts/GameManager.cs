using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[DefaultExecutionOrder(-900)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("턴 설정")]
    public int maxTurn = 12;
    public int CurrentTurn { get; private set; }

    private bool transitioned = false;
    private const string TURN_KEY = "gm_current_turn";

    [SerializeField] private StageChangeAlarm stageChangeAlarm;
    private bool nextStagePromptShown = false;

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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null; // 도메인 리로드 옵션 꺼짐 대비
        }
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey(TURN_KEY))
            CurrentTurn = Mathf.Clamp(PlayerPrefs.GetInt(TURN_KEY), 0, maxTurn);
        else
            CurrentTurn = maxTurn;

        SaveTurn();
        SyncUI();

        if (SoundManager.instance != null)
            SoundManager.instance.PlayBGM(3, false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Raising_Stage" || scene.name == "육성스테이지")
        {
            if (Time.timeScale == 0f) Time.timeScale = 1f;

            SaveManager.Instance?.LoadGame();
            StatManager.Instance?.LoadStatsFromJson(false);

            transitioned = false;
            StartCoroutine(DeferredSyncUI());
        }
    }

    private IEnumerator DeferredSyncUI()
    {
        yield return null;
        StatManager.Instance?.GenerateExpectedStatIncreases();
        SyncUI();
    }

    private void SaveTurn()
    {
        PlayerPrefs.SetInt(TURN_KEY, CurrentTurn);
        PlayerPrefs.Save();
    }

    private void SyncUI()
    {
        UIManager.Instance?.UpdateTurnText(CurrentTurn);
        UIManager.Instance?.UpdateStatUI();
    }

    public int GetCurrentTurn() => CurrentTurn;

    public void SetCurrentTurn(int value, bool syncUI = true)
    {
        CurrentTurn = Mathf.Clamp(value, 0, maxTurn);
        SaveTurn();
        if (syncUI) SyncUI();
    }

    public bool IsTurnAvailable() => CurrentTurn > 0;

    public void UseTurn()
    {
        if (CurrentTurn > 0)
        {
            CurrentTurn--;
            SaveTurn();

            StatManager.Instance?.GenerateExpectedStatIncreases();
            SyncUI();

            if (CurrentTurn == 0)
                HandleTurnsDepleted();
        }
        else
        {
            Debug.LogWarning("[GM] 턴이 0임");
            HandleTurnsDepleted();
        }
    }

    private void HandleTurnsDepleted()
    {
        if (transitioned) return;
        transitioned = true;

        SaveCurrentStats();
        // 팝업 띄우기 (Next 누르면 StageChangeAlarm에서 라우팅)
        ShowNextStagePromptOnce();

    }



    public void ResetTurnsToMax(bool syncUI = true)
    {
        SetCurrentTurn(maxTurn, syncUI);
        Debug.Log($"[GM] Turns refilled to {CurrentTurn}");
    }

    private void SaveCurrentStats()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame();
            return;
        }
        StatManager.Instance?.SaveStatsToJson();
    }
    // 턴수 0일때 다음 스테이지로 넘어갈 팝업창 함수 
        private void ShowNextStagePromptOnce()
    {
        if (nextStagePromptShown) return;
        nextStagePromptShown = true;

        // 싱글턴 우선
        if (StageChangeAlarm.Instance != null)
        {
            StageChangeAlarm.Instance.PromptAndRoute();
            return;
        }

        // 인스펙터 참조(없으면 에러 로그)
        if (stageChangeAlarm != null)
        {
            stageChangeAlarm.PromptAndRoute();
            return;
        }

        Debug.LogError("[GM] StageChangeAlarm 인스턴스를 찾지 못했습니다. 씬에 배치하고 panel/Text/Button을 연결하세요.");
    }

}
