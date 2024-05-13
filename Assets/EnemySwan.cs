using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySwan : Enemy
{
    [Header("NavMesh")]
    [SerializeField] private NavMeshAgent _navAgent;
    [SerializeField] private Transform _trackingTarget;

    [Header("설정해줘!")]
    [SerializeField] private LayerMask _trackingLayerMask;  // 탐색 레이어
    [Range(1f, 30f)]
    [SerializeField] private float _searchTargetRange;      // 탐색 범위

    protected override void F_EnemyInit()
    {
        _enemyType = EnemyType.ANIMAL;

        _navAgent = GetComponent<NavMeshAgent>();

        _searchTargetRange = 10f;
    }

    public override void F_EnemyAttack()
    {
        if (_enemyType == EnemyType.MONSTER)
        {

        }
    }

    public override void F_EnemyIdle()
    {
        F_FindPlayer();     // 플레이어 탐색
    }

    public override void F_EnemyProwl()
    {
        F_FindPlayer();     // 플레이어 탐색
    }

    public override void F_EnemyTracking()
    {
        if (_trackingTarget != null)
        {
            _navAgent.SetDestination(_trackingTarget.position); // 플레이어 위치로 이동
            F_FindPlayer();                                     // 플레이어 탐색
        }
    }

    private void F_FindPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _searchTargetRange, _trackingLayerMask);  // 범위 확인
        foreach (Collider c in colliders)
        {
            _trackingTarget = c.transform;              // 감지된 오브젝트를 타겟으로 설정
            F_ChangeState(EnemyState.TRACKING);         // 몬스터의 상태를 Tracking 변경
            return;
        }
        _trackingTarget = null;                         // 오브젝트가 감지되지않았을때 타겟을 null
        F_ChangeState(EnemyState.IDLE);                 // 몬스터의 상태를 Idle 변경
    }
}
