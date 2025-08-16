using UnityEngine;
using UnityEngine.UI;

public class RestButtonHandler : MonoBehaviour
{
    [Header("회복량 설정")]
    public float recoveryAmount = 30f;  // 회복할 체력 양

    private Button button;
    public RestLoader restLoader; 

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        if (button != null)
            button.onClick.AddListener(OnRestClick);
    }

    private void OnRestClick()
    {
        // 가드: 매니저/턴 체크
        if (GameManager.Instance == null || !GameManager.Instance.IsTurnAvailable())
            return;
        if (StatManager.Instance == null)
        {
            Debug.LogError("StatManager.Instance가 null입니다.");
            return;
        }
        if (restLoader == null)
        {
            Debug.LogError("RestLoader 미연결");
            return;
        }

        // 체력 회복 
        StatManager.Instance.currentStamina = Mathf.Min(
            StatManager.Instance.currentStamina + recoveryAmount,
            StatManager.Instance.maxStamina
        );

        // 로더 활성 보장: 비활성 GO/컴포넌트면 Update가 안 돌므로 먼저 켜기
        if (!restLoader.gameObject.activeInHierarchy)
            restLoader.gameObject.SetActive(true);
        if (!restLoader.enabled)
            restLoader.enabled = true;

        // 로딩/휴식 시작 (같은 클릭에서 바로 실행)
        restLoader.StartRest();

        // 턴 소모 및 UI 갱신
        UIManager.Instance.UpdateStatUI();
        UIManager.Instance.HideAllChosenChecks();
    }


}
