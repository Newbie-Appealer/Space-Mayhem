using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScrapManager : Singleton<ScrapManager>
{
    [Header("기본 재료 아이템")]
    private GameObject _scrap_Group;
    [SerializeField] private GameObject[] _items_etc;                                                
    private Queue<GameObject> _pooling_Item;                                      
    private List<Vector3> _pooling_Spawn;

    [Range(10f, 25f)] 
    public float _item_MoveSpeed = 10f;
    private int _item_Count = 20;

    [Header("플레이어 위치 정보")]
    [SerializeField] private Transform _player_Transform;
    public Transform playerTransform
    { get { return _player_Transform; } }

    [Range(150f, 300f)] //아이템과의 거리
    public float _range_Distance = 150f;                          


    protected override void InitManager()
    {
        //임시 스폰 포인트 및 Queue 생성
        _pooling_Item = new Queue<GameObject>();
        _pooling_Spawn = new List<Vector3>();

        _player_Transform = GameManager.Instance.playerObject.transform;

        _scrap_Group = new GameObject();
        _scrap_Group.name = "Scrap_Group";
        _scrap_Group.transform.position = Vector3.zero;
        for (int l = 0; l < _item_Count ; l++)
        {
            F_CreateSpawnPoint();
            F_CreateEtcItem(l);
        }

        StartCoroutine(C_ItemSpawn());
    }

    //아이템 생성 위치 만들기
    private Vector3 F_CreateSpawnPoint()
    {
        float _spawnX = -100f;
        float _spawnY = Random.Range(-5f, 5f);
        float _spawnZ = Random.Range(-10f, 10f);
        Vector3 _spawnPoint = new Vector3(_spawnX, _spawnY, _spawnZ);
        _pooling_Spawn.Add(_spawnPoint);
        return _spawnPoint;
    }
    //재료 아이템 생성 및 Queue에 풀링
    private void F_CreateEtcItem(int v_VectorIdx)
    {
        int _randomNum = Random.Range(0, _items_etc.Length);
        GameObject _itemPooling = Instantiate(_items_etc[_randomNum]);
        _itemPooling.transform.SetParent(_scrap_Group.transform);
        Vector3 _current_SpawnPoint = _pooling_Spawn[v_VectorIdx];
        _itemPooling.transform.position = _current_SpawnPoint;
        _pooling_Item.Enqueue(_itemPooling);
        _itemPooling.SetActive(false);
    }

    //재료 Queue에서 일정 주기마다 재료 아이템 생성
    private IEnumerator C_ItemSpawn()
    {
        while (true)
        {
            while (_pooling_Item.Count > 0)
            {
                F_ItemSpawn();
                yield return new WaitForSeconds(2f);
            }
            //// 풀링에 아이템이 없을 시 새 아이템 생성
            F_CreateSpawnPoint();
            F_CreateEtcItem(_item_Count);
            _item_Count++;
            F_ItemSpawn();
            yield return new WaitForSeconds(2f);        // 새 아이템 생성 후 딜레이
        }
    }

    private void F_ItemSpawn()
    {
        GameObject _spawnedItem = _pooling_Item.Dequeue();
        _spawnedItem.SetActive(true);
    }

    //아이템과의 거리가 멀어지면 다시 Queue에 오브젝트 추가
    public void F_ItemPoolingAdd(GameObject v_destroyedItem)
    {
        _pooling_Item.Enqueue(v_destroyedItem);
        // 자료형 GameObejct -> Item  으로 변경해야함.
        // ★★ 이부분은 Item구조 완성된 이후 수정★★
    }
}
