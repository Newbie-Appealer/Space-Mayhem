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

public class Enemy : MonoBehaviour
{
    protected EnemyState _enemyState;           // ������ ���� ����
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
    }

    private void Update()
    {
        _currentStateFSM.Excute(); // 1��

        if(Input.GetKeyDown(KeyCode.L))
        {
            F_ChangeState(EnemyState.ATTACK);
        }
        else if(Input.GetKeyDown(KeyCode.K))
        {
            F_ChangeState(EnemyState.TRACKING);
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            F_ChangeState(EnemyState.PROWL);
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            F_ChangeState(EnemyState.IDLE);
        }
    }

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
