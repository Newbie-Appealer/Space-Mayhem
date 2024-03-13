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
    [SerializeField] private Transform _scrapGroup;                                                  // ������ ������Ʈ ��Ƶδ� ����
    [SerializeField] private GameObject[] _scrap_Prefabs;                           // ������ ������Ʈ ������ 
    private List<Queue<Scrap>> _pooling_Item;                                       // Ǯ���� ����� ť            
    private List<Vector3> _pooling_SpawnPoint;                                      // ������Ʈ ���� ���� ��ġ

    [Header("ScrapManager Information")]
    [Range(10f, 25f)]
    public float _item_MoveSpeed = 10f;
    [Range(150f, 300f)] 
    public float _range_Distance = 150f;                                            //�����۰��� �Ÿ�
    public Vector3 _scrapVelocity;

    private int _item_Count = 30;
    private int _spawnPointCount = 1000;

    protected override void InitManager()
    {
        _scrapVelocity = new Vector3(_item_MoveSpeed, 0, 0);

        _pooling_Item = new List<Queue<Scrap>>                                      // Queue �ʱ�ȭ
        {
            new Queue<Scrap>(),        // 0_�ö�ƽ
            new Queue<Scrap>(),        // 1_����
            new Queue<Scrap>(),        // 2_��ö
            new Queue<Scrap>()         // 3_�ڽ�
        };

        _pooling_SpawnPoint = new List<Vector3>();                                  // ���� ��ġ �ʱ�ȭ

        _player_Transform = PlayerManager.Instance.playerTransform;                 // �÷��̾� Transform

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

    #region Ǯ�� �ʱ�ȭ
    /// <summary> _spawnPointCount ��ŭ�� ������ ���� ����Ʈ ���� </summary>
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
    /// <summary> v_prefabIndex�� �ش��ϴ� ������Ʈ�� ���� �� Ǯ���� �߰� </summary>
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
        // Ǯ�� �����ִ� ������Ʈ�� ������ 1�� ����
        if (_pooling_Item[v_index].Count == 0) 
            F_CreateScrap(v_index);

        // ��������
        Scrap scrap = _pooling_Item[v_index].Dequeue();

        // ���� ��������Ʈ ���� �� ������ ����
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

            // TODO:���� ������ �������� �����ϱ�
            yield return new WaitForSeconds(1f);
        }
    }
}
