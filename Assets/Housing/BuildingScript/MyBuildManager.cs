using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

[System.Serializable]
public enum MySelectedBuildType
{
    floor,
    celling,
    wall,
    door,
    window,
    ladder,
    repair
}

public class MyBuildManager : Singleton<MyBuildManager>
{
    [Header("Player")]
    public GameObject _player;

    [Header("CheckBulidBlock")]
    [SerializeField] MyBuildCheck _mybuildCheck;
    [SerializeField] Transform _parentTransform;

    [Header("BundleBuildingPrepab")]
    [SerializeField] public List<List<GameObject>> _bundleBulingPrefab;

    [Header("Build Object")]
    [SerializeField] private List<GameObject> _floorList;
    [SerializeField] private List<GameObject> _cellingList;
    [SerializeField] private List<GameObject> _wallList;
    [SerializeField] private List<GameObject> _doorList;
    [SerializeField] private List<GameObject> _windowList;
    [SerializeField] private List<GameObject> _ladderList;
    [SerializeField] private List<GameObject> _repairList;

    [Header("LayerMask")]
    [SerializeField] LayerMask _nowTempLayer;       // �׷��� ���� ���̾�
    [SerializeField] int _buildingBlocklayer;       // ���� ���� �ִ� ���� layer (�÷��̾� / �ٸ����� �浹 x)
    [SerializeField] int _buildFinishedLayer;       // �� ���� ���� layer 
    [SerializeField] int _dontRaycastLayer;         // temp ��ġ �߿� temp block�� �浹���� ����
    [SerializeField] List< Tuple<LayerMask , int > > _tempUnderBlockLayer;   // �ӽ� �� ���̾�
                                                                             // 0. Temp floor ���̾�
                                                                             // 1. Temp celling ���̾�
                                                                             // 2. Temp wall ���̾�
    [SerializeField] public LayerMask _tempWholeLayer;     //  temp floor , celling, wall ���̾� �� ��ģ

    [Header( "Type")]
    [SerializeField] private MySelectedBuildType _mySelectBuildType;
   
    [Header("Temp Object Setting")]
    [SerializeField] int _buildTypeIdx;     // ���� Ÿ������
    [SerializeField] int _buildDetailIdx;   // �� Ÿ�Ծ� ���° ������Ʈ ����
    [SerializeField] bool _isTempValidPosition = false;             // �ӽ� ������Ʈ�� ������ �� �ִ���
    [SerializeField] bool _isEnoughResource = false;                // ��ġ�ϱ⿡ ��ᰡ �������?

    [Header("Tesmp Object Building")]
    [SerializeField] GameObject _TempObjectBuilding;        // �ӽ� ������Ʈ
    [SerializeField] List<Transform> _tempUnderParentTrs;   // �ӽ� ������Ʈ ���� �� �θ� trs
        // 0. �ӽ� ������Ʈ model �θ�
        // 1. `` tempFloor �θ�
        // 2. `` celling �θ�
        // 3. `` tempWall �θ�
        // 4. (wall �϶���) `` ladder �θ� 
    [SerializeField] Transform _otehrConnectorTr;       // �浹�� �ٸ� ������Ʈ 

    [Header("Ori Material")]
    [SerializeField] Material _oriMaterial;
    [SerializeField] Material _nowBuildMaterial;
    [SerializeField] Material _greenMaterial;
    [SerializeField] Material _redMaterial;

    // ������Ƽ
    public int BuildFinishedLayer { get => _buildFinishedLayer; }

    // �̱��� ( awake ���� )
    protected override void InitManager()
    {
        F_InitLayer();          // ���̾� �ʱ�ȭ
        F_InitBundleBlock();    // �� prefab �� list�ϳ��� �ʱ�ȭ
    
        // ## TODO ������
        SaveManager.Instance.F_LoadBuilding(_parentTransform);
    }

    #region 1. ���̾� �ʱ�ȭ 2. ������ list

    private void F_InitLayer() 
    {
        _dontRaycastLayer = LayerMask.NameToLayer("DontRaycastSphere");
        _buildingBlocklayer = LayerMask.NameToLayer("BuildingBlock");
        _buildFinishedLayer = LayerMask.NameToLayer("BuildFinishedBlock");

        _tempUnderBlockLayer = new List<Tuple<LayerMask, int>>
        {
            new Tuple <LayerMask , int>( LayerMask.GetMask("TempFloorLayer") , 17 ),        // temp floor ���̾�
            new Tuple <LayerMask , int>( LayerMask.GetMask("TempCellingLayer") , 16 ),      // temp celling ���̾�
            new Tuple <LayerMask , int>( LayerMask.GetMask("TempWallLayer") , 15 ),         // temp wall ���̾�
            
        };

        _tempWholeLayer = _tempUnderBlockLayer[0].Item1 | _tempUnderBlockLayer[1].Item1 | _tempUnderBlockLayer[2].Item1;
           
    }

