using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.CullingGroup;

public class EnemySwan : Enemy
{
    private float _currentTime_Idle = 0f;
    private float _limitTime_Idle = 3f;
    private float _currentTime_Prowl = 0f;
    private float _limitTime_Prowl = 3f;

    private bool stateChange => Random.Range(0, 100) % 2 == 0;
    protected override void F_EnemyInit()
    {
        _enemyType = EnemyType.ANIMAL;

        _navAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        _searchTargetRange = 10f;
        _randomTargetRange = 10f;
    }

    public override void F_EnemyAttack()
    {
        // 오리는 공격안해!
    }

    public override void F_EnemyIdle()
    {
        // 플레이어 탐색
        if (F_FindPlayer())
            return;             // 탐색 범위내 플레이어가 존재하면 return;

        // 플레이어가 존재하지않으면
        _currentTime_Idle += Time.deltaTime;
        if(_limitTime_Idle <= _currentTime_Idle)
        {
            if(stateChange)
                F_ChangeState(EnemyState.PROWL);

            _limitTime_Idle = 0f;
        }
        // 확률 Prowl 상태로 바뀌는거 구현하기
    }

    public override void F_EnemyProwl()
    {
        if (F_FindPlayer())     // 플레이어 탐색
            return;             // 탐색 범위 내 플레이어가 존재하면 return;

        _currentTime_Prowl += Time.deltaTime;
        if (_limitTime_Prowl <= _currentTime_Prowl)
        {
            if(stateChange)
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
        // Prowl 동작 구현하기
        // 확률 Idle 상태로 바뀌는거 구현하기
    }

    Vector3 GetRandomPositionOnNavMesh()
    {
        // 범위 내 랜덤한 방향 벡터를 생성
        Vector3 randomDirection = Random.insideUnitSphere * _randomTargetRange;

        // 랜덤 방향 벡터 += 현재 위치
        randomDirection += transform.position; 

        NavMeshHit hit;
        // 랜덤 위치가 NavMesh 위에 있는지 확인
        if (NavMesh.SamplePosition(randomDirection, out hit, _randomTargetRange, NavMesh.AllAreas))
            return hit.position; // NavMesh 위의 랜덤 위치를 반환
        else
            return transform.position; // NavMesh 위의 랜덤 위치를 찾지 못한 경우 현재 위치를 반환
    }

    public override void F_EnemyTracking()
    {
        // 플레이어 탐색
        if(F_FindPlayer())
        {
            // 플레이어 위치로 이동 ( 추격 )
            _navAgent.SetDestination(_trackingTarget.position); 
        }
        else
        {
            // 추격 중지
            _navAgent.SetDestination(transform.position);     
            // 상태 변환 Tracking -> IDLE
            F_ChangeState(EnemyState.IDLE);                   
        }
    }

    private bool F_FindPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _searchTargetRange, _trackingLayerMask);  // 범위 확인
        foreach (Collider c in colliders)
        {
            _trackingTarget = c.transform;              // 감지된 오브젝트를 타겟으로 설정
            F_ChangeState(EnemyState.TRACKING);         // 몬스터의 상태를 Tracking 변경
            return true;
        }
        _trackingTarget = null;                         // 오브젝트가 감지되지않았을때 타겟을 null
        return false;
    }
}
