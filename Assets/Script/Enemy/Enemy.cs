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
    protected EnemyState _enemyState;           // 몬스터의 현재 상태
    protected EnemyFSM _currentStateFSM;        // 몬스터의 현재 상태 FSM

    protected EnemyFSM[] _enemyFSMs;            // 몬스터 상태 배열

    private void Start()
    {
        _enemyFSMs = new EnemyFSM[Enum.GetValues(typeof(EnemyState)).Length];

        _enemyFSMs[(int)EnemyState.IDLE]        = new Idle(this);
        _enemyFSMs[(int)EnemyState.PROWL]       = new Prowl(this);
        _enemyFSMs[(int)EnemyState.TRACKING]    = new Tracking(this);
        _enemyFSMs[(int)EnemyState.ATTACK]      = new Attack(this);

        _enemyState = EnemyState.IDLE;                      // 상태 초기화
        _currentStateFSM = _enemyFSMs[(int)_enemyState];    // 상태 FSM 초기화
    }

    private void Update()
    {
        _currentStateFSM.Excute(); // 1번

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
        // 1. 기존 상태 퇴장
        _enemyFSMs[(int)_enemyState].Exit();            

        // 2. 상태 변환
        _enemyState = v_state;                          

        // 3. 새로운 상태 진입
        _enemyFSMs[(int)_enemyState].Enter();

        // 4. 현재 상태 FSM 초기화
        _currentStateFSM = _enemyFSMs[(int)_enemyState];
    }
}
