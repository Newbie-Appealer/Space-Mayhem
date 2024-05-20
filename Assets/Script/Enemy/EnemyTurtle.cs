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
        // �̵���
        if(_onMove)
        {
            // 1. ���� �Ÿ� ���
            if(Vector3.Distance(_nextPosition, transform.position) <= 1.5f)
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


    public override void F_EnemyAttack()
    {
        // �ź��̴� ���ݾ���!
    }

    public override void F_EnemyTracking()
    {
        // �ź��̴� ��������!
    }

}
