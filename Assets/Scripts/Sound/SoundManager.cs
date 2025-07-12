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

    [SerializeField]
    private AudioSource BGM; // BGM
    [SerializeField]
    private AudioSource SFX; // SFX

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        else
            Destroy(this.gameObject);
            
    }

    private void Start()
    {
        BGM = transform.GetChild(0).GetComponent<AudioSource>();
        SFX = transform.GetChild(1).GetComponent<AudioSource>();
    }

    public void SetAudioVolume(EAudioMixerType audioMixerType, float volume) // 볼륨 값 Set
    {
        
        // 오디오 믹서의 값은 -80 ~ 0 까지이기 때문에 0.0001 ~ 1의 Log10 * 20을 한다.
        audioMixer.SetFloat(audioMixerType.ToString(), Mathf.Log10(volume) * 20);
    }

    public float GetAudioVolume(EAudioMixerType audioMixerType) // 볼륨 값 Get
    {
        audioMixer.GetFloat(audioMixerType.ToString(), out float curVolume);
        return Mathf.Pow(10, curVolume / 20);
    }

    public void SetAudioMute(EAudioMixerType audioMixerType) // 음소거 토글
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

    public void PlayBGM(int index, bool isStage) // BGM 재생
    {
        AudioClip Clip;
        if (!isStage)
            Clip = GetComponent<SoundSource>().GetBGM(index);
        else
            Clip = GetComponent<SoundSource>().GetStageBGM(index);

        BGM.clip = Clip;
        BGM.Play();
    }

    public void PlaySFX(int index, float delay) // SFX 재생
    {
        
        StartCoroutine(PlaySFXWithDelay(index, delay));
        
    }

    IEnumerator PlaySFXWithDelay(int index, float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioClip Clip = GetComponent<SoundSource>().GetSFX(index);
        SFX.PlayOneShot(Clip);
        Debug.Log("OK");
    }
}



