using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[DefaultExecutionOrder(-900)] 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("턴 설정")]
    public int maxTurn = 12;
    public int CurrentTurn { get; private set; } // 외부 읽기만

    private bool transitioned = false;   // 중복 전환 방지
    //private bool initialized  = false;   // 첫 초기화 여부

    private const string TURN_KEY = "gm_current_turn";

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        // 게임 전체에서 단 1회만 기본 초기화
       /* if (!initialized)
        {
            InitGameFirstBoot();
            initialized = true;
        }
        else
        {
            // 재진입 시 UI만 동기화
            SyncUI();
        }
*/
        // 게임 시작 시 스탯과 체력, 턴 초기화 --> 스탯저장 파일 있을시 제거 / 지금은 아직 구현 안해서 놨두기
        if (StatManager.Instance != null)
        {
            StatManager.Instance.Stamina_Stat = 0;
            StatManager.Instance.Flightpower_Stat = 0;
            StatManager.Instance.Balance_Stat = 0;
            StatManager.Instance.Agility_Stat = 0;

            StatManager.Instance.maxStamina = StatManager.Instance.GetStaminaMax();
            StatManager.Instance.currentStamina = StatManager.Instance.maxStamina;
            StatManager.Instance.currentStamina = StatManager.Instance.maxStamina;
            StatManager.Instance.SaveStatsToJson();
        }

        CurrentTurn = maxTurn;
        SaveTurn();
        SyncUI();
        //사운드 관련 
        if (SoundManager.instance != null)
            SoundManager.instance.PlayBGM(3, false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Raising_Stage" || scene.name == "육성스테이지")
        {
            if (Time.timeScale == 0f) Time.timeScale = 1f;

            // 스탯 로드 먼저
           StatManager.Instance?.LoadStatsFromJson(true);

            // 턴 리필 등 상태 변경
            ResetTurnsToMax(syncUI: false);
            transitioned = false;

            // UI는 한 프레임 뒤에
            StartCoroutine(DeferredSyncUI());
        }

    }
    private IEnumerator DeferredSyncUI()
    {
        yield return null; // 1프레임 대기(StatManager Start/OnSceneLoaded 실행 보장)
        SyncUI();          // 여기서 UpdateTurnText/UpdateStatUI
    }
    // 초기화 및 저장 
    private void InitGameFirstBoot()
    {
        // 첫 게임 시작시 시엔 턴을 풀로 시작 
        SaveTurn();
        SyncUI();
        Debug.Log($"[GM] FirstBoot Init - Turns={CurrentTurn}");
    }

    private void SaveTurn()
    {
        PlayerPrefs.SetInt(TURN_KEY, CurrentTurn);
        PlayerPrefs.Save();
    }

    private void SyncUI()
    {
        StatManager.Instance?.GenerateExpectedStatIncreases();
        UIManager.Instance?.UpdateTurnText(CurrentTurn);
        UIManager.Instance?.UpdateStatUI();
    }

   
    public bool IsTurnAvailable() => CurrentTurn > 0;

    public void UseTurn()
    {
        if (CurrentTurn > 0)
        {
            CurrentTurn--;
            SaveTurn();

            StatManager.Instance?.GenerateExpectedStatIncreases();
            UIManager.Instance?.UpdateTurnText(CurrentTurn);
            UIManager.Instance?.UpdateStatUI();

            if (CurrentTurn == 0)
                HandleTurnsDepleted();
        }
        else
        {
            Debug.LogWarning("[GM] 턴이 0임");
            HandleTurnsDepleted();
        }
    }

    // 턴 모두 소진 시
    private void HandleTurnsDepleted()
    {
        if (transitioned) return;
        transitioned = true;

        SaveCurrentStats();
        StartCoroutine(WaitAndRouteNextStage());
    }

    private void SaveCurrentStats()
    {
      /*  var abs = new AbsoluteStats
        {
            Stamina     = StatManager.Instance.Stamina_Stat,
            FlightPower = StatManager.Instance.Flightpower_Stat,
            Balance     = StatManager.Instance.Balance_Stat,
            Agility     = StatManager.Instance.Agility_Stat
        };
        //StatsStore.Save(abs, "턴 0, 자동 저장");
        Debug.Log("[GameManager] 스탯 저장 완료");*/
    }
    private IEnumerator WaitAndRouteNextStage()
    {
        for (int i = 0; i < 10; i++)
        {
            if (SceneSettingManager.Instance != null) break;
            yield return null;
        }

        var ssm = SceneSettingManager.Instance ?? Object.FindObjectOfType<SceneSettingManager>();
        if (ssm == null)
        {
            Debug.LogWarning("[GameManager] SSM 없음 → Stage1 폴백");
            SceneManager.LoadScene("Stage1");
            yield break;
        }

        if (!ssm.isStage1_Clear)        ssm.ChangeScene("Stage1");
        else if (!ssm.isStage2_Clear)    ssm.ChangeScene("Stage2");
        else                             Debug.Log("[GameManager] 모든 스테이지 클리어 상태");
    }

    // 턴을 max로 리필 (저장/선택적 UI 동기화)
    public void ResetTurnsToMax(bool syncUI = true)
    {
        CurrentTurn = maxTurn;
        SaveTurn();
        if (syncUI) SyncUI();
        Debug.Log($"[GM] Turns refilled to {CurrentTurn}");
    }
}
