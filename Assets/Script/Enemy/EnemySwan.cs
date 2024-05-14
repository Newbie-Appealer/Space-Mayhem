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
        // Prowl ���� �����ϱ�
        // Ȯ�� Idle ���·� �ٲ�°� �����ϱ�
    }

    Vector3 GetRandomPositionOnNavMesh()
    {
        // ���� �� ������ ���� ���͸� ����
        Vector3 randomDirection = Random.insideUnitSphere * _randomTargetRange;

        // ���� ���� ���� += ���� ��ġ
        randomDirection += transform.position; 

        NavMeshHit hit;
        // ���� ��ġ�� NavMesh ���� �ִ��� Ȯ��
        if (NavMesh.SamplePosition(randomDirection, out hit, _randomTargetRange, NavMesh.AllAreas))
            return hit.position; // NavMesh ���� ���� ��ġ�� ��ȯ
        else
            return transform.position; // NavMesh ���� ���� ��ġ�� ã�� ���� ��� ���� ��ġ�� ��ȯ
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

    private bool F_FindPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _searchTargetRange, _trackingLayerMask);  // ���� Ȯ��
        foreach (Collider c in colliders)
        {
            _trackingTarget = c.transform;              // ������ ������Ʈ�� Ÿ������ ����
            F_ChangeState(EnemyState.TRACKING);         // ������ ���¸� Tracking ����
            return true;
        }
        _trackingTarget = null;                         // ������Ʈ�� ���������ʾ����� Ÿ���� null
        return false;
    }
}
