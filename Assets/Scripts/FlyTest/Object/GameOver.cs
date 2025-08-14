using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] GameObject gameOverScreen;

    [SerializeField]
    Player player;

    public bool gameover = false;
    private bool gameoverSoundCheck = false;

    [SerializeField]
    StaminaSlider slider;


    public void Gameover()
    {
        gameOverScreen.SetActive(true);
        gameover = true;
        gameoverSoundCheck = true;
        if(gameoverSoundCheck)
        {
            SoundManager.instance.StopBGM();
            SoundManager.instance.PlayBGM(4, false);
            SoundManager.instance.PlaySFX(4, 0);
            gameoverSoundCheck = false;
        }
        Time.timeScale = 0f;
    }
}
