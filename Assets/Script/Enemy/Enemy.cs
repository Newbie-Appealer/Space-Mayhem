using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [Header("Enemy Defalut Information")]
    [SerializeField] protected EnemyState _enemyState;           // ������ ���� ����
    [SerializeField] protected EnemyType _enemyType;             // ���� �з� ( ���� or ��ȣ )

    protected EnemyFSM _currentStateFSM;        // ������ ���� ���� FSM
    protected EnemyFSM[] _enemyFSMs;            // ���� ���� �迭

    private void Start()
    {
        _enemyFSMs = new EnemyFSM[Enum.GetValues(typeof(EnemyState)).Length];

        _enemyFSMs[(int)EnemyState.IDLE]        = new Idle(this);
        _enemyFSMs[(int)EnemyState.PROWL]       = new Prowl(this);
        _enemyFSMs[(int)EnemyState.TRACKING]    = new Tracking(this);
        _enemyFSMs[(int)EnemyState.ATTACK]      = new Attack(this);

        _enemyState = EnemyState.IDLE;                      // ���� �ʱ�ȭ
        _currentStateFSM = _enemyFSMs[(int)_enemyState];    // ���� FSM �ʱ�ȭ

        F_EnemyInit();
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
        // 1. ���� ���� ����
        _enemyFSMs[(int)_enemyState].Exit();            

        // 2. ���� ��ȯ
        _enemyState = v_state;                          

        // 3. ���ο� ���� ����
        _enemyFSMs[(int)_enemyState].Enter();

        // 4. ���� ���� FSM �ʱ�ȭ
        _currentStateFSM = _enemyFSMs[(int)_enemyState];
    }
}
