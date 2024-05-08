using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public enum EffectType
{
    EXPLOSION
}

class EffectBundle
{
    public GameObject _effectObject;
    public ParticleSystem _effectParticle;
    public AudioSource _audioSource;
    private AudioClip _audioClip;

    public EffectBundle(GameObject v_effectObject, ParticleSystem v_effectParticle, AudioSource audioSource)
    {
        _effectObject = v_effectObject;
        _effectParticle = v_effectParticle;
        _audioSource = audioSource;             // 이펙트 재상 사운드

        _audioClip = SoundManager.Instance._audioClip_SFX[(int)SFXClip.EXPLOSION];  // 이펙트 사운드 클립
    }

    /// <summary>
    /// v_effectPosition 위치에 이펙트를 실행시킴
    /// </summary>
    /// <param name="v_effectPosition"></param>
    public void F_UseEffect(Vector3 v_effectPosition)
    {
        _effectObject.transform.position = v_effectPosition;
        _effectParticle.Play();
        
        if(_audioSource != null)
        {
            _audioSource.PlayOneShot(_audioClip, SoundManager.Instance.volume_SFX);
        }
    }
}

public class ResourceManager : Singleton<ResourceManager>
{
    string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

    [Header("Sprite")]
    [SerializeField] private Sprite _emptySlotSprite;
    [SerializeField] private List<Sprite> _inventorySprites;
    public Sprite emptySlotSprite => _emptySlotSprite;

    [Header("install Item Intercation TEXT")]
    [SerializeField] private Dictionary<string, string> _intercationTEXT_install;


    [Header("FX")]
    [SerializeField] private Transform _particleTransform;
    [SerializeField] private GameObject[] _particlePrefabs;
    private List<Queue<EffectBundle>> _particlePooling;
    protected override void InitManager() 
    {
        F_InitIntercationTEXTData();
        F_InitEffectObjects();
    }

    #region Sprite
    public Sprite F_GetInventorySprite(int v_code)
    {
        return _inventorySprites[v_code];
    }
    #endregion

    #region TEXT
    private void F_InitIntercationTEXTData()
    {
        _intercationTEXT_install = new Dictionary<string, string>();

        TextAsset data = Resources.Load("IntercationTEXT") as TextAsset;

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);
        var header = Regex.Split(lines[0], SPLIT_RE);

        for(int i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);

            string objectName = values[0];
            string eng = values[1];
            string kor = values[2];

            _intercationTEXT_install.Add(objectName, eng);
        }
    }

    public string F_GetIntercationTEXT(string v_objectName)
    {
        return _intercationTEXT_install[v_objectName];
    }
    #endregion

    #region
    private void F_InitEffectObjects()
    {
        _particlePooling = new List<Queue<EffectBundle>>();

        foreach (GameObject effect in _particlePrefabs)
        {
            Queue<EffectBundle> queue = new Queue<EffectBundle>();
            for(int i = 0; i < 3; i++)
            {
                GameObject effectObject = Instantiate(effect, _particleTransform);
                ParticleSystem effectParticleSystem = effectObject.GetComponent<ParticleSystem>();
                AudioSource audioSource = effectObject.GetComponent<AudioSource>();

                queue.Enqueue(new EffectBundle(effectObject, effectParticleSystem, audioSource));
            }
            _particlePooling.Add(queue);
        }
    }
    
    public void F_GetEffect(EffectType v_type, Vector3 v_position)
    {
        if (_particlePooling[(int)v_type].Count <= 0)
        {
            GameObject effectObject = Instantiate(_particlePrefabs[(int)v_type], _particleTransform);
            ParticleSystem effectParticleSystem = effectObject.GetComponent<ParticleSystem>();
            AudioSource audioSource = effectObject.GetComponent<AudioSource>();

            _particlePooling[(int)v_type].Enqueue(new EffectBundle(effectObject, effectParticleSystem, audioSource));
        }

        EffectBundle effect = _particlePooling[(int)v_type].Dequeue();
        effect.F_UseEffect(v_position);
        StartCoroutine(C_ReturnEffect(effect, v_type));
    }

    IEnumerator C_ReturnEffect(EffectBundle v_effect, EffectType v_type)
    {
        while (v_effect._effectParticle.isPlaying)  //실행되고있는동안 지연
            yield return null;

        _particlePooling[(int)v_type].Enqueue(v_effect);
    }
    #endregion
}
