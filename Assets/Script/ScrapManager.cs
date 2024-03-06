using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapManager : Singleton<ScrapManager>
{
    [Header("기본 재료 아이템")]
    [SerializeField] private GameObject[] _items_etc;                                                
    [Range(10f, 25f)] 
    public float _item_MoveSpeed = 10f;
    private Queue<GameObject> _pooling_Item;                                      
    private Transform[] _pooling_Spawn;                                    

    [Header("About Player")]
    [SerializeField] private Transform _player_Transform;
    public Transform playerTransform
    { get { return _player_Transform; } }

    [Range(150f, 300f)]
    public float _range_Distance = 150f;                          


    protected override void InitManager()
    {
        //임시 스폰 포인트 및 Queue 생성
        _pooling_Item = new Queue<GameObject>();
        _pooling_Spawn = new Transform[20];

        _player_Transform = GameManager.Instance.playerObject.transform;

        for (int l = 0; l < 20; l++)
        {
            F_CreateSpawnPoint(l);
        }
        for (int l = 0; l < 100; l++)
        {
            F_CreateEtcItem();
        }

        StartCoroutine(C_ItemSpawn());
    }

    //임시 스폰 포인트 생성
    private void F_CreateSpawnPoint(int v_index)
    {
        float _spawnX = -100f;
        float _spawnY = Random.Range(-5f, 5f);
        float _spawnZ = Random.Range(-10f, 10f);
        GameObject _spawnPooling = new GameObject();
        _spawnPooling.transform.position = new Vector3(_spawnX, _spawnY, _spawnZ);
        _spawnPooling.name = "Spawner_" + v_index;
        _pooling_Spawn[v_index] = _spawnPooling.transform;
    }

    //재료 아이템 생성 및 Queue에 풀링
    private void F_CreateEtcItem()
    {
        int _randomNum = Random.Range(0, _items_etc.Length);
        int _randomSpawnPoint = Random.Range(0, _pooling_Spawn.Length);
        GameObject _itemPooling = Instantiate(_items_etc[_randomNum], _pooling_Spawn[_randomSpawnPoint]);
        _itemPooling.transform.SetParent(_pooling_Spawn[_randomSpawnPoint]);
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
                GameObject _spawnedItem = _pooling_Item.Dequeue();
                _spawnedItem.transform.localPosition = new Vector3(0, 0, 0);
                _spawnedItem.SetActive(true);
                yield return new WaitForSeconds(0.5f);
            }
            F_CreateEtcItem();                                        // 풀링에 아이템이 없을 시 새 아이템 생성
            Debug.Log("아이템 새로 생성 중. . . .");
            yield return new WaitForSeconds(3f);        // 새 아이템 생성 후 딜레이
        }
    }

    //아이템과의 거리가 멀어지면 다시 Queue에 오브젝트 추가
    public void F_ItemPoolingAdd(GameObject v_destroyedItem)
    {
        Debug.Log("풀링에 아이템 추가 중 . . . .");
        _pooling_Item.Enqueue(v_destroyedItem);

        // 자료형 GameObejct -> Item  으로 변경해야함.
        // ★★ 이부분은 Item구조 완성된 이후 수정★★
    }
}
