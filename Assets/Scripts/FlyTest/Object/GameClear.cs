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
        ClearSound();
        clear.SetActive(true);
        SceneSettingManager.Instance.SetisStageClear(1,true);
        SceneSettingSaver.Instance.SaveSceneData(); // 클리어 여부저장 추가 : 민서
        SaveManager.Instance.ResetGameTurn();
        player.rb.constraints = RigidbodyConstraints2D.FreezeAll;
        player.anim.enabled = false;
        slider.stopStaminaBar = true;
        }
    public void Stage2Clear()
    {
        ClearSound();
        clear.SetActive(true);
        SceneSettingManager.Instance.SetisStageClear(2, true);
        SceneSettingSaver.Instance.SaveSceneData(); // 클리어 여부저장 추가 : 민서
        SaveManager.Instance.ResetGameTurn();
        player.rb.constraints = RigidbodyConstraints2D.FreezeAll;
        player.anim.enabled = false;
        slider.stopStaminaBar = true;
    }

    private void ClearSound()
    {
        SoundManager.instance.StopBGM();
        SoundManager.instance.PlaySFX(5, 0);
        SoundManager.instance.PlayBGM(4, false);
    }
}
