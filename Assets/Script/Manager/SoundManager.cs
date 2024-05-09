using System;
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
    [SerializeField] private AudioSource _audioSource_BGM_player;
    [SerializeField] private AudioSource _audioSource_SFX_player;

    [Header("AudioClips")]
    [SerializeField] public AudioClip[] _audioClip_BGM;
    [SerializeField] public AudioClip[] _audioClip_SFX;
    [SerializeField] public AudioClip[] _audioClip_UI;
    
    [Header("Volumes")]
    [SerializeField] private float _masterVolume;
    [SerializeField] private float _bgmVolume;
    [SerializeField] private float _sfxVolume;

    IEnumerator _bgmCoroutine;
    public float masterValue => _masterVolume;
    public float bgmValue => _bgmVolume;
    public float sfxValue => _sfxVolume;

    public float volume_BGM => _bgmVolume * _masterVolume;
    public float volume_SFX => _sfxVolume * _masterVolume;

    protected override void InitManager()
    {
        _bgmCoroutine = C_PlayBGMTrack();

        // !!사운드 값 저장/불러오기 필요!!
        _masterVolume = 1f;
        _bgmVolume = 0.5f;
        _sfxVolume = 0.5f;
    }

    private void Start()
    {
        // BGM 시작 ( 임시 )
        StartCoroutine(_bgmCoroutine);
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

        F_UpdateBGMVolumes();
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

    private void F_UpdateBGMVolumes()
    {
        // 1. BGM 볼륨 업데이트 
        _audioSource_BGM_player.volume = volume_BGM;
    }

    public void F_PlaySFX(SFXClip v_clip)
    { 
        _audioSource_SFX_player.PlayOneShot(_audioClip_SFX[(int)v_clip], volume_SFX);
    }

    public void F_ChangeBGM(BGMClip v_clip)
    {
        _audioSource_BGM_player.clip = _audioClip_BGM[(int)v_clip];
    }

    /// <summary> 임시 브금 로테이션 </summary>
    IEnumerator C_PlayBGMTrack()
    {
        int idx = 0;
        int maxIndex = Enum.GetNames(typeof(BGMClip)).Length;

        for(int i = 0; i < maxIndex; i++)
        {
            // 1. BGM 클립 변경
            F_ChangeBGM((BGMClip)i);

            // 2. BGM 재싱
            _audioSource_BGM_player.Play();

            // 2. BGM 길이만큼 대기
            yield return new WaitForSeconds(_audioSource_BGM_player.clip.length + 5f);
        }

        // 모든 트랙을 실행하고 60초후 새로운 트랙 실행
        yield return new WaitForSeconds(60f);   
        StartCoroutine(_bgmCoroutine);
    }
}
