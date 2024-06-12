using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public enum ScrapType
{
    PLASTIC,
    FIBER,
    SCRAP,
    BOX
}

public enum BoxItems
{
    PLASTIC = 0,
    FIBER = 1,
    SCRAP = 2,
    WOODEN_BOARD = 65
}

public class ScrapManager : Singleton<ScrapManager>
{
    [Header("Player Transform")]
    private Transform _player_Transform;                           
    public Transform playerTransform
    { get { return _player_Transform; } }

    [Header("=== ABOUT Pooling ===")]
    private List<Vector3> _scrap_StartMovePosition;                                      // ������Ʈ ���� ���� ����(�� 8����) ��ġ
    public List<Vector3> scrapStartPosition => _scrap_StartMovePosition;
    [SerializeField] private Transform _scrapGroup;                                     // ������ ������Ʈ ��Ƶδ� ����
    [SerializeField] private GameObject[] _scrap_Prefabs;                           // ������ ������Ʈ ������ 
    [SerializeField] private int _scrap_Count = 10;                                                                   // Scrap �������� ����
    private List<Queue<Scrap>> _pooling_Item;                                        // Ǯ���� ����� ť            
    private List<Vector3> _pooling_SpawnPoint;
    private int _spawnPointCount = 100;

    // ������Ʈ ���� ���� ��ġ

    [Header("ScrapManager Information")]
    [SerializeField] PlanetManager _planetManager;
    [Range(0f, 25f)]
    public float _item_MoveSpeed = 2f;
    [Range(300f, 500f)]
    public float _range_Distance = 300f;                                            //�����۰��� �Ÿ�
    private float _spawn_Distance = 75f;                                            //���� ���� �Ÿ�
    public List<Scrap> _scrapHitedSpear;

    [Header("=== ABOUT SCRAP MOVE ===")] 
    [SerializeField] private Vector3 _scrapVelocity;               // ������ ������ ����
    private Vector3 _currentSpawnPoint;
    private List<Vector3> _scrapVelocity_List;
    private int _randomItemSpawnIdx;      //�����ϰ� ���� Spawn ��ġ �ε���


    protected override void InitManager()
    {
        _player_Transform = PlayerManager.Instance.playerTransform;                 // �÷��̾� Transform

        _randomItemSpawnIdx = Random.Range(0, 8);

        //�÷��̾� �ֺ� �� ���� ���� ���� ��ǥ ����
        _scrapHitedSpear = new List<Scrap>();

        _pooling_Item = new List<Queue<Scrap>>                                      // Queue �ʱ�ȭ
        {
            new Queue<Scrap>(),        // 0_�ö�ƽ
            new Queue<Scrap>(),        // 1_����
            new Queue<Scrap>(),        // 2_��ö
            new Queue<Scrap>()         // 3_�ڽ�
        };

        _pooling_SpawnPoint = new List<Vector3>();                                  // ���� ��ġ �ʱ�ȭ
        _scrap_StartMovePosition = new List<Vector3>();                         //���� Scrap ���� ��ġ
        _scrapVelocity_List = new List<Vector3>();

        for (int index = 0; index < _pooling_Item.Count; index++)
        {
            for (int cnt = 0; cnt < _scrap_Count; cnt++)
            {
                F_CreateScrap(index);
            }
        }

        //���� 8���� Vector3 ����
        for (int l = 0; l < 8; l++)
        {
            F_CreateStartMovePosition(l);
        }

        F_CreateSpawnPoint();
        _currentSpawnPoint = _scrap_StartMovePosition[_randomItemSpawnIdx];
        _scrapGroup.transform.localPosition = _currentSpawnPoint;

        // 8���� Vector3 �ȿ��� ���� _spawnPointCount���� ���� ����Ʈ ����, 8���� Velocity ���� ����
        for (int l = 0; l < 8; l++)
        {
            _scrapVelocity_List.Add(F_SetScrapVelocity(l, _scrap_StartMovePosition[l]));
        }
        _scrapVelocity = _scrapVelocity_List[_randomItemSpawnIdx];
        StartCoroutine("C_ScrapSpawn");
    }

