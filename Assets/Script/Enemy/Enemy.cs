using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    IDLE,
    PROWL,
    TRACKING,
    ATTACK
}

public enum EnemyType
{
    ANIMAL,     // 우호적
    MONSTER     // 적대적
                // 중립 몬스터는 우호 - 적대를 왔다갔다..
}

public abstract class Enemy : MonoBehaviour
{
    // information
    [Header("Enemy Defalut Information")]
    [SerializeField] protected EnemyState _enemyState;           // 몬스터의 현재 상태
    [SerializeField] protected EnemyType _enemyType;             // 몬스터 분류 ( 적대 or 우호 )

    // FSM
    protected EnemyFSM _currentStateFSM;        // 몬스터의 현재 상태 FSM
    protected EnemyFSM[] _enemyFSMs;            // 몬스터 상태 배열

    // Components
    protected NavMeshAgent _navAgent;
    protected Animator _animator;

    // Other
    protected Transform _trackingTarget;

    // setup unity
    [Header("설정해줘!")]
    [SerializeField] protected LayerMask _trackingLayerMask;  // 탐색 레이어
    [Range(1f, 30f)]
    [SerializeField] protected float _searchTargetRange;      // 탐색 범위
    [Range(1f, 30f)]
    [SerializeField] protected float _randomTargetRange;      // 탐색 범위

    // getter
    public Animator animator => _animator;
    private void Start()
    {
        _enemyFSMs = new EnemyFSM[Enum.GetValues(typeof(EnemyState)).Length];

        _enemyFSMs[(int)EnemyState.IDLE]        = new Idle(this);
        _enemyFSMs[(int)EnemyState.PROWL]       = new Prowl(this);
        _enemyFSMs[(int)EnemyState.TRACKING]    = new Tracking(this);
        _enemyFSMs[(int)EnemyState.ATTACK]      = new Attack(this);

        F_EnemyInit();                      // 몬스터 초기화
        F_ChangeState(EnemyState.IDLE);     // 상태   초기화
    }
    private void Update()
    {
        _currentStateFSM.Excute(); // 1번
    }
    protected abstract void F_EnemyInit();
    public abstract void F_EnemyIdle();
    public abstract void F_EnemyProwl();
    public abstract void F_EnemyTracking();
    public abstract void F_EnemyAttack();

    protected void F_ChangeState(EnemyState v_state)
    {
        // 1. 기존 상태 퇴장시 
        _enemyFSMs[(int)_enemyState].Exit();

        // 2. 상태 변경
        _enemyState = v_state;

        // 3. 새로운 상태 진입시
        _enemyFSMs[(int)v_state].Enter();

        // 4. 현재 상태 FSM 초기화
        _currentStateFSM = _enemyFSMs[(int)_enemyState];
    }
}
