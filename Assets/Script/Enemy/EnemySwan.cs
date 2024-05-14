using UnityEngine;

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

        _searchTargetRange = 10f;
        _randomTargetRange = 10f;
    }

    public override void F_EnemyAttack()
    {
        // ������ ���ݾ���!
    }

    public override void F_EnemyIdle()
    {
        // �÷��̾� Ž��
        if (F_FindPlayer())
            return;             // Ž�� ������ �÷��̾ �����ϸ� return;

        // �÷��̾ ��������������
        _currentTime_Idle += Time.deltaTime;
        if(_limitTime_Idle <= _currentTime_Idle)
        {
            if(stateChange)
                F_ChangeState(EnemyState.PROWL);

            _limitTime_Idle = 0f;
        }
        // Ȯ�� Prowl ���·� �ٲ�°� �����ϱ�
    }

    public override void F_EnemyProwl()
    {
        if (F_FindPlayer())     // �÷��̾� Ž��
            return;             // Ž�� ���� �� �÷��̾ �����ϸ� return;

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
    }

    public override void F_EnemyTracking()
    {
        // �÷��̾� Ž��
        if(F_FindPlayer())
        {
            // �÷��̾� ��ġ�� �̵� ( �߰� )
            _navAgent.SetDestination(_trackingTarget.position); 
        }
        else
        {
            // �߰� ����
            _navAgent.SetDestination(transform.position);     
            // ���� ��ȯ Tracking -> IDLE
            F_ChangeState(EnemyState.IDLE);                   
        }
    }
}
