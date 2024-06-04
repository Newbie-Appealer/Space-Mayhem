using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : Furniture
{
    private AudioSource _radioAudioSource;
    [SerializeField] private AudioClip _radioAudioClip;

    private IEnumerator _radioCoroutine;
    public float radioVolume => SoundManager.Instance.F_GetVolume(VolumeType.SFX);
    protected override void F_InitFurniture() 
    { 
        _radioAudioSource = GetComponent<AudioSource>();

        _radioAudioSource.clip = _radioAudioClip;

        _radioCoroutine = C_OnRadio();
    }

    public override void F_Interaction()
    {
        // 라디오 재생중일때 -> STOP
        if (_radioAudioSource.isPlaying)
        {
            _radioAudioSource.Stop();
            StopCoroutine(_radioCoroutine);
        }
        else
        {
            _radioAudioSource.Play();
            StartCoroutine(_radioCoroutine);
        }
    }

    IEnumerator C_OnRadio()
    {
        while(_radioAudioSource.isPlaying)
        {
            _radioAudioSource.volume = radioVolume;
            yield return new WaitForSeconds(1f);
        }
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
