using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [Header("Sound option")]
    [SerializeField] private Slider _silder_MasterVolume;
    [SerializeField] private Slider _silder_BGMVolume;
    [SerializeField] private Slider _silder_SFXVolume;

    private void Start()
    {
        F_initSoundSliders();
    }

    #region Sound
    private void F_initSoundSliders()
    {
        _silder_MasterVolume.value = SoundManager.Instance.masterVolume;
        _silder_BGMVolume.value = SoundManager.Instance.bgmVolume;
        _silder_SFXVolume.value = SoundManager.Instance.sfxVolume;

        //함수 바인딩
        _silder_MasterVolume.onValueChanged.AddListener(
            delegate {
                SoundManager.Instance.F_ChangedVolume(VolumeType.MASTER, _silder_MasterVolume.value);
            });

        _silder_BGMVolume.onValueChanged.AddListener(
            delegate {
                SoundManager.Instance.F_ChangedVolume(VolumeType.BGM, _silder_BGMVolume.value);
            });

        _silder_SFXVolume.onValueChanged.AddListener(
            delegate {
                SoundManager.Instance.F_ChangedVolume(VolumeType.SFX, _silder_SFXVolume.value);
            });
    }
    #endregion
}
