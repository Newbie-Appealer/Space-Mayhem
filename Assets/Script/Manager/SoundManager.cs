using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = Unity.Random;

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
    [SerializeField] private float _masterVolume = 0.5f;
    [SerializeField] private float _bgmVolume = 0.5f;
    [SerializeField] private float _sfxVolume = 0.5f;

    IEnumerator _bgmCoroutine;
    public float masterValue {  get=> _masterVolume; set => _masterVolume = value; }
    public float bgmValue { get => _bgmVolume; set => _bgmVolume = value; }
    public float sfxValue { get => _sfxVolume; set => _sfxVolume = value; }

    public float volume_BGM => _bgmVolume * _masterVolume;
    public float volume_SFX => _sfxVolume * _masterVolume;

    protected override void InitManager()
    {
        _bgmCoroutine = C_PlayBGMTrack();
    }

    private void Start()
    {
        F_StartSpaceShipBGM();
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
        // 1. BGM ���� ������Ʈ 
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


    IEnumerator C_PlayBGMTrack()
    {
        _audioSource_BGM_player.loop = false;
        int maxIndex = Enum.GetNames(typeof(BGMClip)).Length;

        for(int i = 0; i < maxIndex; i++)
        {
            // 1. BGM Ŭ�� ����
            F_ChangeBGM((BGMClip)i);

            // 2. BGM ���
            _audioSource_BGM_player.Play();

            // 2. BGM ��� �Ǵµ��� �����, ����� ����ǰ��� 3���� ���� Ŭ�� ���
            yield return new WaitWhile(() => _audioSource_BGM_player.isPlaying);
            yield return new WaitForSeconds(3f);
        }

        // ��� Ʈ���� �����ϰ� 60���� ���ο� Ʈ�� ����
        yield return new WaitForSeconds(60f);   
        StartCoroutine(_bgmCoroutine);
    }

    // �⺻ BGM�� ���������� �����̼� 
    // �ܺθ� BGM ��� ( Loop ) �ܺθ� BGM ���� 1
    // ���θ� BGM ��� ( Loop ) ���θ� BGM ���� 1

    public void F_StartSpaceShipBGM()
    {
        // ���ּ� BGM �����̼�
        StartCoroutine(_bgmCoroutine);
    }
    public void F_StartOUTSideBGM()
    {
        // 1. BGM �ʱ�ȭ
        StopCoroutine(_bgmCoroutine);           // ���ּ� BGM �����̼� ����
        _audioSource_BGM_player.Stop();         // BGM ����
        _audioSource_BGM_player.loop = true;    // Loop ON

        // 2. BGM Clip ����
        BGMClip[] clips = { BGMClip.OUTSIDE_WORLD1, BGMClip.OUTSIDE_WORLD2 };
        BGMClip clip = clips[Random.range(0, clips.Length)];
        F_ChangeBGM(clip);

        // 3. ���
        _audioSource_BGM_player.Play();
    }
    public void F_StartINSideBGM()
    {
        // ���ּ� BGM 
        StopCoroutine(_bgmCoroutine);           // ���ּ� BGM �����̼� ����
        _audioSource_BGM_player.Stop();         // BGM ����
        _audioSource_BGM_player.loop = true;    // Loop ON

        // 2. BGM Clip ����
        BGMClip clip = BGMClip.INSIDE_DUNGEON;
        F_ChangeBGM(clip);

        // 3. ���
        _audioSource_BGM_player.Play();
    }
}
