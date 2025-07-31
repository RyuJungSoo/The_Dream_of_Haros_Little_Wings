using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QTEManager : MonoBehaviour
{
    [SerializeField] Player player;

    [SerializeField] Image First;
    [SerializeField] Image Second;
    [SerializeField] Image Third;
    [SerializeField] Image Fourth;

    [SerializeField] Sprite arrowUp;
    [SerializeField] Sprite arrowDown;
    [SerializeField] Sprite arrowLeft;
    [SerializeField] Sprite arrowRight;

    private Image[] arrowImages;

    private KeyCode[] assignedKeys = new KeyCode[4];
    private KeyCode[] availableKeys = new KeyCode[] { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.W };

    public bool clearQTE = false;
    public bool isQTE = false;

    private Sprite GetSpriteForKey(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.W:
                return arrowUp;
            case KeyCode.S:
                return arrowDown;
            case KeyCode.A:
                return arrowLeft;
            case KeyCode.D:
                return arrowRight;
            default:
                return null;
        }
    }

    private void Awake()
    {
        arrowImages = new Image[] { First, Second, Third, Fourth };

        for (int i = 0; i < arrowImages.Length; i++)
        {
            arrowImages[i].enabled = false;
        }
    }

    public IEnumerator StartQTETime()
    {
        isQTE = true;
        clearQTE = false;

        Time.timeScale = 0f; // 게임 시간 정지

        // 각 버튼에 랜덤 키와 화살표 스프라이트 할당
        for (int i = 0; i < 4; i++)
        {
            assignedKeys[i] = availableKeys[Random.Range(0, availableKeys.Length)];
            arrowImages[i].sprite = GetSpriteForKey(assignedKeys[i]);
            Debug.Log($"[{i + 1}]을 입력해주세요: {assignedKeys[i]}");
            arrowImages[i].enabled = true;
        }

        // 입력 대기 루프
        for (int i = 0; i < 4; i++)
        {
            float timer = 0f;
            float maxTime = 5f;
            bool success = false;

            while (timer < maxTime)
            {
                if (Input.GetKeyDown(assignedKeys[i]))
                {
                    success = true;
                    Debug.Log($"[{i + 1}] 입력 성공: {assignedKeys[i]}");
                    break;
                }

                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            if (!success)
            {
                Debug.Log("QTE 실패!");
                EndQTE();
                yield break;
            }
        }

        clearQTE = true;
        Debug.Log("QTE 성공!");
        EndQTE();
    }

    private void EndQTE()
    {
        isQTE = false;
        Time.timeScale = 1f;

        for (int i = 0; i < arrowImages.Length; i++)
        {
            arrowImages[i].enabled = false;
        }
    }
}