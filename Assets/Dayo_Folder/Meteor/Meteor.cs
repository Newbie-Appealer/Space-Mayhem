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
    
    //얘는 어디 쓰는걸까
    //[SerializeField] private LayerMask _meteorAttack_Layer;

    private Rigidbody _rb;
    private float _meteor_moveSpeed;

    private float _targetX; 
    private float _targetY;
    private float _targetZ;

    // 메테오의 풀링 번호
    private int _poolingNumber;

    //획득 시 아이템 코드
    private int _itemCode = 3;

    //플레이어 주변 범위
    private float _player_Sphere_Radius;

    #region 운석 초기화
    public void F_SettingMeteor(int v_index)
    {
        _rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        _audioClip = SoundManager.Instance._audioClip_SFX[(int)SFXClip.EXPLOSION];

        _player_Sphere_Radius = MeteorManager.Instance.player_SphereCollider.radius;    // 범위 설정
        gameObject.name = "Meteor";                                                     // 오브젝트 이름 설정

        _poolingNumber = v_index;
    }
    #endregion

    #region 메테오 움직임
    public void F_MoveMeteor()
    {
        // 플레이어 주변 구를 기준으로 그 안의 범위 랜덤한 좌표 뽑기
        float _targetX = Random.Range(-_player_Sphere_Radius, _player_Sphere_Radius);
        float _targetY = Random.Range(-_player_Sphere_Radius, _player_Sphere_Radius);
        float _targetZ = Random.Range(-_player_Sphere_Radius, _player_Sphere_Radius);
        Vector3 _targetDirection = (new Vector3(_targetX, _targetY, _targetZ) - transform.position).normalized;
        _meteor_moveSpeed = Random.Range(5f, 10f);
        _rb.velocity = _targetDirection * _meteor_moveSpeed;
        StartCoroutine(C_MeteorDistanceCheck(gameObject));
    }

    //메테오와 플레이어 주변 원 사이 거리 측정
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

    #region 운석 획득
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

    #region 운석 충돌
    //운석 충돌
    public IEnumerator F_CrashBlock()
    {
        _audioSource.PlayOneShot(_audioClip, SoundManager.Instance.volume_SFX);

        //폭발 이펙트
        _rb.useGravity = true;
        _meteor_ExplosionEffect.SetActive(true);
        yield return new WaitForSeconds(1f);

        //폭발 이펙트 점점 줄이기
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
        //운석 변경된 정보 초기화
        MeteorManager.Instance.F_ReturnMeteor(this, _poolingNumber);
    }

    private void F_ResetMeteor()
    {
        // 이펙트 초기화
        _meteor_Effect.transform.localScale = new Vector3(3f, 3f, 3f);
        _meteor_ExplosionEffect.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        _meteor_ExplosionEffect.SetActive(false);

        //메테오 움직임 초기화
        _rb.useGravity = false;
        _rb.velocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(F_CrashBlock());
        
        if( collision.gameObject.layer == BuildMaster.Instance._buildFinishedint )      // 레이어 검사는 int 
        {
            // 구조물의 체력을 깍음
            collision.transform.parent.GetComponent<MyBuildingBlock>().F_CrashMeteor();
        }
    }
    #endregion
    
    public void F_StopMeteorCoroutine()
    {
        F_ResetMeteor();        // 리셋 후
        StopAllCoroutines();    // 코루틴 삭제
    }
}
