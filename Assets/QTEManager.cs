using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QTEManager : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] StatManager statManager;

    [SerializeField] GameObject backGround;

    [SerializeField] Image First;
    [SerializeField] Image Second;
    [SerializeField] Image Third;
    [SerializeField] Image Fourth;

    [SerializeField] Sprite arrowUp;
    [SerializeField] Sprite arrowDown;
    [SerializeField] Sprite arrowLeft;
    [SerializeField] Sprite arrowRight;

    [SerializeField] private Image[] qteImages; // 0~3 인덱스에 QTE 이미지 넣기

    private Image[] arrowImages;

    private KeyCode[] assignedKeys = new KeyCode[4];
    private KeyCode[] availableKeys = new KeyCode[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
    private KeyCode[] arrowKeysAlt = new KeyCode[]  { KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.RightArrow };

    public bool clearQTE = false;
    public bool isQTE = false;

    private bool canQTE = true;

    private Sprite GetSpriteForKey(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.W : return arrowUp;
            case KeyCode.S : return arrowDown;
            case KeyCode.A : return arrowLeft;
            case KeyCode.D : return arrowRight;
            default : return null;
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

    public IEnumerator StartQTETime(float coefficient, int damage)
    {
        if (!StatManager.Instance.ShouldTriggerQTE(coefficient, player.stageFactor) || !canQTE)
        {
            player.Hit(damage);
            clearQTE = true;
            EndQTE();
            yield break;
        }
        else
        {
            isQTE = true;
            backGround.SetActive(true);
            clearQTE = false;
            Time.timeScale = 0f; // 게임 시간 정지

            for (int i = 0; i < 4; i++)
            {
                assignedKeys[i] = availableKeys[Random.Range(0, availableKeys.Length)];
                arrowImages[i].sprite = GetSpriteForKey(assignedKeys[i]);
                Debug.Log($"[{i + 1}]을 입력해주세요: {assignedKeys[i]}");
                arrowImages[i].enabled = true;
            }

            for (int i = 0; i < 4; i++)
            {
                float timer = 0f;
                float maxTime = 4.5f;
                bool success = false;

                while (timer < maxTime)
                {
                    foreach (var key in availableKeys.Concat(arrowKeysAlt))
                    {
                        if (Input.GetKeyDown(key))
                        {
                            KeyCode assigned = assignedKeys[i];
                            int index = System.Array.IndexOf(availableKeys, assigned);

                            if (key == availableKeys[index] || key == arrowKeysAlt[index])
                            {
                                SoundManager.instance.PlaySFX(8,0);
                                success = true;
                                qteImages[i].enabled = false;
                                yield return new WaitForSecondsRealtime(0.05f);
                                break;
                            }
                            else
                            {
                                player.Hit(damage);
                                EndQTE();
                                yield break;
                            }
                        }
                    }

                    if (success)
                        break;
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
            yield return new WaitForSecondsRealtime(1f);
            canQTE = true;
        }
    }

    private void EndQTE()
    {
        isQTE = false;
        canQTE = false;
        backGround.SetActive(false);
        Time.timeScale = 1f;
        StartCoroutine(InvincibleTimeAfterQTE());

        for (int i = 0; i < arrowImages.Length; i++)
        {
            arrowImages[i].enabled = false;
        }
    }

    private IEnumerator InvincibleTimeAfterQTE()
    {
        player.isInvincible = true;
        yield return new WaitForSeconds(0.7f);
        player.isInvincible = false;
    }
}