    private void F_InitBundleBlock() 
    {
        // �� block ������ List�� �ϳ��� List�� ����
        _bundleBulingPrefab = new List<List<GameObject>> 
        {
            _floorList,
            _cellingList,
            _wallList,
            _doorList,
            _windowList,
            _ladderList,
            _repairList
        };
    }
    #endregion

    private void Update()
    {
        Debug.DrawRay(_player.transform.position , _player.transform.forward * 10f , Color.red);

        // ##TODO ������
        // L ������ building ����
        if (Input.GetKeyDown(KeyCode.L))
            SaveManager.Instance.F_SaveBuilding(_parentTransform.transform);
    }

    public void F_GetbuildType( int v_type = 0 , int v_detail = 1) 
    {
        // 0. Ui�� �߸��� idx�� �Ѿ���� �� 
        if (v_type < 0 && v_detail < 0)
            return;

        // 1. index �ʱ�ȭ
        _buildTypeIdx = v_type;
        _buildDetailIdx = v_detail;

        // 2. �ӽ� ������Ʈ Ȯ��
        if (_TempObjectBuilding != null)
            Destroy(_TempObjectBuilding);
        _TempObjectBuilding = null;

        // �ʹݿ� 1ȸ ����
        _mybuildCheck.F_BuildingStart();

        // 3. ���� ���� 
        StopAllCoroutines();
        StartCoroutine( F_TempBuild() );
    }

    IEnumerator F_TempBuild() 
    {
        // 0. index�� �ش��ϴ� ���� ������Ʈ return
        GameObject _currBuild = F_GetCurBuild();
        // 0.1. �� �� Ÿ�Կ� ���� �˻��� layer ���ϱ�
        F_TempRaySetting( _mySelectBuildType );         

        while (true) 
        {
            // 1. index�� �ش��ϴ� ���ӿ�����Ʈ ����
            F_CreateTempPrefab(_currBuild);

            // 2. �ش� ���̶� ���� Layer�� raycast
            F_Raycast(_nowTempLayer);

            // 3. temp ������Ʈ�� �ݶ��̴� �˻�
            F_CheckCollision();

            // 4. ��ġ
            if (Input.GetMouseButtonDown(0))
                F_FinishBuild();

            // update ȿ�� 
            yield return null;     
        }
    }

    #region ray , snap ����

    private void F_Raycast( LayerMask v_layer ) 
    {
        // �Ѿ�� Layer�� ���� raycast
        RaycastHit _hit;

        if (Physics.Raycast( _player.transform.position  , _player.transform.forward * 10 , out _hit , 5f , v_layer)) // Ÿ�� : LayerMask
        {
            _TempObjectBuilding.transform.position = _hit.point;
        }
    }

    private void F_CheckCollision()
    {
        Collider[] _coll = Physics.OverlapSphere(_TempObjectBuilding.transform.position, 1f, _nowTempLayer);    // Ÿ�� : LayerMask

        if (_coll.Length > 0)
            F_Snap(_coll);
        else
            _isTempValidPosition = false; 

    }

    private void F_Snap(Collider[] v_coll ) 
    {
        _otehrConnectorTr = null; 

        for (int i = 0; i < v_coll.Length; i++)
        {

            if (v_coll[i].GetComponent<MyConnector>()._canConnect == true)
            {
                _otehrConnectorTr = v_coll[i].transform;
                break;
            }
        }

        // ��ġ ������ ��ġ�� ������
        if (_otehrConnectorTr == null)
        {
            F_ChangeMesh(_tempUnderParentTrs[0] , false);
            _isTempValidPosition = false;
            return;
        }

        _TempObjectBuilding.transform.position
             = _otehrConnectorTr.position;

        F_ChangeMesh(_tempUnderParentTrs[0], true);
        _isTempValidPosition = true;
    }

