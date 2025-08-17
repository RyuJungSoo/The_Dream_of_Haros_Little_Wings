using UnityEngine;
using UnityEngine.UI;

public class StaminaSlider : MonoBehaviour
{
    public Slider slider;
    [SerializeField] PlayerJump playerjump;

    public float stamina;
    public float fullStamina;
    private float curStamina;

    public bool stopStaminaBar = false;

    public StatManager data;

    public GameOver gameover;

    public CollideItem item;


    public void Start()
    {
        fullStamina = StatManager.Instance.GetStaminaMax();
        // fullStamina = 200;
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
            if(!gameover.gameover)
            {
                gameover.Gameover();
            }
        }

        if (playerjump.onWind && !gameover.gameover)
        {
            slider.value = stamina / fullStamina;
        }
        else if (stamina > 0 && !stopStaminaBar)
        {
            slider.value = stamina / fullStamina;

            if (!playerjump.isJump)
            {
                stamina -= StatManager.Instance.GetStaminaDrainSpeed_Descend() * Time.deltaTime / 15f;
            }
            else if (playerjump.isJump)
            {
                stamina -= StatManager.Instance.GetStaminaDrainSpeed_Ascend() * Time.deltaTime / 15f;
            }
            // stamina -= 5 * Time.deltaTime;
        }
    }
}