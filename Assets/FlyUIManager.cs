using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class FlyUIManager : MonoBehaviour
{
    [SerializeField]
    Player player;

    [SerializeField]
    StaminaSlider slider;

    [SerializeField]
    GameObject pauseScreen;

    public void PauseButtonClick()
    {
        pauseScreen.SetActive(true);
        Time.timeScale = 0f;
    }
    public void ReturnButtonClick()
    {
        pauseScreen.SetActive(false);
        Time.timeScale = 1.0f;
    }
    public void TitlePortalClick()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
