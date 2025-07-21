using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Name
    {
        Wind,
        Bell,
        acorn
    }

public class CollideItem : MonoBehaviour
{
    [SerializeField]
    Collider2D Player;

    public StaminaSlider slider;
    public Player player;

    public bool onShield = false;
    public bool onWind = false;

    public Name state;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"플레이어가 {state}을 획득했습니다.");

            if (state == Name.acorn)
            {
                if(slider.stamina >= slider.fullStamina * 0.85)
                {
                    slider.stamina = slider.fullStamina;
                }
                else
                {
                    slider.stamina += slider.fullStamina * 15/100;
                }
                gameObject.SetActive(false);
            }
            if (state == Name.Bell)
            {
                onShield = true;
                player.isInvincible = true;
                gameObject.SetActive(false);
            }
            if (state==Name.Wind)
            {
                StartCoroutine(Wind());
            }
        }
    }

    private IEnumerator Wind()
    {
        onWind = true;
        yield return new WaitForSeconds(1f);
        onWind = false;
        gameObject.SetActive(false);
    }

}
