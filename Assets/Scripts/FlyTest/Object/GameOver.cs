using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] GameObject gameOverScreen;

    [SerializeField]
    Player player;

    [SerializeField]
    public bool gameover = false;

    [SerializeField]
    StaminaSlider slider;


    public void Gameover()
    {
        gameOverScreen.SetActive(true);
        gameover = true;
        Time.timeScale = 0f;
    }
}
