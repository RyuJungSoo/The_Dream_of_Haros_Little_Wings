using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

/// 오디오 믹서 그룹 종류
public enum EAudioMixerType { Master, BGM, SFX }

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField] private AudioMixer audioMixer;

    private bool[]  isMute = new bool[3]; // 뮤트 상태
    private float[] audioVolumes = new float[3]; // 볼륨 dB값 저장

    [SerializeField]
    private AudioSource BGM; // BGM
    [SerializeField]
    private AudioSource SFX; // SFX
    [SerializeField]
    private AudioSource ScriptSound; // 스크립트 출력 효과음 재생 전용 AudioSource

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        BGM = transform.GetChild(0).GetComponent<AudioSource>();
        SFX = transform.GetChild(1).GetComponent<AudioSource>();
    }

    public void SetAudioVolume(EAudioMixerType audioMixerType, float volume) // 볼륨 값 Set
    {
        // AudioMixer는 dB(-80~0)를 받으므로 선형값(0.0001~1)을 20*log10(volume)로 변환
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        audioMixer.SetFloat(audioMixerType.ToString(), Mathf.Log10(volume) * 20f);
    }

    public float GetAudioVolume(EAudioMixerType audioMixerType) // 볼륨 값 Get
    {
        audioMixer.GetFloat(audioMixerType.ToString(), out float curDb);
        return Mathf.Pow(10f, curDb / 20f);
    }

    public void SetAudioMute(EAudioMixerType audioMixerType) // 뮤트 토글
    {
        int type = (int)audioMixerType; // 인덱스 구분용

        if (!isMute[type]) // 뮤트 ON
        {
            isMute[type] = true;
            audioMixer.GetFloat(audioMixerType.ToString(), out float curDb);
            audioVolumes[type] = curDb;       // 현재 dB 임시 저장
            SetAudioVolume(audioMixerType, 0.0001f); // 거의 무음
        }
        else // 뮤트 OFF
        {
            isMute[type] = false;
            audioMixer.SetFloat(audioMixerType.ToString(), audioVolumes[type]); // dB 복원
        }
    }

    public void PlayBGM(int index, bool isStage) // BGM 재생
    {
        AudioClip clip = !isStage
            ? GetComponent<SoundSource>().GetBGM(index)
            : GetComponent<SoundSource>().GetStageBGM(index);

        BGM.clip = clip;
        BGM.Play();
    }

    public void StopBGM() // BGM 정지
    {
        BGM.Stop();
    }

    public void PlayScriptSFX()
    {
        //if (ScriptSound.isPlaying)
            //return;

        int index = Random.Range(0, 12);

        ScriptSound.pitch = Random.Range(0.95f, 1.05f);
        ScriptSound.clip = GetComponent<SoundSource>().GetScriptSFX(index);
        ScriptSound.Play();
    }

    public void StopScriptSFX()
    {
        if (!ScriptSound.isPlaying)
            return;
        ScriptSound.Stop();
    }

    public void PlaySFX(int index, float delay) // SFX 재생(지연 지원)
    {
        StartCoroutine(PlaySFXWithDelay(index, delay));
    }

    private IEnumerator PlaySFXWithDelay(int index, float delay) // SFX 코루틴
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);
        AudioClip clip = GetComponent<SoundSource>().GetSFX(index);
        SFX.PlayOneShot(clip);
        Debug.Log("OK");
    }
}
