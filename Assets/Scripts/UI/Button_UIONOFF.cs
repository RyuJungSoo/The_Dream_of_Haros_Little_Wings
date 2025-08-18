using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Button_UIONOFF : MonoBehaviour
{
    [SerializeField]
    private GameObject UI;
    [SerializeField]
    private bool isOn;

    public void UI_ONFF()
    {
        SoundManager.instance.PlaySFX(11,0);
        UI.SetActive(isOn);
    }
}
