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
        if (Instance == this) {
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
        StartCoroutine(WaitAndRouteNextStage());
    }

    private IEnumerator WaitAndRouteNextStage()
    {
        for (int i = 0; i < 10; i++)
        {
            if (SceneSettingManager.Instance != null) break;
            yield return null;
        }

        var ssm = SceneSettingManager.Instance ?? FindObjectOfType<SceneSettingManager>();
        if (ssm == null)
        {
            Debug.LogWarning("[GameManager] SSM 없음 → Stage1 폴백");
            SceneManager.LoadScene("Stage1");
            yield break;
        }

        if (!ssm.isStage1_Clear)       ssm.ChangeScene("Stage1");
        else if (!ssm.isStage2_Clear)  ssm.ChangeScene("Stage2");
        else                            Debug.Log("[GameManager] 모든 스테이지 클리어 상태");
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
            SaveManager.Instance.SaveGame("턴 소진 자동 저장");
            return;
        }
        StatManager.Instance?.SaveStatsToJson();
    }
}
