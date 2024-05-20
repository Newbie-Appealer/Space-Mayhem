using UnityEngine;

public class EnemyTurtle : Enemy
{
    private float _currentTime_Idle = 0f;
    private float _limitTime_Idle = 3f;

    protected override void F_EnemyInit()
    {
        _enemyType = EnemyType.ANIMAL;

        _randomTargetRange = 25f;
        
        _onMove = false;
    }

    public override void F_EnemyIdle()
    {
        _currentTime_Idle += Time.deltaTime;
        if (_limitTime_Idle <= _currentTime_Idle)
        {
            F_ChangeState(EnemyState.PROWL);
            _currentTime_Idle = 0f;
        }
    }

    public override void F_EnemyProwl()
    {
        // 이동중
        if(_onMove)
        {
            // 1. 남은 거리 계산
            if(Vector3.Distance(_nextPosition, transform.position) <= 1.5f)
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


    public override void F_EnemyAttack()
    {
        // 거북이는 공격안해!
    }

    public override void F_EnemyTracking()
    {
        // 거북이는 추적안해!
    }

}