    private GameObject F_GetCurBuild() 
    {
        switch (_buildTypeIdx)
        {
            case 0:
                _mySelectBuildType = MySelectedBuildType.floor;
                return _floorList[_buildDetailIdx];
            case 1:
                _mySelectBuildType = MySelectedBuildType.celling; 
                return _cellingList[_buildDetailIdx];
            case 2:
                _mySelectBuildType = MySelectedBuildType.wall;
                return _wallList[_buildDetailIdx];
            case 3:
                _mySelectBuildType = MySelectedBuildType.door;
                return _doorList[_buildDetailIdx];
            case 4:
                _mySelectBuildType = MySelectedBuildType.window;
                return _windowList[_buildDetailIdx];
            case 5:
                _mySelectBuildType = MySelectedBuildType.ladder;
                return _ladderList[_buildDetailIdx];
            case 6:
                _mySelectBuildType = MySelectedBuildType.repair;
                return _repairList[_buildDetailIdx];
            default:
                return null;
        }
    }

    private void F_TempRaySetting(MySelectedBuildType v_type)
    {
        switch (v_type)
        {
            case MySelectedBuildType.floor:
                _nowTempLayer = _tempUnderBlockLayer[0].Item1;          // floor ���̾�
                break;
            case MySelectedBuildType.celling:
                _nowTempLayer = _tempUnderBlockLayer[1].Item1;          // celling ���̾�
                break;
            case MySelectedBuildType.wall:                              // window, door, window�� ���� wall ���̾� ��� 
            case MySelectedBuildType.door:
            case MySelectedBuildType.window:
                _nowTempLayer = _tempUnderBlockLayer[2].Item1;          // wall ���̾�
                break;
        }

    }


    private void F_CreateTempPrefab( GameObject v_temp) 
    {
        // ��������
        // temp������Ʈ�� null�̵Ǹ� �ٷ� ������!
        if (_TempObjectBuilding == null)
        {
            // 1. ����
            _TempObjectBuilding = Instantiate(v_temp);

            // 2. �θ� trs �ʱ�ȭ
            _tempUnderParentTrs = new List<Transform>
            {
                _TempObjectBuilding.transform.GetChild(0),      // model parent
                _TempObjectBuilding.transform.GetChild(1),      // floor parent
                _TempObjectBuilding.transform.GetChild(2),      // celling parent
                _TempObjectBuilding.transform.GetChild(3),      // wall parent
            };

            // 2. �� Parent ���� ������Ʈ�� ���̾� �ٲٱ�
            for (int i = 1; i < _tempUnderParentTrs.Count; i++) 
            {
                F_ChangeLayer(_tempUnderParentTrs[i], _dontRaycastLayer);        
            }  
            F_ChangeLayer(_tempUnderParentTrs[0], _buildingBlocklayer , true);       // model�� layer ���� , �ٸ��Ͱ� �浹 �ȵǰ�

            // 3. ���� material ����
            _oriMaterial = _tempUnderParentTrs[0].GetChild(0).GetComponent<MeshRenderer>().material;

            // 4. mybuilding check�� ���۽���
            F_BuldingInitCheckBuild();

            // 5. modeld�� Material �ٲٱ�
            F_ChangeMaterial(_tempUnderParentTrs[0], _nowBuildMaterial);
        }
    }

    private void F_BuldingInitCheckBuild() 
    {
        // 1. �ʱ�ȭ
        _mybuildCheck.F_BuildingStart();

        // 2. ��ᰡ �������? ����ϸ� true,  �ƴϸ� false
        _isEnoughResource = _mybuildCheck.F_WholeSourseIsEnough();

        // 3. �����е��� ���� material ��ȭ
        if (_isEnoughResource)
            _nowBuildMaterial = _greenMaterial;
        else
            _nowBuildMaterial = _redMaterial;
    }

    #endregion

    #region building ���� �� (��ġ)

    private void F_FinishBuild() 
    {
        // 1. �κ� �� ��ᰡ ����ϸ� -> ���� 
        if (_isEnoughResource == true)
        {
            // 2. ����
            F_BuildTemp();

            // 3. �κ��丮 ������Ʈ
            _mybuildCheck.F_UpdateInvenToBuilding();
        }
        else
            return;
    }
    
