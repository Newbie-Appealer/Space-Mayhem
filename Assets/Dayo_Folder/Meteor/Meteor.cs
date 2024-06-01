using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    private AudioSource _audioSource;
    private AudioClip _audioClip;

    [Header("=== ABOUT METEOR ===")]
    [SerializeField, Range(300f, 500f)] private float _meteor_Distance = 300f;
    [SerializeField] private GameObject _meteor_Effect;
    [SerializeField] private GameObject _meteor_ExplosionEffect;
    
    //��� ��� ���°ɱ�
    //[SerializeField] private LayerMask _meteorAttack_Layer;

    private Rigidbody _rb;
    private float _meteor_moveSpeed;

    private float _targetX; 
    private float _targetY;
    private float _targetZ;

    // ���׿��� Ǯ�� ��ȣ
    private int _poolingNumber;

    //ȹ�� �� ������ �ڵ�
    private int _itemCode = 3;

    //�÷��̾� �ֺ� ����
    private float _player_Sphere_Radius;

    #region � �ʱ�ȭ
    public void F_SettingMeteor(int v_index)
    {
        _rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        _audioClip = SoundManager.Instance._audioClip_SFX[(int)SFXClip.EXPLOSION];

        _player_Sphere_Radius = MeteorManager.Instance.player_SphereCollider.radius;    // ���� ����
        gameObject.name = "Meteor";                                                     // ������Ʈ �̸� ����

        _poolingNumber = v_index;
    }
    #endregion

    #region ���׿� ������
    public void F_MoveMeteor()
    {
        // �÷��̾� �ֺ� ���� �������� �� ���� ���� ������ ��ǥ �̱�
        float _targetX = Random.Range(-_player_Sphere_Radius, _player_Sphere_Radius);
        float _targetY = Random.Range(-_player_Sphere_Radius, _player_Sphere_Radius);
        float _targetZ = Random.Range(-_player_Sphere_Radius, _player_Sphere_Radius);
        Vector3 _targetDirection = (new Vector3(_targetX, _targetY, _targetZ) - transform.position).normalized;
        _meteor_moveSpeed = Random.Range(5f, 10f);
        _rb.velocity = _targetDirection * _meteor_moveSpeed;
        StartCoroutine(C_MeteorDistanceCheck(gameObject));
    }

    //���׿��� �÷��̾� �ֺ� �� ���� �Ÿ� ����
    private IEnumerator C_MeteorDistanceCheck(GameObject v_Meteor) 
    {
        Transform _PlayerSphere;
        while(gameObject.activeSelf)
        {
            _PlayerSphere = MeteorManager.Instance.player_SphereCollider.transform;
            if (Vector3.Distance(_PlayerSphere.position, v_Meteor.transform.position) > _meteor_Distance)
            {
                MeteorManager.Instance.F_ReturnMeteor(this, _poolingNumber);
            }
            yield return new WaitForSeconds(3f);
        }
    }
    #endregion

    #region � ȹ��
    public int F_SettingItemCode()
    {
        _itemCode = 3;
        float _randomChance = Mathf.Floor(Random.value * 100);
        for (int l = 0; l < MeteorManager.Instance._drop_Chance.Length; l++)
        {
            if (_randomChance < MeteorManager.Instance._drop_Chance[l])
                return _itemCode;
            else
            {
                _randomChance -= MeteorManager.Instance._drop_Chance[l];
                _itemCode++;
            }
        }
        return _itemCode = 7;
    }

    public void F_GetMeteor(int v_itemCode)
    {
        ItemManager.Instance.inventorySystem.F_GetItem(v_itemCode);
        ItemManager.Instance.inventorySystem.F_InventoryUIUpdate();
        F_ResetMeteor();
        MeteorManager.Instance.F_ReturnMeteor(this,_poolingNumber);
    }
    #endregion

    #region � �浹
    //� �浹
    public IEnumerator F_CrashBlock()
    {
        _audioSource.PlayOneShot(_audioClip, SoundManager.Instance.volume_SFX);

        //���� ����Ʈ
        _rb.useGravity = true;
        _meteor_ExplosionEffect.SetActive(true);
        yield return new WaitForSeconds(1f);

        //���� ����Ʈ ���� ���̱�
        float _scaleX = _meteor_Effect.transform.localScale.x;
        float _scaleY = _meteor_Effect.transform.localScale.y;
        float _scaleZ = _meteor_Effect.transform.localScale.z;
        float _explosionScaleX = _meteor_ExplosionEffect.transform.localScale.x;
        float _explosionScaleY = _meteor_ExplosionEffect.transform.localScale.y;
        float _explosionScaleZ = _meteor_ExplosionEffect.transform.localScale.z;

        while (_scaleX > 0.001f)
        {
            _scaleX -= 0.01f;
            _scaleY -= 0.01f;
            _scaleZ -= 0.01f;
            _explosionScaleX -= 0.001f;
            _explosionScaleY -= 0.001f;
            _explosionScaleZ -= 0.001f;
            _meteor_Effect.transform.localScale = new Vector3(_scaleX, _scaleY, _scaleZ);   
            _meteor_ExplosionEffect.transform.localScale = new Vector3(_explosionScaleX, _explosionScaleY, _explosionScaleZ);   
            yield return new WaitForSeconds(0.001f);
        }

        yield return new WaitForSeconds(20f);
        //� ����� ���� �ʱ�ȭ
        MeteorManager.Instance.F_ReturnMeteor(this, _poolingNumber);
    }

    private void F_ResetMeteor()
    {
        // ����Ʈ �ʱ�ȭ
        _meteor_Effect.transform.localScale = new Vector3(3f, 3f, 3f);
        _meteor_ExplosionEffect.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        _meteor_ExplosionEffect.SetActive(false);

        //���׿� ������ �ʱ�ȭ
        _rb.useGravity = false;
        _rb.velocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(F_CrashBlock());
        
        if( collision.gameObject.layer == BuildMaster.Instance._buildFinishedint )      // ���̾� �˻�� int 
        {
            // �������� ü���� ����
            collision.transform.parent.GetComponent<MyBuildingBlock>().F_CrashMeteor();
        }
    }
    #endregion
    
    public void F_StopMeteorCoroutine()
    {
        F_ResetMeteor();        // ���� ��
        StopAllCoroutines();    // �ڷ�ƾ ����
    }
}
