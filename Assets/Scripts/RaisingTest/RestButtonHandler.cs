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
        if (GameManager.Instance != null && GameManager.Instance.IsTurnAvailable())
        {
            // 체력 회복 처리
            StatManager.Instance.currentStamina += recoveryAmount;
            if (StatManager.Instance.currentStamina > StatManager.Instance.maxStamina)
                StatManager.Instance.currentStamina = StatManager.Instance.maxStamina;

            //로딩 실행
            if (restLoader != null)
                restLoader.StartRest();

            // 턴 소모 및 UI 갱신 요청
            GameManager.Instance.UseTurn();
            UIManager.Instance.UpdateStatUI();
            UIManager.Instance.HideAllChosenChecks();
        }
    }

}
