using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class VolumeBar : MonoBehaviour
{
    [SerializeField]
    private string MixerTypeName;

    public void Mute()
    {
        if (MixerTypeName == "Master" || MixerTypeName == "master")
            SoundManager.instance.SetAudioMute(EAudioMixerType.Master);
        else if (MixerTypeName == "BGM" || MixerTypeName == "bgm")
            SoundManager.instance.SetAudioMute(EAudioMixerType.BGM);
        else if (MixerTypeName == "SFX" || MixerTypeName == "sfx")
            SoundManager.instance.SetAudioMute(EAudioMixerType.SFX);
        else
            return;
    }

    public void ChangeVolume()
    {
        float volume = GetComponent<Slider>().value;
        if (MixerTypeName == "Master" || MixerTypeName == "master")
            SoundManager.instance.SetAudioVolume(EAudioMixerType.Master, volume);
        else if (MixerTypeName == "BGM" || MixerTypeName == "bgm")
            SoundManager.instance.SetAudioVolume(EAudioMixerType.BGM, volume);
        else if (MixerTypeName == "SFX" || MixerTypeName == "sfx")
            SoundManager.instance.SetAudioVolume(EAudioMixerType.SFX, volume);
        else
            return;
    }
}
