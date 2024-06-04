using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpider : Enemy
{
    private float _currentTime_Idle = 0f;
    private float _limitTime_Idle = 2f;

    private float _attackRange = 4.3f;

    [Header("Spider �𵨸� ���� �ذ��")]
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
            obj.SetParent(_parentObject);
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
            _navAgent.speed = 2.5f;
            F_ChangeState(EnemyState.PROWL);

            _currentTime_Idle = 0f;
        }
    }

    public override void F_EnemyProwl()
    {
        if (F_FindPlayer())
        {
            // Tracking ���·� �ٲ� �̵��ӵ� ����
            _navAgent.speed = 6f;
            return;           
        }

        // �̵���
        if (_onMove)
        {
            // 1. ���� �Ÿ� ���
            if (Vector3.Distance(_nextPosition, transform.position) <= 5f)
            {
                // 2. ���� ��ȯ
                F_ChangeState(EnemyState.IDLE);

                // 3. �̵��� X ����
                _onMove = false;
            }
        }
        // �̵��� X
        else
        {
            // 1. ���� ��ġ ����
            _nextPosition = F_GetRandomPositionOnNavMesh();

            // 2. ���� ��ġ �̵�
            _navAgent.SetDestination(_nextPosition);

            // 3. �̵��� ����
            _onMove = true;
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
            if (!PlayerManager.Instance.PlayerController._isPlayerDead)
            {
                float _deathPercent = Random.Range(0, 100f);
                Debug.Log(_deathPercent);
                if (_deathPercent <= 30f)
                {
                    UIManager.Instance.F_KnockDownUI(true);
                    PlayerManager.Instance.F_PlayerKnockDown();
                }
            }
        }
    }
}
