using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class FlyUIManager : MonoBehaviour
{
    [SerializeField]
    Player player;

    [SerializeField] GameOver gameover;
    [SerializeField] Portal portal;
    [SerializeField] QTEManager QTEManager;

    [SerializeField]
    StaminaSlider slider;

    [SerializeField]
    GameObject pauseScreen;

    private bool canPause = false;

    void Start()
    {
        StartCoroutine(StartTime());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
             ESCButtonClick();
        }
    }

    private void ESCButtonClick()
    {
            if(!pauseScreen.activeSelf)
            {
                PauseButtonClick();
            }
            else if (pauseScreen.activeSelf)
            {
                ReturnButtonClick();
            }
    }
    private IEnumerator StartTime()
    {
        canPause = false; // 일시정지 잠금
        yield return new WaitForSecondsRealtime(3f);
        canPause = true;  // 3초 뒤부터 가능
    }
    public void PauseButtonClick()
    {
        if (!gameover.gameover && !portal.isgameclear && !QTEManager.isQTE)
        {
            if (!canPause) return; // 3초 안 됐으면 무시
            SoundManager.instance.PlaySFX(11, 0);
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;
        }
    }
    public void ReturnButtonClick()
    {
        SoundManager.instance.PlaySFX(11, 0);
        pauseScreen.SetActive(false);
        Time.timeScale = 1.0f;
    }
    public void TitlePortalButtonClick()
    {
        SoundManager.instance.PlaySFX(11, 0);
        SceneSettingManager.Instance.ChangeScene("MainMenu");
        Time.timeScale = 1.0f;
    }
    public void RetryStageButtonClick()
    {
        SoundManager.instance.PlaySFX(11, 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1.0f;
    }
    public void NextStageButtonClick()
    {
        SoundManager.instance.PlaySFX(11, 0);
        SceneSettingManager.Instance.ChangeScene("DialogueScene");
        Time.timeScale = 1.0f;
    }
    public void ReCultivateButtonClick()
    {
        SoundManager.instance.PlaySFX(11, 0);
        SceneSettingManager.Instance.ChangeScene("Raising_Stage");
        Time.timeScale = 1.0f;
    }
}
