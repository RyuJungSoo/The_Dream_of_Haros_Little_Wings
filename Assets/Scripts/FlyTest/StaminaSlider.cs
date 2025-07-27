using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class StaminaSlider : MonoBehaviour
{
    public Slider slider;

    public float stamina;
    public float fullStamina;

    public StatManager data;

    public GameOver gameover;

    public CollideItem item;


    public void Start()
    {
        // fullStamina = data.GetStaminaMax();
        fullStamina = 200;
        stamina = fullStamina;
    }

    public void Update()
    {
        if (stamina > fullStamina)
        {
            stamina = fullStamina;
        }
        if (stamina <= 0)
        {
            slider.value = 0;
            gameover.Gameover();
        }


        if(item.onWind && !gameover.gameover)
        {
            slider.value = stamina / fullStamina;
        }
        else if (stamina > 0 && !gameover.gameover)
        {
            slider.value = stamina / fullStamina;
            // stamina -= data.GetStaminaDrainSpeed() * Time.deltaTime / 10f;
            stamina -= 5 * Time.deltaTime;
        }
    }
}