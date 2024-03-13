using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScrapManager : Singleton<ScrapManager>
{
    [Header("Player Transform")]
    private Transform _player_Transform;                           // player Transform
    public Transform playerTransform
    { get { return _player_Transform; } }

    [Header("Scrap Pooling")]
    [SerializeField] private Transform _scrapGroup;                                                  // 생성한 오브젝트 담아두는 변수
    [SerializeField] private GameObject[] _scrap_Prefabs;                           // 생성할 오브젝트 프리팹 
    private List<Queue<Scrap>> _pooling_Item;                                       // 풀링에 사용할 큐            
    private List<Vector3> _pooling_SpawnPoint;                                      // 오브젝트 랜덤 생성 위치

    [Header("ScrapManager Information")]
    [Range(10f, 25f)]
    public float _item_MoveSpeed = 10f;
    [Range(150f, 300f)] 
    public float _range_Distance = 150f;                                            //아이템과의 거리
    public Vector3 _scrapVelocity;

    private int _item_Count = 30;
    private int _spawnPointCount = 1000;

    protected override void InitManager()
    {
        _scrapVelocity = new Vector3(_item_MoveSpeed, 0, 0);

        _pooling_Item = new List<Queue<Scrap>>                                      // Queue 초기화
        {
            new Queue<Scrap>(),        // 0_플라스틱
            new Queue<Scrap>(),        // 1_섬유
            new Queue<Scrap>(),        // 2_고철
            new Queue<Scrap>()         // 3_박스
        };

        _pooling_SpawnPoint = new List<Vector3>();                                  // 생성 위치 초기화

        _player_Transform = PlayerManager.Instance.playerTransform;                 // 플레이어 Transform

        for(int index = 0; index < _pooling_Item.Count; index++)
        {
            for(int cnt = 0; cnt < _item_Count; cnt++)
            {
                F_CreateScrap(index);
            }
        }
        F_CreateSpawnPoint();

        StartCoroutine(C_ItemSpawn());
    }

    #region 풀링 초기화
    /// <summary> _spawnPointCount 만큼의 랜덤한 스폰 포인트 생성 </summary>
    void F_CreateSpawnPoint()           
    {
        for(int i = 0; i < _spawnPointCount; i++)
        {
            float _spawnX = -100f;
            float _spawnY = Random.Range(-5f, 5f);
            float _spawnZ = Random.Range(-10f, 10f); 

            _pooling_SpawnPoint.Add(new Vector3(_spawnX, _spawnY, _spawnZ));
        }
    }
    /// <summary> v_prefabIndex에 해당하는 오브젝트를 생성 및 풀링에 추가 </summary>
    void F_CreateScrap(int v_prefabIndex)
    {
        Scrap scrap = Instantiate(_scrap_Prefabs[v_prefabIndex], _scrapGroup).GetComponent<Scrap>();

        scrap.F_SettingScrap();
        scrap.F_InitScrap();

        _pooling_Item[v_prefabIndex].Enqueue(scrap);
    }
    #endregion

    void F_SpawnScrap(int v_index)
    {
        // 풀에 남아있는 오브젝트가 없을때 1개 생성
        if (_pooling_Item[v_index].Count == 0) 
            F_CreateScrap(v_index);

        // 꺼내오기
        Scrap scrap = _pooling_Item[v_index].Dequeue();

        // 랜덤 스폰포인트 지정 및 움직임 시작
        Vector3 randomSpawnPoint = _pooling_SpawnPoint[Random.Range(0, _pooling_SpawnPoint.Count)];
        scrap.F_MoveScrap(randomSpawnPoint);

    }

    public void F_ReturnScrap(Scrap scrap)
    {
        _pooling_Item[scrap.scrapNumber].Enqueue(scrap);
        scrap.F_InitScrap();
    }

    private IEnumerator C_ItemSpawn()
    {
        yield return new WaitForSeconds(3f);
        while (true)
        {
            int randomIndex = Random.Range(0,_pooling_Item.Count);
            F_SpawnScrap(randomIndex);

            // TODO:생성 딜레이 랜덤으로 수정하기
            yield return new WaitForSeconds(1f);
        }
    }
}
