using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameClear : MonoBehaviour
{
    [SerializeField]
    Player player;

    [SerializeField]
    public GameObject clear;

    [SerializeField]
    StaminaSlider slider;

    public void Stage1Clear()
        {
        clear.SetActive(true);
        SceneSettingManager.Instance.SetisStageClear(1,true);
        player.rb.constraints = RigidbodyConstraints2D.FreezeAll;
        player.anim.enabled = false;
        slider.stopStaminaBar = true;
        }
    public void Stage2Clear()
    {
        clear.SetActive(true);
        SceneSettingManager.Instance.SetisStageClear(2, true);
        player.rb.constraints = RigidbodyConstraints2D.FreezeAll;
        player.anim.enabled = false;
        slider.stopStaminaBar = true;
    }
}
