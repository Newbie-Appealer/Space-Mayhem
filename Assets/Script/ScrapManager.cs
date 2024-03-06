using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapManager : Singleton<ScrapManager>
{
    [Header("�⺻ ��� ������")]
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
        //�ӽ� ���� ����Ʈ �� Queue ����
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

    //�ӽ� ���� ����Ʈ ����
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

    //��� ������ ���� �� Queue�� Ǯ��
    private void F_CreateEtcItem()
    {
        int _randomNum = Random.Range(0, _items_etc.Length);
        int _randomSpawnPoint = Random.Range(0, _pooling_Spawn.Length);
        GameObject _itemPooling = Instantiate(_items_etc[_randomNum], _pooling_Spawn[_randomSpawnPoint]);
        _itemPooling.transform.SetParent(_pooling_Spawn[_randomSpawnPoint]);
        _pooling_Item.Enqueue(_itemPooling);
        _itemPooling.SetActive(false);
    }
    //��� Queue���� ���� �ֱ⸶�� ��� ������ ����
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
            F_CreateEtcItem();                                        // Ǯ���� �������� ���� �� �� ������ ����
            Debug.Log("������ ���� ���� ��. . . .");
            yield return new WaitForSeconds(3f);        // �� ������ ���� �� ������
        }
    }

    //�����۰��� �Ÿ��� �־����� �ٽ� Queue�� ������Ʈ �߰�
    public void F_ItemPoolingAdd(GameObject v_destroyedItem)
    {
        Debug.Log("Ǯ���� ������ �߰� �� . . . .");
        _pooling_Item.Enqueue(v_destroyedItem);

        // �ڷ��� GameObejct -> Item  ���� �����ؾ���.
        // �ڡ� �̺κ��� Item���� �ϼ��� ���� �����ڡ�
    }
}
