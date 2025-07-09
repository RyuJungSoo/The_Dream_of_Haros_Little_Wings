using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum EAudioMixerType { Master, BGM, SFX}
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField] private AudioMixer audioMixer;

    private bool[] isMute = new bool[3]; // 음소거 여부
    private float[] audioVolumes = new float[3]; // 볼륨 

    private void Awake()
    {
        if (instance == null)
            instance = this;
        
        else
            Destroy(this.gameObject);
            
    }

    public void SetAudioVolume(EAudioMixerType audioMixerType, float volume)
    {
        
        // 오디오 믹서의 값은 -80 ~ 0 까지이기 때문에 0.0001 ~ 1의 Log10 * 20을 한다.
        audioMixer.SetFloat(audioMixerType.ToString(), Mathf.Log10(volume) * 20);
    }


    public float GetAudioVolume(EAudioMixerType audioMixerType)
    {
        audioMixer.GetFloat(audioMixerType.ToString(), out float curVolume);
        return Mathf.Pow(10, curVolume / 20);
    }
    public void SetAudioMute(EAudioMixerType audioMixerType)
    {
        int type = (int)audioMixerType; // 인덱스 가져오기

        // 음소거
        if (!isMute[type])
        {
            isMute[type] = true;
            audioMixer.GetFloat(audioMixerType.ToString(), out float curVolume);
            audioVolumes[type] = curVolume; // 현재 볼륨값 임시 저장
            SetAudioVolume(audioMixerType, 0.0001f);
        }
        else
        {
            isMute[type] = false;
            SetAudioVolume(audioMixerType, audioVolumes[type]);
        }
    }

    
}
