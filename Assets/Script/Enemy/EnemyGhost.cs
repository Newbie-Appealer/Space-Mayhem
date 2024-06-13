using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGhost : Enemy
{
    protected override void F_EnemyInit()
    {
        _enemyType = EnemyType.MONSTER;

        _searchTargetRange = 120f;
        _randomTargetRange = 120f;

        _onMove = false;
        
        _animator = GetComponentInChildren<Animator>();
    }
    public override void F_EnemyAttack()
    {
        // 이 몬스터한테는 닿이면 사망함!
    }

    public override void F_EnemyIdle()
    {
        F_ChangeState(EnemyState.PROWL);
    }

    public override void F_EnemyProwl()
    {
        if (F_FindPlayer())
            return;

        if(_onMove)
        {
            if(Vector3.Distance(_nextPosition, transform.position) <= 5f)
            {
                F_ChangeState(EnemyState.IDLE);
                _onMove = false;
            }
        }
        else
        {
            _nextPosition = F_GetRandomPositionOnNavMesh();
            _navAgent.SetDestination(_nextPosition);
            _onMove = true;
        }
    }

    public override void F_EnemyTracking()
    {
        if(F_FindPlayer())
        {
            _navAgent.SetDestination(_trackingTarget.position);
        }
        else
        {
            _navAgent.SetDestination(transform.position);
            F_ChangeState(EnemyState.IDLE);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(UIManager.Instance.F_DamagedUI());
            PlayerManager.Instance.F_PlayerKnockDown();
            StartCoroutine(UIManager.Instance.F_KnockDownUI(true));
        }
    }
}
