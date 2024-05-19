using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpider : Enemy
{
    private float _currentTime_Idle = 0f;
    private float _currentTime_Prowl = 0f;
    private float _limitTime_Idle = 3f;
    private float _limitTime_Prowl = 3f;

    private float _attackRange = 4.3f;
    private bool stateChange => Random.Range(0, 100) % 2 == 0;

    [Header("Spider 모델링 버그 해결용")]
    [SerializeField] private Transform _parentObject;
    [SerializeField] private Transform[] _childObjects;

    [Header(" Hitbox ")]
    [SerializeField] private Transform _hitboxPosition;
    [SerializeField] private Vector3 _hitboxSize;
    protected override void F_EnemyInit()
    {
        _enemyType = EnemyType.MONSTER;

        _searchTargetRange = 15f;
        _randomTargetRange = 10f;

        foreach(Transform obj in _childObjects)
        {
            obj.SetParent(_parentObject);
        }
    }

    public override void F_EnemyAttack()
    {
        if (Vector3.Distance(_trackingTarget.position, transform.position) > _attackRange)
            F_ChangeState(EnemyState.TRACKING);
    }

    public override void F_EnemyIdle()
    {
        if (F_FindPlayer())
        {
            _navAgent.speed = 6f;
            return;
        }

        _currentTime_Idle += Time.deltaTime;
        if(_limitTime_Idle <=  _currentTime_Idle)
        {
            if (stateChange)
            {
                _navAgent.speed = 2.5f;
                F_ChangeState(EnemyState.PROWL);
            }

            _limitTime_Idle = 0f;
        }
    }

    public override void F_EnemyProwl()
    {
        if (F_FindPlayer())
        {
            // Tracking 상태로 바뀔때 이동속도 증가
            _navAgent.speed = 6f;
            return;           
        }

        _currentTime_Prowl += Time.deltaTime;
        if (_limitTime_Prowl <= _currentTime_Prowl)
        {
            if (stateChange)
            {
                _navAgent.SetDestination(transform.position);
                F_ChangeState(EnemyState.IDLE);
            }
            else
            {
                Vector3 nextPosition = GetRandomPositionOnNavMesh();
                _navAgent.SetDestination(nextPosition);
            }
            _currentTime_Prowl = 0f;
        }
    }

    public override void F_EnemyTracking()
    {
        if(F_FindPlayer())
        {
            _navAgent.SetDestination(_trackingTarget.position);
            if (Vector3.Distance(_trackingTarget.position, transform.position) <= _attackRange)
                F_ChangeState(EnemyState.ATTACK);
        }
        else
        {
            _navAgent.SetDestination(transform.position);
            F_ChangeState(EnemyState.IDLE);
        }
    }

    public void F_Attack()
    {
        Collider[] cols = Physics.OverlapBox(_hitboxPosition.position, _hitboxSize, transform.rotation, _trackingLayerMask);

        foreach(Collider col in cols)
        {
            Debug.Log("플레이어! 공격! 맞음! ^A^ : " + col.name);
        }
    }
}
