using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum EnemyState
{
    IDLE,
    PROWL,
    TRACKING,
    ATTACK
}

public enum EnemyType
{
    ANIMAL,     // ��ȣ��
    MONSTER     // ������
                // �߸� ���ʹ� ��ȣ - ���븦 �Դٰ���..
}

public abstract class Enemy : MonoBehaviour
{
    // information
    [Header("Enemy Defalut Information")]
    [SerializeField] protected EnemyState _enemyState;           // ������ ���� ����
    [SerializeField] protected EnemyType _enemyType;             // ���� �з� ( ���� or ��ȣ )

    // FSM
    protected EnemyFSM _currentStateFSM;        // ������ ���� ���� FSM
    protected EnemyFSM[] _enemyFSMs;            // ���� ���� �迭

    // Components
    protected NavMeshAgent _navAgent;
    protected Animator _animator;

    // Other
    protected Transform _trackingTarget;

    // setup unity
    [Header("��������!")]
    [SerializeField] protected LayerMask _trackingLayerMask;  // Ž�� ���̾�
    [Range(1f, 30f)]
    [SerializeField] protected float _searchTargetRange;      // Ž�� ����
    [Range(1f, 30f)]
    [SerializeField] protected float _randomTargetRange;      // Ž�� ����

    // getter
    public Animator animator => _animator;
    private void Start()
    {
        _enemyFSMs = new EnemyFSM[Enum.GetValues(typeof(EnemyState)).Length];

        _enemyFSMs[(int)EnemyState.IDLE]        = new Idle(this);
        _enemyFSMs[(int)EnemyState.PROWL]       = new Prowl(this);
        _enemyFSMs[(int)EnemyState.TRACKING]    = new Tracking(this);
        _enemyFSMs[(int)EnemyState.ATTACK]      = new Attack(this);

        _navAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        F_EnemyInit();                      // ���� �ʱ�ȭ
        F_ChangeState(EnemyState.IDLE);     // ����   �ʱ�ȭ
    }
    private void Update()
    {
        _currentStateFSM.Excute(); // 1��
    }
    protected abstract void F_EnemyInit();
    public abstract void F_EnemyIdle();
    public abstract void F_EnemyProwl();
    public abstract void F_EnemyTracking();
    public abstract void F_EnemyAttack();

    protected void F_ChangeState(EnemyState v_state)
    {
        // 1. ���� ���� ����� 
        _enemyFSMs[(int)_enemyState].Exit();

        // 2. ���� ����
        _enemyState = v_state;

        // 3. ���ο� ���� ���Խ�
        _enemyFSMs[(int)v_state].Enter();

        // 4. ���� ���� FSM �ʱ�ȭ
        _currentStateFSM = _enemyFSMs[(int)_enemyState];
    }

    /// <summary> �÷��̾ Ž���ϴ� �Լ� </summary>
    protected bool F_FindPlayer()
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

    /// <summary> Prowl ������ ���� ��ġ�� ���ϴ� �Լ�</summary>
    protected Vector3 GetRandomPositionOnNavMesh()
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
}
