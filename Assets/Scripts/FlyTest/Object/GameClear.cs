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

    public void Gameclear()
        {
            clear.SetActive(true);
        player.rb.constraints = RigidbodyConstraints2D.FreezeAll;
        player.anim.enabled = false;
        slider.stopStaminaBar = true;
    }
}
