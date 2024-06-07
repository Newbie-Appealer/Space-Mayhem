using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpider : Enemy
{
    private float _currentTime_Idle = 0f;
    private float _limitTime_Idle = 2f;

    private float _attackRange = 4.3f;

    [Header("Spider 모델링 버그 해결용")]
    [SerializeField] private Transform _parentObject;
    [SerializeField] private Transform[] _childObjects;

    [Header(" Hitbox ")]
    [SerializeField] private Transform _hitboxPosition;
    [SerializeField] private Vector3 _hitboxSize;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource_Move;
    [SerializeField] private AudioSource _audioSource_Attack;

    private bool _onAttack;
    protected override void F_EnemyInit()
    {
        _enemyType = EnemyType.MONSTER;

        _searchTargetRange = 15f;
        _randomTargetRange = 10f;

        foreach(Transform obj in _childObjects)
            obj.SetParent(_parentObject);

        // 사운드 관련 초기화
        _audioSource_Attack.clip = SoundManager.Instance._audioClip_SFX[(int)SFXClip.SPIDER_ATTACK];
        _audioSource_Move.clip = SoundManager.Instance._audioClip_SFX[(int)SFXClip.SPIDER_MOVE];

        _onAttack = false;
    }

    public override void F_EnemyAttack()
    {
        if (Vector3.Distance(_trackingTarget.position, transform.position) > _attackRange)
            F_ChangeState(EnemyState.TRACKING);
    }

    public override void F_EnemyIdle()
    {
        // 현재 공격 애니메이션이 진행중일떄
        if (_onAttack)
            return;

        if (F_FindPlayer())
        {
            _navAgent.speed = 6f;
            return;
        }

        _currentTime_Idle += Time.deltaTime;
        if(_limitTime_Idle <=  _currentTime_Idle)
        {
            _navAgent.speed = 2.5f;
            F_ChangeState(EnemyState.PROWL);

            _currentTime_Idle = 0f;
        }
    }

    public override void F_EnemyProwl()
    {
        // 현재 공격 애니메이션이 진행중일떄
        if (_onAttack)
            return;

        if (F_FindPlayer())
        {
            // Tracking 상태로 바뀔때 이동속도 증가
            _navAgent.speed = 6f;
            return;           
        }

        // 이동중
        if (_onMove)
        {
            // 1. 남은 거리 계산
            if (Vector3.Distance(_nextPosition, transform.position) <= 5f)
            {
                // 2. 상태 변환
                F_ChangeState(EnemyState.IDLE);

                // 3. 이동중 X 상태
                _onMove = false;
            }
        }
        // 이동중 X
        else
        {
            // 1. 랜덤 위치 생성
            _nextPosition = F_GetRandomPositionOnNavMesh();

            // 2. 랜덤 위치 이동
            _navAgent.SetDestination(_nextPosition);

            // 3. 이동중 상태
            _onMove = true;
        }
    }

    public override void F_EnemyTracking()
    {
        // 현재 공격 애니메이션이 진행중일떄
        if (_onAttack)
            return;

        F_MoveSound();
        if (F_FindPlayer())
        {
            _navAgent.SetDestination(_trackingTarget.position);
            if (Vector3.Distance(_trackingTarget.position, transform.position) <= _attackRange && !PlayerManager.Instance.PlayerController._isPlayerDead)
                F_ChangeState(EnemyState.ATTACK);
        }
        else
        {
            _navAgent.SetDestination(transform.position);
            F_ChangeState(EnemyState.IDLE);
        }
    }

    #region 애니메이션에서 호출
    public void F_Attack()
    {
        Collider[] cols = Physics.OverlapBox(_hitboxPosition.position, _hitboxSize, transform.rotation, _trackingLayerMask);

        foreach(Collider col in cols)
        {
            if (!PlayerManager.Instance.PlayerController._isPlayerDead)
            {
                float _deathPercent = Random.Range(0, 100f);
                //몬스터 피격 기절
                if (_deathPercent <= 30f)
                {
                    PlayerManager.Instance.F_PlayerKnockDown();
                    StartCoroutine(UIManager.Instance.F_KnockDownUI(true));
                }
                //몬스터 피격, 기절 X
                else
                {
                    PlayerManager.Instance.PlayerController.F_DamagedMotion();
                }
                //피격 UI ON
                StartCoroutine(UIManager.Instance.F_DamagedUI());
            }
        }
    }
    public void F_OnAttackState()
    {
        _onAttack = true;
    }
    public void F_OffAttackState()
    {
        _onAttack = false;
    }

    public void F_AttackSound()
    {
        if (_audioSource_Attack.isPlaying)
            return;

        _audioSource_Attack.volume = SoundManager.Instance.F_GetVolume(VolumeType.SFX);
        _audioSource_Attack.Play();
    }
    #endregion

    private void F_MoveSound()
    {
        if (_audioSource_Move.isPlaying)
            return;

        _audioSource_Move.volume = SoundManager.Instance.F_GetVolume(VolumeType.SFX);
        _audioSource_Move.Play();
    }
}