    #region About Pooling

    private void F_CreateStartMovePosition(int v_index)
    {
        float _spawnX = _scrapGroup.position.x;
        float _spawnY = _scrapGroup.position.y;
        float _spawnZ = _scrapGroup.position.z;

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
    void F_CreateSpawnPoint()
    {
        for (int i = 0; i < _spawnPointCount; i++)
        {
            _pooling_SpawnPoint.Add(new Vector3(Random.Range(-35f, 35f), Random.Range(-7f, 7f), Random.Range(-35f, 35f)));
        }
    }

    /// <summary> v_prefabIndex�� �ش��ϴ� ������Ʈ�� ���� �� Ǯ���� �߰� </summary>
    void F_CreateScrap(int v_prefabIndex)
    {
        Scrap scrap = Instantiate(_scrap_Prefabs[v_prefabIndex], _scrapGroup).GetComponent<Scrap>();

        scrap.F_SettingScrap();
        scrap.F_InitScrap(_scrapGroup);

        _pooling_Item[v_prefabIndex].Enqueue(scrap);
    }

    //Pooling���� Scrap �ǵ����� �Լ�
    public void F_ReturnScrap(Scrap scrap)
    {
        _pooling_Item[(int)scrap.scrapType].Enqueue(scrap);
        scrap.F_InitScrap(_scrapGroup);
    }
    #endregion

    #region About Scrap
    void F_SpawnScrap(int v_index, Vector3 v_velocity)
    {
        //Ǯ�� �����ִ� ������Ʈ�� ������ 1�� ����
        if (_pooling_Item[v_index].Count == 0)
            F_CreateScrap(v_index);
        
        // ��������
        Scrap scrap = _pooling_Item[v_index].Dequeue();
        
        // ��������Ʈ ���� �� ������ ����
        Vector3 randomSpawnPoint = _pooling_SpawnPoint[Random.Range(0, _pooling_SpawnPoint.Count)] + playerTransform.position;
        scrap.F_MoveScrap(randomSpawnPoint, v_velocity);
    }

    //Scrap ���� �Լ�
    private IEnumerator C_ScrapSpawn()
    {
        yield return new WaitForSeconds(3f);

        while (true)
        {
            if (!_planetManager.joinPlanet)
            {
                int randomIndex = Random.Range(0, _pooling_Item.Count);

                F_SpawnScrap(randomIndex, _scrapVelocity);

            }
            float _randomDelay = Random.Range(0.5f, 2f);
            yield return new WaitForSeconds(_randomDelay);
        }
    }

    public void F_GetScrapBox(int v_scrapNum, string v_scrapName)
    {
        StartCoroutine(UIManager.Instance.C_GetItemUIOn(ResourceManager.Instance.F_GetInventorySprite(v_scrapNum), v_scrapName));
    }
    #endregion

    #region About Scrap Move
    //���� Scrap�� �������� ���ϴ� �Լ�
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

    //Scrap �帣�� ���� �ٲٴ� �Լ�
    public void F_ScrapMoveChange()
    {
        StopCoroutine("C_ScrapSpawn");
        int _nextRandomIdx = Random.Range(-1, 2);
        int _nextIdx = _randomItemSpawnIdx - _nextRandomIdx;

        if (_nextIdx != _randomItemSpawnIdx)
        {
            if (_nextIdx == -1)
                _nextIdx = 7;
            else if (_nextIdx == 8)
                _nextIdx = 0;
        }
        Debug.Log("���� ���� : " + _nextIdx);
        F_ChangeScrapVelocity(_nextIdx, 5f);
        _scrapVelocity = _scrapVelocity_List[_nextIdx];
        StartCoroutine("C_ScrapSpawn");
        _randomItemSpawnIdx = _nextIdx;
    }
    
    //Scrap�� ���� �帣�� ���� Vector3 ���ϴ� �Լ�
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

    //Scrap���� �θ� ������Ʈ(SpawnPoint)�� �̵���Ű�� �Լ�
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

