using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemName
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

    public ItemName itemState;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"플레이어가 {itemState}을 획득했습니다.");

            if (itemState == ItemName.acorn)
            {
                SoundManager.instance.PlaySFX(1, 0);
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
            if (itemState == ItemName.Bell)
            {
                SoundManager.instance.PlaySFX(2, 0);
                player.onShield = true;
                player.isInvincible = true;
                gameObject.SetActive(false);
            }
            if (itemState == ItemName.Wind)
            {
                SoundManager.instance.PlaySFX(3, 0);
                sr.enabled = false;
                StartCoroutine(Wind());
            }
        }
    }

    private IEnumerator Wind()
    {
        playerjump.onWind = true;
        yield return new WaitForSeconds(1f);
        playerjump.onWind = false;
        gameObject.SetActive(false);
    }

}
