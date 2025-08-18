using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quit_Button : MonoBehaviour
{
    public void Quit()
    {
        SoundManager.instance.PlaySFX(11, 0);
        Application.Quit();
    }
}
