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
    [SerializeField] private Transform _scrapGroup;                                 // ������ ������Ʈ ��Ƶδ� ����
    [SerializeField] private GameObject[] _scrap_Prefabs;                           // ������ ������Ʈ ������ 
    private List<Queue<Scrap>> _pooling_Item;                                       // Ǯ���� ����� ť            
    private List<Vector3> _scrap_StartMovePosition;                                      // ������Ʈ ���� ���� ����(�� 8����) ��ġ
    public List<Vector3> scrapStartPosition => _scrap_StartMovePosition;
    // ������Ʈ ���� ���� ��ġ
    // 0 ~ 99 : 1�� ����, 100 ~ 199 : 2�� ����, 200 ~ 299 : 3�� ���� ~
    private Dictionary<int, Vector3[]> _pooling_SpawnPoint;
    private int _randomItemSpawnIdx;
    private int _pooling_Key = 0;

    [Header("ScrapManager Information")]
    [Range(0f, 25f)]
    public float _item_MoveSpeed = 2f;
    [Range(150f, 300f)]
    public float _range_Distance = 200f;                                            //�����۰��� �Ÿ�
    private float _spawn_Distance = 100f;                                            //�����۰��� �Ÿ�
    public Vector3 _scrapToMoveVelocity; //���� �÷��̾� �ֺ� �� ���� ���� ��ǥ
    public Vector3 _scrapVelocity; //������ ������ ����
    public List<Scrap> _scrapHitedSpear;

    private int _item_Count = 1;
    private int _spawnPointCount = 100;

    protected override void InitManager()
    {
        _randomItemSpawnIdx = Random.Range(0, 8);

        //�÷��̾� �ֺ� �� ���� ���� ���� ��ǥ ����
        _scrapToMoveVelocity = Random.insideUnitSphere * MeteorManager.Instance.player_SphereCollider.radius;
        _scrapVelocity = new Vector3(_item_MoveSpeed, 0, 0);
        _scrapHitedSpear = new List<Scrap>();

        _pooling_Item = new List<Queue<Scrap>>                                      // Queue �ʱ�ȭ
        {
            new Queue<Scrap>(),        // 0_�ö�ƽ
            new Queue<Scrap>(),        // 1_����
            new Queue<Scrap>(),        // 2_��ö
            new Queue<Scrap>()         // 3_�ڽ�
        };

        _pooling_SpawnPoint = new Dictionary<int, Vector3[]>();                                  // ���� ��ġ �ʱ�ȭ
        _scrap_StartMovePosition = new List<Vector3>();                         //���� Scrap ���� ��ġ
        _player_Transform = PlayerManager.Instance.playerTransform;                 // �÷��̾� Transform

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
        //���� Scrap Velocity �����ֱ�
        _scrapVelocity = F_SetScrapVelocity(_randomItemSpawnIdx, F_SettingScrapSpawnPoint(_randomItemSpawnIdx), _scrap_StartMovePosition[_randomItemSpawnIdx]);
        StartCoroutine("C_ItemSpawn", _randomItemSpawnIdx);
        ////�켱 ���� ���� �� 10�� �� ���� ����, Coroutine �������� ������ ���� ���� ���� ����
        StartCoroutine(C_ScrapMoveChange());
    }

    #region Ǯ�� �ʱ�ȭ

    private void F_CreateStartMovePosition(int v_index)
    {
        float _spawnX = _scrapToMoveVelocity.x;
        float _spawnY = _scrapToMoveVelocity.y;
        float _spawnZ = _scrapToMoveVelocity.z;
        //8���� �� �� ó�� �ʱ� ���� ���� ���� 
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

    /// <summary> 8���� �� ���� ���� ����Ʈ, �� �ȿ��� _spawnPointCount�� ��ŭ ������ ���� ����Ʈ ���� </summary>
    void F_CreateSpawnPoint(List<Vector3> v_startPosition, int v_index)
    {
        Vector3 _startVector3 = v_startPosition[v_index];
        _pooling_SpawnPoint[_pooling_Key] = new Vector3[_spawnPointCount];
        for (int i = 0; i < _spawnPointCount; i++)
        {
            _pooling_SpawnPoint[_pooling_Key][i] = new Vector3(_startVector3.x - Random.Range(-20f, 20f), _startVector3.y - Random.Range(-10f, 10f), _startVector3.z - Random.Range(-20f, 20f));
        }
        _pooling_Key++;
    }

    /// <summary> v_prefabIndex�� �ش��ϴ� ������Ʈ�� ���� �� Ǯ���� �߰� </summary>
    void F_CreateScrap(int v_prefabIndex)
    {
        Scrap scrap = Instantiate(_scrap_Prefabs[v_prefabIndex], _scrapGroup).GetComponent<Scrap>();

        scrap.F_SettingScrap();
        scrap.F_InitScrap(_scrapGroup);

        _pooling_Item[v_prefabIndex].Enqueue(scrap);
    }
    #endregion

    void F_SpawnScrap(int v_index, int v_startPositionIdx, Vector3 v_velocity, Vector3 v_RandomStart)
    {
        //Ǯ�� �����ִ� ������Ʈ�� ������ 1�� ����
        if (_pooling_Item[v_index].Count == 0)
            F_CreateScrap(v_index);
        
        // ��������
        Scrap scrap = _pooling_Item[v_index].Dequeue();

        // ���� ��������Ʈ ���� �� ������ ����
        Vector3 randomSpawnPoint = _pooling_SpawnPoint[v_startPositionIdx][Random.Range(0, _spawnPointCount)];
        scrap.F_MoveScrap(randomSpawnPoint, v_velocity);
    }

    //startPosition 8���� ���� 100���� ��ġ ������ �ϳ� �̱�
    private Vector3 F_SettingScrapSpawnPoint(int v_startPositionIdx)
    {
        return _pooling_SpawnPoint[v_startPositionIdx][Random.Range(0, _spawnPointCount)];
    }

    //pooling���� �ǵ�����
    public void F_ReturnScrap(Scrap scrap)
    {
        _pooling_Item[scrap.scrapNumber].Enqueue(scrap);
        scrap.F_InitScrap(_scrapGroup);
    }

    private IEnumerator C_ItemSpawn(int v_index)
    {
        yield return new WaitForSeconds(3f);

        //Scrap�� Spawn�ϸ鼭 ���� ��ġ �ٲ��ֱ�
        while (true)
        {
            int randomIndex = Random.Range(0,_pooling_Item.Count);

            Vector3 _randomSpawnPosition = F_SettingScrapSpawnPoint(v_index);
            F_SpawnScrap(randomIndex, v_index, _scrapVelocity, _randomSpawnPosition);

            int _randomDelay = Random.Range(2, 4);
            yield return new WaitForSeconds(_randomDelay);
        }
    }

    private Vector3 F_SetScrapVelocity(int v_startPositionIdx, Vector3 v_randomSpawnPoint, Vector3 v_startSpawnVector)
    {
        switch (v_startPositionIdx)
        {
            case 0:
                {
                    _scrapVelocity = new Vector3(-(v_randomSpawnPoint.x + v_startSpawnVector.x), 0, 0).normalized;
                    break;
                }
            case 1:
                {
                    _scrapVelocity = new Vector3(-(v_randomSpawnPoint.x + v_startSpawnVector.x), 0, -(v_randomSpawnPoint.z + v_startSpawnVector.z)).normalized;
                    break;
                }
            case 2:
                {
                    _scrapVelocity = new Vector3(0, 0, -(v_randomSpawnPoint.z + v_startSpawnVector.z)).normalized;
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
        yield return new WaitForSeconds(10f);
        while(true)
        {
            StopCoroutine("C_ItemSpawn");
            int _nextRandomIdx = Random.Range(0, 8);
            
            if(_nextRandomIdx == _randomItemSpawnIdx)
                Debug.Log("���� ����, 30�� �� �ٽ� ���� ����");
            else
            {
                Debug.Log("���� ���� : " + _nextRandomIdx);
                //�ݴ� ����
                if (_nextRandomIdx - _randomItemSpawnIdx == -4 || _nextRandomIdx - _randomItemSpawnIdx == 4)
                    F_ChangeScrapVelocity(_randomItemSpawnIdx, _nextRandomIdx, 5f);

                //���� ��ǥ���� ���� �а��� ��
                else if (_nextRandomIdx - _randomItemSpawnIdx == -3 || _nextRandomIdx - _randomItemSpawnIdx == -5 || _nextRandomIdx - _randomItemSpawnIdx == 3 || _nextRandomIdx - _randomItemSpawnIdx == 5)
                    F_ChangeScrapVelocity(_randomItemSpawnIdx, _nextRandomIdx, 10f);

                else
                    F_ChangeScrapVelocity(_randomItemSpawnIdx, _nextRandomIdx, 15f);

                StartCoroutine("C_ItemSpawn", _nextRandomIdx);
                _randomItemSpawnIdx = _nextRandomIdx;
                _scrapVelocity = F_SetScrapVelocity(_nextRandomIdx, F_SettingScrapSpawnPoint(_nextRandomIdx), _scrap_StartMovePosition[_nextRandomIdx]);
                yield return new WaitForSeconds(30f);
            }
        }
    }

    public void F_ChangeScrapVelocity(int v_currentSpawnIdx ,int v_nextSpawnIdx, float v_changeSpeed)
    {
        int count = 0;
        Vector3 _currentVelocity = _scrap_StartMovePosition[v_currentSpawnIdx];
        Vector3 _newVelocity = _scrap_StartMovePosition[v_nextSpawnIdx];
        
        Vector3 _nextScrapVelocity = new Vector3(-(_newVelocity.x - _currentVelocity.x), 0, -(_newVelocity.z - _currentVelocity.z)).normalized * _item_MoveSpeed;
        for(int l = 0 ; l < _scrapGroup.transform.childCount; l++)
        {
            if (_scrapGroup.transform.GetChild(l).gameObject.activeSelf)
            {
                StartCoroutine(_scrapGroup.transform.GetChild(l).gameObject.GetComponent<Scrap>().C_ItemVelocityChange(_nextScrapVelocity, v_changeSpeed));
                count++;
            }
            else
                continue;
        }
        Debug.Log("�̵� ���� ������Ʈ �� : " + count + ", ���� �ӵ� : " + v_changeSpeed);

    }
}


