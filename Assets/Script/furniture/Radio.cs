using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : Furniture
{
    [SerializeField] private AudioSource _radioAudioSource;
    [SerializeField] private AudioClip[] _radioAudioClip;
    protected override void F_InitFurniture() 
    { 
        _radioAudioSource = GetComponent<AudioSource>();
    }

    public override void F_Interaction()
    {
        if (_radioAudioSource.isPlaying)
            return;

        // 볼륨 받아오기
        float volume = SoundManager.Instance.F_GetVolume(VolumeType.SFX);
        // 클립 랜덤 선택
        AudioClip clip = _radioAudioClip[Random.Range(0,_radioAudioClip.Length)];
        // 재생
        _radioAudioSource.PlayOneShot(clip,volume);
    }

    #region 저장 / 불러오기
    public override string F_GetData()
    {
        return "NONE";    
    }

    public override void F_SetData(string v_data)
    {
        
    }
    #endregion
}
