using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum EAudioMixerType { Master, BGM, SFX}
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField] private AudioMixer audioMixer;

    private bool[] isMute = new bool[3]; // ���Ұ� ����
    private float[] audioVolumes = new float[3]; // ���� 

    private void Awake()
    {
        if (instance == null)
            instance = this;
        
        else
            Destroy(this.gameObject);
            
    }

    public void SetAudioVolume(EAudioMixerType audioMixerType, float volume)
    {
        
        // ����� �ͼ��� ���� -80 ~ 0 �����̱� ������ 0.0001 ~ 1�� Log10 * 20�� �Ѵ�.
        audioMixer.SetFloat(audioMixerType.ToString(), Mathf.Log10(volume) * 20);
    }


    public float GetAudioVolume(EAudioMixerType audioMixerType)
    {
        audioMixer.GetFloat(audioMixerType.ToString(), out float curVolume);
        return Mathf.Pow(10, curVolume / 20);
    }
    public void SetAudioMute(EAudioMixerType audioMixerType)
    {
        int type = (int)audioMixerType; // �ε��� ��������

        // ���Ұ�
        if (!isMute[type])
        {
            isMute[type] = true;
            audioMixer.GetFloat(audioMixerType.ToString(), out float curVolume);
            audioVolumes[type] = curVolume; // ���� ������ �ӽ� ����
            SetAudioVolume(audioMixerType, 0.0001f);
        }
        else
        {
            isMute[type] = false;
            SetAudioVolume(audioMixerType, audioVolumes[type]);
        }
    }

    
}
