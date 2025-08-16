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
        if(!gameover.gameover && !portal.isgameclear)
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
    }
    private IEnumerator StartTime()
    {
        canPause = false; // 일시정지 잠금
        yield return new WaitForSecondsRealtime(3f);
        canPause = true;  // 3초 뒤부터 가능
    }
    public void PauseButtonClick()
    {
        if (!canPause) return; // 3초 안 됐으면 무시
        pauseScreen.SetActive(true);
        Time.timeScale = 0f;
    }
    public void ReturnButtonClick()
    {
        pauseScreen.SetActive(false);
        Time.timeScale = 1.0f;
    }
    public void TitlePortalButtonClick()
    {
        SceneSettingManager.Instance.ChangeScene("MainMenu");
        Time.timeScale = 1.0f;
    }
    public void RetryStageButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1.0f;
    }
    public void NextStageButtonClick()
    {
        SceneSettingManager.Instance.ChangeScene("DialogueScene");
        Time.timeScale = 1.0f;
    }
    public void ReCultivateButtonClick()
    {
        SceneSettingManager.Instance.ChangeScene("Raising_Stage");
        Time.timeScale = 1.0f;
    }
}
