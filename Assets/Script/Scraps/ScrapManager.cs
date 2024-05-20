using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class ScrapManager : Singleton<ScrapManager>
{

    [Header("Player Transform")]
    private Transform _player_Transform;                           
    public Transform playerTransform
    { get { return _player_Transform; } }

    [Header("=== ABOUT Pooling ===")]
    private List<Vector3> _scrap_StartMovePosition;                                      // 오브젝트 최초 생성 방향(총 8방향) 위치
    public List<Vector3> scrapStartPosition => _scrap_StartMovePosition;
    [SerializeField] private Transform _scrapGroup;                                     // 생성한 오브젝트 담아두는 변수
    [SerializeField] private GameObject[] _scrap_Prefabs;                           // 생성할 오브젝트 프리팹 
    private int _scrap_Count = 2;                                                                   // Scrap 종류별로 개수
    private List<Queue<Scrap>> _pooling_Item;                                        // 풀링에 사용할 큐            
    private List<Vector3> _pooling_SpawnPoint;
    private int _spawnPointCount = 100;

    // 오브젝트 랜덤 생성 위치

    [Header("ScrapManager Information")]
    [Range(0f, 25f)]
    public float _item_MoveSpeed = 2f;
    [Range(300f, 500f)]
    public float _range_Distance = 300f;                                            //아이템과의 거리
    private float _spawn_Distance = 150f;                                            //최초 스폰 거리
    public List<Scrap> _scrapHitedSpear;


    [Header("=== ABOUT SCRAP MOVE ===")] 
    [SerializeField] private Vector3 _scrapVelocity;               // 아이템 움직임 벡터
    private Vector3 _currentSpawnPoint;
    private List<Vector3> _scrapVelocity_List;
    private int _randomItemSpawnIdx;      //랜덤하게 뽑을 Spawn 위치 인덱스


    protected override void InitManager()
    {
        _player_Transform = PlayerManager.Instance.playerTransform;                 // 플레이어 Transform

        _randomItemSpawnIdx = Random.Range(0, 8);

        //플레이어 주변 구 범위 내에 랜덤 좌표 선정
        _scrapHitedSpear = new List<Scrap>();

        _pooling_Item = new List<Queue<Scrap>>                                      // Queue 초기화
        {
            new Queue<Scrap>(),        // 0_플라스틱
            new Queue<Scrap>(),        // 1_섬유
            new Queue<Scrap>(),        // 2_고철
            new Queue<Scrap>()         // 3_박스
        };

        _pooling_SpawnPoint = new List<Vector3>();                                  // 생성 위치 초기화
        _scrap_StartMovePosition = new List<Vector3>();                         //최초 Scrap 생성 위치
        _scrapVelocity_List = new List<Vector3>();

        for (int index = 0; index < _pooling_Item.Count; index++)
        {
            for (int cnt = 0; cnt < _scrap_Count; cnt++)
            {
                F_CreateScrap(index);
            }
        }

        //최초 8방향 Vector3 생성
        for (int l = 0; l < 8; l++)
        {
            F_CreateStartMovePosition(l);
        }

        F_CreateSpawnPoint();
        _currentSpawnPoint = _scrap_StartMovePosition[_randomItemSpawnIdx];
        _scrapGroup.transform.localPosition = _currentSpawnPoint;

        // 8방향 Vector3 안에서 각각 _spawnPointCount개의 스폰 포인트 생성, 8개의 Velocity 벡터 생성
        for (int l = 0; l < 8; l++)
        {
            _scrapVelocity_List.Add(F_SetScrapVelocity(l, _scrap_StartMovePosition[l]));
        }
        _scrapVelocity = _scrapVelocity_List[_randomItemSpawnIdx];
        StartCoroutine("C_ScrapSpawn");

        //게임 시작 후 30초 후 방향 변경, 그 후 1분마다 방향 변경
        StartCoroutine(C_ScrapMoveChange());
    }

    #region About Pooling

    private void F_CreateStartMovePosition(int v_index)
    {
        float _spawnX = _scrapGroup.position.x;
        float _spawnY = _scrapGroup.position.y;
        float _spawnZ = _scrapGroup.position.z;

        //8방향 중 맨 처음 초기 진행 방향 설정 
        switch (v_index)
        {
            case 0:
                _spawnX += _spawn_Distance;
                break;
            case 1:
                _spawnX += _spawn_Distance;
                _spawnZ += _spawn_Distance;
                break;
            case 2:
                _spawnZ += _spawn_Distance;
                break;
            case 3:
                _spawnZ += _spawn_Distance;
                _spawnX -= _spawn_Distance;
                break;
            case 4:
                _spawnX -= _spawn_Distance;
                break;
            case 5:
                _spawnX -= _spawn_Distance;
                _spawnZ -= _spawn_Distance;
                break;
            case 6:
                _spawnZ -= _spawn_Distance;
                break;
            case 7:
                _spawnX += _spawn_Distance;
                _spawnZ -= _spawn_Distance;
                break;
        }
        _scrap_StartMovePosition.Add(new Vector3(_spawnX, _spawnY, _spawnZ));
    }

    /// <summary> 8방향 중 랜덤 방향 포인트, 그 안에서 _spawnPointCount개 만큼 랜덤한 스폰 포인트 생성 </summary>
    void F_CreateSpawnPoint()
    {
        for (int i = 0; i < _spawnPointCount; i++)
        {
            _pooling_SpawnPoint.Add(new Vector3(Random.Range(-100f, 100f), Random.Range(-10f, 10f), Random.Range(-100f, 100f)));
        }
    }

    /// <summary> v_prefabIndex에 해당하는 오브젝트를 생성 및 풀링에 추가 </summary>
    void F_CreateScrap(int v_prefabIndex)
    {
        Scrap scrap = Instantiate(_scrap_Prefabs[v_prefabIndex], _scrapGroup).GetComponent<Scrap>();

        scrap.F_SettingScrap();
        scrap.F_InitScrap(_scrapGroup);

        _pooling_Item[v_prefabIndex].Enqueue(scrap);
    }

    public void F_ReturnScrap(Scrap scrap)
    {
        _pooling_Item[scrap.scrapNumber].Enqueue(scrap);
        scrap.F_InitScrap(_scrapGroup);
    }
    #endregion

    #region About Scrap
    void F_SpawnScrap(int v_index, Vector3 v_velocity)
    {
        //풀에 남아있는 오브젝트가 없을때 1개 생성
        if (_pooling_Item[v_index].Count == 0)
            F_CreateScrap(v_index);
        
        // 꺼내오기
        Scrap scrap = _pooling_Item[v_index].Dequeue();
        
        // 스폰포인트 지정 및 움직임 시작
        Vector3 randomSpawnPoint = _pooling_SpawnPoint[Random.Range(0, _pooling_SpawnPoint.Count)];
        scrap.F_MoveScrap(randomSpawnPoint, v_velocity);
    }

    private IEnumerator C_ScrapSpawn()
    {
        yield return new WaitForSeconds(3f);

        while (true)
        {
            int randomIndex = Random.Range(0,_pooling_Item.Count);

            F_SpawnScrap(randomIndex, _scrapVelocity);

            float _randomDelay = Random.Range(0.5f, 2f);
            yield return new WaitForSeconds(_randomDelay);
        }
    }
    #endregion

    #region About Scrap Move
    private Vector3 F_SetScrapVelocity(int v_startPositionIdx,  Vector3 v_startSpawnVector)
    {
        switch (v_startPositionIdx)
        {
            case 0:
                {
                    _scrapVelocity = new Vector3(- v_startSpawnVector.x, 0, 0).normalized;
                    break;
                }
            case 1:
                {
                    _scrapVelocity = new Vector3(-v_startSpawnVector.x, 0,  -v_startSpawnVector.z).normalized;
                    break;
                }
            case 2:
                {
                    _scrapVelocity = new Vector3(0, 0, - v_startSpawnVector.z).normalized;
                    break;
                }
            case 3:
                    goto case 1;
            case 4:
                    goto case 1;
            case 5:
                    goto case 1;
            case 6:
                    goto case 2;
            case 7:
                    goto case 1;
        }
        return _scrapVelocity;
    }

    private IEnumerator C_ScrapMoveChange()
    {
        yield return new WaitForSeconds(30f);

        while(true)
        {
            StopCoroutine("C_ScrapSpawn");
            int _nextRandomIdx = Random.Range(-1, 2);
            int _nextIdx = _randomItemSpawnIdx - _nextRandomIdx;

            if (_nextIdx == _randomItemSpawnIdx)
                continue;
            else
            {
                if (_nextIdx == -1)
                    _nextIdx = 7;
                else if (_nextIdx == 8)
                    _nextIdx = 0;

                Debug.Log("다음 방향 : " + _nextIdx);
                F_ChangeScrapVelocity(_nextIdx, 5f);
                _scrapVelocity = _scrapVelocity_List[_nextIdx];
                StartCoroutine("C_ScrapSpawn");
                _randomItemSpawnIdx = _nextIdx;
                yield return new WaitForSeconds(60f);
            }
        }
    }
    

    public void F_ChangeScrapVelocity(int v_nextIdx, float v_changeSpeed)
    {
        Vector3 _newVelocity = _scrapVelocity_List[v_nextIdx] * _item_MoveSpeed;
        for(int l = 0 ; l < _scrapGroup.transform.childCount; l++)
        {
            if (_scrapGroup.transform.GetChild(l).gameObject.activeSelf)
                StartCoroutine(_scrapGroup.transform.GetChild(l).gameObject.GetComponent<Scrap>().C_ItemVelocityChange(_newVelocity, v_changeSpeed));
            else
                continue;
        }
        StartCoroutine(C_SpawnPointMove(_currentSpawnPoint, _scrap_StartMovePosition[v_nextIdx]));
    }

    private IEnumerator C_SpawnPointMove(Vector3 v_currentPoint, Vector3 v_nextPoint)
    {
        float _float = 0f;
        float _duration = 1f;
        while (_float < _duration)
        {
            _currentSpawnPoint = Vector3.Lerp(v_currentPoint, v_nextPoint, _float);
            _scrapGroup.transform.localPosition = _currentSpawnPoint;
            _float += Time.deltaTime / 35f;
            yield return null;
        }
    }
    #endregion
}

