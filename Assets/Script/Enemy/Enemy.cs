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
    protected EnemyFSM      _currentStateFSM;       // 몬스터의 현재 상태 FSM
    protected EnemyFSM[]    _enemyFSMs;             // 몬스터 상태 배열

    // Components
    protected NavMeshAgent  _navAgent;              // 몬스터 NavMeshAgent
    protected Animator      _animator;              // 몬스터 애니메이터

    // Other
    protected Transform     _trackingTarget;        // 추적중인 타겟 Transform
    protected bool          _onMove;                // 현재 움직이는중인지 확인하는 변수 ( Prowl )
    protected Vector3       _nextPosition;          // 현재위치에서 이동해야하는 랜덤 위치값 ( Prowl )

    // setup unity
    [Header("설정해줘!")]
    [SerializeField] protected LayerMask _trackingLayerMask;    // 탐색 레이어
    [Range(1f, 150f)]
    [SerializeField] protected float    _searchTargetRange;     // 타겟 탐색 범위
    [Range(1f, 150f)]
    [SerializeField] protected float    _randomTargetRange;      // 랜덤위치탐색 범위

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

        F_EnemyInit();                      // 몬스터 초기화
        F_ChangeState(EnemyState.IDLE);     // 상태   초기화
    }
    private void Update()
    {
        _currentStateFSM.Excute();
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

    /// <summary> 플레이어를 탐지하는 함수 </summary>
    protected bool F_FindPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _searchTargetRange, _trackingLayerMask);  // 범위 확인
        foreach (Collider c in colliders)
        {
            _trackingTarget = c.transform;              // 감지된 오브젝트를 타겟으로 설정
            F_ChangeState(EnemyState.TRACKING);         // 몬스터의 상태를 Tracking 변경
            return true;
        }
        _trackingTarget = null;                         // 오브젝트가 감지되지않았을때 타겟을 null
        return false;
    }

    /// <summary> Prowl 상태중 랜덤 위치를 구하는 함수</summary>
    protected Vector3 F_GetRandomPositionOnNavMesh()
    {
        // 범위 내 랜덤한 방향 벡터를 생성
        Vector3 randomDirection = Random.insideUnitSphere * _randomTargetRange;

        // 랜덤 방향 벡터 += 현재 위치
        randomDirection += transform.position;

        NavMeshHit hit;
        // 랜덤 위치가 NavMesh 위에 있는지 확인
        if (NavMesh.SamplePosition(randomDirection, out hit, _randomTargetRange, NavMesh.AllAreas))
            return hit.position; // NavMesh 위의 랜덤 위치를 반환
        else
            return transform.position; // NavMesh 위의 랜덤 위치를 찾지 못한 경우 현재 위치를 반환
    }
}
