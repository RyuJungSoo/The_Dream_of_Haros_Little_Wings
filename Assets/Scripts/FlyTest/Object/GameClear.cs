using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameClear : MonoBehaviour
{
    public Player player;

    public GameObject clear;

    [SerializeField]
    StaminaSlider slider;

    public void Stage1Clear()
    {
        SoundManager.instance.PlaySFX(5, 0);
        clear.SetActive(true);
        SceneSettingManager.Instance.SetisStageClear(1,true);
        player.rb.constraints = RigidbodyConstraints2D.FreezeAll;
        player.anim.enabled = false;
        slider.stopStaminaBar = true;
        }
    public void Stage2Clear()
    {
        SoundManager.instance.PlaySFX(5, 0);
        clear.SetActive(true);
        SceneSettingManager.Instance.SetisStageClear(2, true);
        player.rb.constraints = RigidbodyConstraints2D.FreezeAll;
        player.anim.enabled = false;
        slider.stopStaminaBar = true;
    }
}
