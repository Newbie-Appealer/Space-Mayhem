using OpenCover.Framework.Model;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public enum TurretType
{
    MACHINEGUN,
    ROCKET
}

public class Turret : Furniture
{
    IEnumerator _turretAction;

    [Header("설정해줘!")]
    [SerializeField] private TurretType _turretType;        // 터렛 유형
    [SerializeField] private int _shootingDelay;            // 공격 딜레이
    [SerializeField] private float _shootingRange;          // 운석 감지 범위

    [SerializeField] private LayerMask _meteorLayer;        // 메테오 레이어
    [SerializeField] private LayerMask _obstacleLayer;      // 레이 장애물 레이어 ( 플레이어가 설치한 오브젝트 )

    [SerializeField] private Transform _rayPoint;           // 레이 발싸 위치
    [SerializeField] private Transform _rotateObject;       // 회전 오브젝트 ( 메테오 바라보기 )

    [Header("=== Effect ===")]
    [SerializeField] private ParticleSystem _shotEffect;    // 발싸 이펙트
    [SerializeField] private ParticleSystem _bulletEffect;  // 총알 이펙트

    [Header("=== turret information ===")]
    [SerializeField] private Collider[] _meteors;           // 감지된 운석 배열
    [SerializeField] private bool        _canShooting;


    private RaycastHit _rayHit;
    protected override void F_InitFurniture()
    {
        _turretAction = C_TurretAction();
        StartCoroutine(_turretAction);
    }

    IEnumerator C_TurretAction()
    {
        _canShooting = true;
        yield return new WaitForSeconds(_shootingDelay);

        while (true)
        {
            if (!_onEnergy)
                yield return new WaitForSeconds(_shootingDelay);
            
            if(_canShooting)
            {
                yield return null;

                _meteors = Physics.OverlapSphere(transform.position, _shootingRange, _meteorLayer);

                foreach (Collider meteor in _meteors)
                {
                    // 1. 메테오 바라보기
                    F_RotateTurret(meteor.transform);

                    // 2. 메테오 방향으로 오브젝트가 있는지 확인
                    if (F_RaycastObstacle())
                        continue;



                    // 3. 공격 ( 이펙트 실행 )
                    F_EffectPlay();

                    yield return new WaitForSeconds(0.5f);

                    // 4. 확률에 의한 운석 파괴
                    F_DestoryMeteor(meteor.transform);

                    // 탐지/공격 불가능 상태로 변경
                    _canShooting = false;
                    break;
                }
            }
            else
            {
                yield return new WaitForSeconds(_shootingDelay);
                _canShooting = true;
            }
        }
    }

    private void F_RotateTurret(Transform v_target)
    {
        Vector3 dir = v_target.transform.position - transform.position;
        _rotateObject.rotation = Quaternion.LookRotation(dir.normalized);
    }

    private bool F_RaycastObstacle()
    {
        return Physics.Raycast(_rayPoint.position, _rayPoint.forward, out _rayHit, 30, _obstacleLayer);
    }

    private void F_EffectPlay()
    {
        if (!_shotEffect.isPlaying)
            _shotEffect.Play();
        if (!_bulletEffect.isPlaying)
            _bulletEffect.Play();
    }

    private void F_DestoryMeteor(Transform v_meteor)
    {
        int rnd = Random.Range(0, 1000);
        switch(_turretType)
        {
            // 타입에 따른 확률 나중에 해줘야함!
            case TurretType.MACHINEGUN:
                if (rnd % 3 != 0)   // 대충 33%
                    return;
                break;
            case TurretType.ROCKET:
                if (rnd % 2 != 0)   // 대충 50%
                    return;
                break;
        }

        Meteor tmpMeteor = v_meteor.GetComponent<Meteor>();
        if (tmpMeteor != null)
        {
            if(tmpMeteor.gameObject.activeSelf)
            {
                int dropitemCode = tmpMeteor.F_SettingItemCode();
                tmpMeteor.F_GetMeteor(dropitemCode);
                ResourceManager.Instance.F_GetEffect(EffectType.EXPLOSION, tmpMeteor.transform.position);
            }
        }
    }

    public override void F_ChangeEnergyState(bool v_state)
    {
        _onEnergy = v_state;
    }

    #region 저장 / 불러오기 
    public override string F_GetData()
    {
        return "";
    }

    public override void F_SetData(string v_data)
    {

    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _shootingRange);
    }
}
