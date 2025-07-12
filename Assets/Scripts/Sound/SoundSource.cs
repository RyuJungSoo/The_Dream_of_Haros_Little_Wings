using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSource : MonoBehaviour
{
    [SerializeField]
    List<AudioClip> BGM_List;
    [SerializeField]
    List<AudioClip> StageBGM_List;
    [SerializeField]
    List<AudioClip> SFX_List;

    public AudioClip GetBGM(int index)
    {  
        return BGM_List[index]; 
    }

    public AudioClip GetStageBGM(int index)
    {
        return StageBGM_List[index];
    }

    public AudioClip GetSFX(int index)
    {
        return SFX_List[index];
    }
}
