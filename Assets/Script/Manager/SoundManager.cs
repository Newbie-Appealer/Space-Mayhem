using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VolumeType
{
    MASTER,
    BGM,
    SFX
}

public class SoundManager : Singleton<SoundManager>
{
    [Header("AudioSource")]
    [SerializeField] private AudioSource _audioSource_BGM;

    [Header("AudioClips")]
    [SerializeField] private AudioClip[] _audioClip_BGM;

    [Header("Volumes")]
    [SerializeField] private float _masterVolume;
    [SerializeField] private float _bgmVolume;
    [SerializeField] private float _sfxVolume;

    public float masterVolume => _masterVolume;
    public float bgmVolume => _bgmVolume;
    public float sfxVolume => _sfxVolume;

    private float volume_BGM => _bgmVolume * _masterVolume;
    private float volume_SFX => _sfxVolume * _masterVolume;

    protected override void InitManager()
    {
        // !!사운드 값 저장/불러오기 필요!!
        _masterVolume = 0.5f;
        _bgmVolume = 0.5f;
        _sfxVolume = 0.5f;
    }

    public void F_ChangedVolume(VolumeType v_type, float v_volume)
    {
        switch (v_type)
        {
            case VolumeType.MASTER:
                _masterVolume = v_volume;
                break;

            case VolumeType.BGM:
                _bgmVolume = v_volume;
                break;

            case VolumeType.SFX:
                _sfxVolume = v_volume;
                break;
        }

        F_UpdateVolumes();
    }

    public float F_GetVolume(VolumeType v_type)
    {
        switch (v_type)
        {
            case VolumeType.BGM:
                return volume_BGM;

            case VolumeType.SFX:
                return volume_SFX;
        }
        return 0;
    }

    private void F_UpdateVolumes()
    {
        // 1. BGM 볼륨 업데이트 
        _audioSource_BGM.volume = bgmVolume;

        // 2. SFX 볼륨 업데이트
        // 흠.. 아직 SFX 넣을게없군
    }
}