    private void F_BuildTemp() 
    { 
        if( _TempObjectBuilding != null && _isTempValidPosition == true) 
        { 
            // 0. ����
            GameObject _nowbuild = Instantiate(F_GetCurBuild(), _TempObjectBuilding.transform.position , _TempObjectBuilding.transform.rotation , _parentTransform);

            // 1. destory
            Destroy( _TempObjectBuilding);
            _TempObjectBuilding = null;

            // 3. model�� material ����
            F_ChangeMaterial(_nowbuild.transform.GetChild(0) , _oriMaterial);

            // 4. model�� layer (buildFinished��) ����
            F_ChangeLayer( _nowbuild.transform.GetChild(0) , _buildFinishedLayer , true );

            // 5. �� ������Ʈ�� �´� temp ���̾�� ��ȯ
            for (int i = 1; i < _nowbuild.transform.childCount - 1; i++) 
            {
                F_ChangeLayer( _nowbuild.transform.GetChild(i) , _tempUnderBlockLayer[i-1].Item2 );   
            }

            // 6. ���� ���θ��� block Connector ������Ʈ
            F_ConeectorUpdate(_nowbuild.transform);

            // 8. ���� ���� ���� block�� MyBuildingBlock �߰�
            if (_nowbuild.GetComponent<MyBuildingBlock>() == null)
            {
                // null �̸� �߰��ϱ� 
                _nowbuild.AddComponent<MyBuildingBlock>();
            }

            // 9. nowBuild�� �浹�� Ŀ���͵� ������Ʈ
            MyBuildingBlock _nowBuildBlock = _nowbuild.GetComponent<MyBuildingBlock>();
            _nowBuildBlock.F_BlockCollisionConnector();

            // 8-1. block�� �ʵ� �ʱ�ȭ 
            _nowBuildBlock.F_SetBlockFeild(_buildTypeIdx, _buildDetailIdx % 10, _mybuildCheck._myblock.BlockHp);

        }
    }

    // Ŀ���� ������Ʈ 
    public void F_ConeectorUpdate( Transform v_pa ) 
    {
        for (int i = 1; i <= 3; i ++) 
        {
            foreach (MyConnector mc in v_pa.transform.GetChild(i).GetComponentsInChildren<MyConnector>())
            {
                mc.F_UpdateConnector(); 
            }
        }
    }

    #endregion

    #region saveManager (�ʱ� 9�� floor �����ϱ�)
    public void F_FirstInitBaseFloor() 
    {
        _buildTypeIdx = 0;
        _buildDetailIdx = 0;

        Vector3 _buildVec = Vector3.zero;
        for (int i = 0; i < 3; i++) 
        {
            for (int j = 0; j < 3; j++) 
            {
                _buildVec = new Vector3( j * 5 , 0 , i * -5);
                GameObject _nowbuild = Instantiate(F_GetCurBuild(), _buildVec, Quaternion.identity , _parentTransform);
            }
        }
    }

    #endregion

    #region chagneEct
    private void F_ChangeMaterial( Transform v_pa , Material material ) 
    {
        foreach ( MeshRenderer msr in v_pa.GetComponentsInChildren<MeshRenderer>()) 
        {
            msr.material = material;
        }
    }

    private void F_ChangeMesh( Transform v_pa , bool v_flag) 
    {
        foreach (MeshRenderer msr in v_pa.GetComponentsInChildren<MeshRenderer>())
        {
            msr.enabled = v_flag;
        }
    }

    // ���̾� ����
    // �θ� ���̾� ���� �� v_flag�� true
    private void F_ChangeLayer( Transform v_pa , int v_layer , bool v_flag = false )    // ���ӿ�����Ʈ�� ���̾ �ٲ� ���� int ������
    {
        if (v_flag)
            v_pa.gameObject.layer = v_layer;

        else 
        {
            for (int i = 0; i < v_pa.childCount; i++)
            {
                v_pa.GetChild(i).gameObject.layer = v_layer;
            }
        }
    }
    #endregion

    #region Player Controller
    
    // �Ǽ� ���� ������ �� �ʱ�ȭ �Լ� 
    public void F_InitBuildngMode() 
    {
        // 0. ���� �����ϰ� �ִ� building �ڷ�ƾ ���� 
        StopAllCoroutines();
    
        // 1. buildingProgressUi ����
        HousingUiManager.Instance._buildingProgressUi.gameObject.SetActive(false);

        // 2. Ȥ�ó� �������� housing ui�� ���� 
        //HousingUiManager.Instance._buildingCanvas.gameObject.SetActive(false);

        // 3. preview ������Ʈ ����
        if (_TempObjectBuilding != null)
            Destroy(_TempObjectBuilding);
        _TempObjectBuilding = null;

    }

    #endregion
}
