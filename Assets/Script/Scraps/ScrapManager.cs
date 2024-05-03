using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class ScrapManager : Singleton<ScrapManager>
{

    [Header("Player Transform")]
    private Transform _player_Transform;                           // player Transform
    public Transform playerTransform
    { get { return _player_Transform; } }

    [Header("Scrap Pooling")]
    [SerializeField] private Transform _scrapGroup;                                 // 생성한 오브젝트 담아두는 변수
    [SerializeField] private GameObject[] _scrap_Prefabs;                           // 생성할 오브젝트 프리팹 
    private List<Queue<Scrap>> _pooling_Item;                                       // 풀링에 사용할 큐            
    private List<Vector3> _scrap_StartMovePosition;                                      // 오브젝트 최초 생성 방향(총 8방향) 위치
    public List<Vector3> scrapStartPosition => _scrap_StartMovePosition;
    // 오브젝트 랜덤 생성 위치
    // 0 ~ 99 : 1번 방향, 100 ~ 199 : 2번 방향, 200 ~ 299 : 3번 방향 ~
    private Dictionary<int, Vector3[]> _pooling_SpawnPoint;
    private int _randomItemSpawnIdx;
    private int _pooling_Key = 0;

    [Header("ScrapManager Information")]
    [Range(0f, 25f)]
    public float _item_MoveSpeed = 2f;
    [Range(150f, 300f)]
    public float _range_Distance = 150f;                                            //아이템과의 거리
    private float _spawn_Distance = 100f;                                            //아이템과의 거리
    public Vector3 _scrapToMoveVelocity; //최초 플레이어 주변 구 범위 내의 좌표
    public Vector3 _scrapVelocity; //아이템 움직임 벡터
    public List<Scrap> _scrapHitedSpear;

    private int _item_Count = 1;
    private int _spawnPointCount = 100;

    protected override void InitManager()
    {
        _randomItemSpawnIdx = Random.Range(0, 8);

        //플레이어 주변 구 범위 내에 랜덤 좌표 선정
        _scrapToMoveVelocity = Random.insideUnitSphere * MeteorManager.Instance.player_SphereCollider.radius;
        _scrapVelocity = new Vector3(_item_MoveSpeed, 0, 0);
        _scrapHitedSpear = new List<Scrap>();

        _pooling_Item = new List<Queue<Scrap>>                                      // Queue 초기화
        {
            new Queue<Scrap>(),        // 0_플라스틱
            new Queue<Scrap>(),        // 1_섬유
            new Queue<Scrap>(),        // 2_고철
            new Queue<Scrap>()         // 3_박스
        };

        _pooling_SpawnPoint = new Dictionary<int, Vector3[]>();                                  // 생성 위치 초기화
        _scrap_StartMovePosition = new List<Vector3>();                         //최초 Scrap 생성 위치
        _player_Transform = PlayerManager.Instance.playerTransform;                 // 플레이어 Transform

        for (int index = 0; index < _pooling_Item.Count; index++)
        {
            for (int cnt = 0; cnt < _item_Count; cnt++)
            {
                F_CreateScrap(index);
            }
        }
        for (int l = 0; l < 8; l++)
        {
            F_CreateStartMovePosition(l);
        }
        for (int l = 0; l < 8; l++)
        {
            F_CreateSpawnPoint(_scrap_StartMovePosition, l);
        }
        StartCoroutine("C_ItemSpawn", _randomItemSpawnIdx);
        //우선 게임 시작 후 10초 후 방향 변경, Coroutine 시작으로 언제든 방향 변경 설정 가능
        StartCoroutine(C_ScrapMoveChange());
    }

    #region 풀링 초기화

    private void F_CreateStartMovePosition(int v_index)
    {
        float _spawnX = _scrapToMoveVelocity.x;
        float _spawnY = _scrapToMoveVelocity.y;
        float _spawnZ = _scrapToMoveVelocity.z;
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
    /// <summary> 8방향 중 랜덤 방향 포인트, 그 안에서 랜덤한 스폰 포인트 생성 </summary>
    void F_CreateSpawnPoint(List<Vector3> v_startPosition, int v_index)
    {
        Vector3 _startVector3 = v_startPosition[v_index];
        _pooling_SpawnPoint[_pooling_Key] = new Vector3[_spawnPointCount];
        for (int i = 0; i < _spawnPointCount; i++)
        {
            _pooling_SpawnPoint[_pooling_Key][i] = new Vector3(_startVector3.x - Random.Range(-30f, 30f), _startVector3.y - Random.Range(-10f, 10f), _startVector3.z - Random.Range(-30f, 30f));
        }
        _pooling_Key++;
    }
    /// <summary> v_prefabIndex에 해당하는 오브젝트를 생성 및 풀링에 추가 </summary>
    void F_CreateScrap(int v_prefabIndex)
    {
        Scrap scrap = Instantiate(_scrap_Prefabs[v_prefabIndex], _scrapGroup).GetComponent<Scrap>();

        scrap.F_SettingScrap();
        scrap.F_InitScrap(_scrapGroup);

        _pooling_Item[v_prefabIndex].Enqueue(scrap);
    }
    #endregion

    void F_SpawnScrap(int v_index, int v_startPositionIdx)
    {
        //풀에 남아있는 오브젝트가 없을때 1개 생성
        if (_pooling_Item[v_index].Count == 0)
            F_CreateScrap(v_index);
        
        // 꺼내오기
        Scrap scrap = _pooling_Item[v_index].Dequeue();

        // 랜덤 스폰포인트 지정 및 움직임 시작
        Vector3 randomSpawnPoint = _pooling_SpawnPoint[v_startPositionIdx][Random.Range(0, _spawnPointCount)];
        _scrapVelocity = new Vector3(-(randomSpawnPoint.x - _scrapToMoveVelocity.x - Random.Range(-10f, 10f)), 0, -(randomSpawnPoint.z - _scrapToMoveVelocity.z - Random.Range(-5f, 5f))).normalized * _item_MoveSpeed;
        scrap.F_MoveScrap(randomSpawnPoint);
    }

    public void F_ReturnScrap(Scrap scrap)
    {
        _pooling_Item[scrap.scrapNumber].Enqueue(scrap);
        scrap.F_InitScrap(_scrapGroup);
    }

    private IEnumerator C_ItemSpawn(int v_index)
    {
        yield return new WaitForSeconds(3f);
        while (true)
        {
            int randomIndex = Random.Range(0,_pooling_Item.Count);
            F_SpawnScrap(randomIndex, v_index);

            // TODO:생성 딜레이 랜덤으로 수정하기
            int _randomDelay = Random.Range(1, 4);
            yield return new WaitForSeconds(_randomDelay);
        }
    }

    private IEnumerator C_ScrapMoveChange()
    {
        yield return new WaitForSeconds(10f);
        while(true)
        {
            StopCoroutine("C_ItemSpawn");
            int _nextRandomIdx = Random.Range(0, 8);
            
            //반대 방향
            if(_nextRandomIdx - _randomItemSpawnIdx == -4 || _nextRandomIdx - _randomItemSpawnIdx == 4)
            {
                F_ChangeScrapVelocity(_nextRandomIdx, 5f);
            }
            //다음 좌표와의 각이 둔각일 때
            else if (_nextRandomIdx - _randomItemSpawnIdx == -3 ||  _nextRandomIdx - _randomItemSpawnIdx == -5 || _nextRandomIdx - _randomItemSpawnIdx == 3 || _nextRandomIdx - _randomItemSpawnIdx == 5)
            {
                F_ChangeScrapVelocity(_nextRandomIdx, 10f);
            }
            else 
                F_ChangeScrapVelocity(_nextRandomIdx, 15f);

            if(_nextRandomIdx == _randomItemSpawnIdx)
                Debug.Log("같은 방향, 30초 후 다시 방향 설정");
            else 
                Debug.Log("다음 방향 : " + _nextRandomIdx);
            StartCoroutine("C_ItemSpawn", _nextRandomIdx);
            _randomItemSpawnIdx = _nextRandomIdx;
            yield return new WaitForSeconds(30f);
        }
    }

    public void F_ChangeScrapVelocity(int v_spawnIdx, float v_changeSpeed)
    {
        int count = 0;
        Vector3 _newVelocity = _scrap_StartMovePosition[v_spawnIdx];
        Vector3 _targetVelocity = _scrapToMoveVelocity;
        _scrapVelocity = new Vector3(-(_newVelocity.x - _targetVelocity.x), 0, -(_newVelocity.z - _targetVelocity.z)).normalized * _item_MoveSpeed;
        for(int l = 0 ; l < _scrapGroup.transform.childCount; l++)
        {
            if (_scrapGroup.transform.GetChild(l).gameObject.activeSelf)
            {
                StartCoroutine(_scrapGroup.transform.GetChild(l).gameObject.GetComponent<Scrap>().C_ItemVelocityChange(_scrapVelocity, v_changeSpeed));
                count++;
            }
            else
                continue;
        }
        Debug.Log("이동 변경 오브젝트 수 : " + count + ", 변경 속도 : " + v_changeSpeed);

    }
}


