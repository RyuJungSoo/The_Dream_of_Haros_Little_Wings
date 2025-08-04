using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
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
    [SerializeField] Collider2D Player;
    [SerializeField] PlayerJump playerjump;
    [SerializeField] SpriteRenderer sr;

    public StaminaSlider slider;
    public Player player;

    public Name state;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"플레이어가 {state}을 획득했습니다.");

            if (state == Name.acorn)
            {
                if(slider.stamina >= slider.fullStamina * 0.75)
                {
                    slider.stamina = slider.fullStamina;
                }
                else
                {
                    slider.stamina += slider.fullStamina * 25/100;
                }
                gameObject.SetActive(false);
            }
            if (state == Name.Bell)
            {
                player.onShield = true;
                player.isInvincible = true;
                gameObject.SetActive(false);
            }
            if (state==Name.Wind)
            {
                sr.enabled = false;
                StartCoroutine(Wind());
            }
        }
    }

    private IEnumerator Wind()
    {
        Debug.Log("윈드 코루틴 실행");
        playerjump.onWind = true;
        yield return new WaitForSeconds(1f);
        playerjump.onWind = false;
        gameObject.SetActive(false);
    }

}
