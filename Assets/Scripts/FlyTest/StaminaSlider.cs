using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaSlider : MonoBehaviour
{
    public Slider slider;

    float stamina;
    float fullStamina;

    public Stat_Data data;

    public GameOver gameover;


    void Start()
    {
        fullStamina = data.Stamina_Max;
        stamina = fullStamina;
    }

    void Update()
    {
        if(stamina <= 0)
        {
            stamina = 0;
            gameover.Gameover();
        }
        else if(stamina > 0 && !gameover.gameover)
        {
        slider.value = stamina/fullStamina;
        stamina -= data.Total_Stamina_DecreaseSpeed * Time.deltaTime;
        }

    }
}
