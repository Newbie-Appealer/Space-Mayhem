using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySwan : Enemy
{
    [Header("NavMesh")]
    [SerializeField] private NavMeshAgent _navAgent;
    [SerializeField] private Transform _trackingTarget;

    [Header("��������!")]
    [SerializeField] private LayerMask _trackingLayerMask;  // Ž�� ���̾�
    [Range(1f, 30f)]
    [SerializeField] private float _searchTargetRange;      // Ž�� ����

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
        F_FindPlayer();     // �÷��̾� Ž��
    }

    public override void F_EnemyProwl()
    {
        F_FindPlayer();     // �÷��̾� Ž��
    }

    public override void F_EnemyTracking()
    {
        if (_trackingTarget != null)
        {
            _navAgent.SetDestination(_trackingTarget.position); // �÷��̾� ��ġ�� �̵�
            F_FindPlayer();                                     // �÷��̾� Ž��
        }
    }

    private void F_FindPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _searchTargetRange, _trackingLayerMask);  // ���� Ȯ��
        foreach (Collider c in colliders)
        {
            _trackingTarget = c.transform;              // ������ ������Ʈ�� Ÿ������ ����
            F_ChangeState(EnemyState.TRACKING);         // ������ ���¸� Tracking ����
            return;
        }
        _trackingTarget = null;                         // ������Ʈ�� ���������ʾ����� Ÿ���� null
        F_ChangeState(EnemyState.IDLE);                 // ������ ���¸� Idle ����
    }
}
