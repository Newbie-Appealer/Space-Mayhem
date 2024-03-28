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
    [SerializeField] LayerMask _nowTempLayer;       // 그래서 현재 레이어
    [SerializeField] int _buildingBlocklayer;       // 현재 짓고 있는 블럭의 layer (플레이어 / 다른블럭과 충돌 x)
    [SerializeField] int _buildFinishedLayer;       // 다 지은 블럭의 layer 
    [SerializeField] int _dontRaycastLayer;         // temp 설치 중에 temp block의 충돌감지 방지
    [SerializeField] List< Tuple<LayerMask , int > > _tempUnderBlockLayer;   // 임시 블럭 레이어
                                                                             // 0. Temp floor 레이어
                                                                             // 1. Temp celling 레이어
                                                                             // 2. Temp wall 레이어
    [SerializeField] public LayerMask _tempWholeLayer;     //  temp floor , celling, wall 레이어 다 합친

    [Header( "Type")]
    [SerializeField] private MySelectedBuildType _mySelectBuildType;
   
    [Header("Temp Object Setting")]
    [SerializeField] int _buildTypeIdx;     // 무슨 타입인지
    [SerializeField] int _buildDetailIdx;   // 그 타입안 몇번째 오브젝트 인지
    [SerializeField] bool _isTempValidPosition = false;             // 임시 오브젝트가 지어질 수 있는지
    [SerializeField] bool _isEnoughResource = false;                // 설치하기에 재료가 충분한지?

    [Header("Tesmp Object Building")]
    [SerializeField] GameObject _TempObjectBuilding;        // 임시 오브젝트
    [SerializeField] List<Transform> _tempUnderParentTrs;   // 임시 오브젝트 밑의 각 부모 trs
        // 0. 임시 오브젝트 model 부모
        // 1. `` tempFloor 부모
        // 2. `` celling 부모
        // 3. `` tempWall 부모
        // 4. (wall 일때만) `` ladder 부모 
    [SerializeField] Transform _otehrConnectorTr;       // 충돌한 다른 오브젝트 

    [Header("Ori Material")]
    [SerializeField] Material _oriMaterial;
    [SerializeField] Material _nowBuildMaterial;
    [SerializeField] Material _greenMaterial;
    [SerializeField] Material _redMaterial;

    // 프로퍼티
    public int BuildFinishedLayer { get => _buildFinishedLayer; }

    // 싱글톤 ( awake 역할 )
    protected override void InitManager()
    {
        F_InitLayer();          // 레이어 초기화
        F_InitBundleBlock();    // 블럭 prefab 을 list하나로 초기화
    
        // ## TODO 저장기능
        SaveManager.Instance.F_LoadBuilding(_parentTransform);
    }

    #region 1. 레이어 초기화 2. 프리팹 list

    private void F_InitLayer() 
    {
        _dontRaycastLayer = LayerMask.NameToLayer("DontRaycastSphere");
        _buildingBlocklayer = LayerMask.NameToLayer("BuildingBlock");
        _buildFinishedLayer = LayerMask.NameToLayer("BuildFinishedBlock");

        _tempUnderBlockLayer = new List<Tuple<LayerMask, int>>
        {
            new Tuple <LayerMask , int>( LayerMask.GetMask("TempFloorLayer") , 17 ),        // temp floor 레이어
            new Tuple <LayerMask , int>( LayerMask.GetMask("TempCellingLayer") , 16 ),      // temp celling 레이어
            new Tuple <LayerMask , int>( LayerMask.GetMask("TempWallLayer") , 15 ),         // temp wall 레이어
            
        };

        _tempWholeLayer = _tempUnderBlockLayer[0].Item1 | _tempUnderBlockLayer[1].Item1 | _tempUnderBlockLayer[2].Item1;
           
    }

    private void F_InitBundleBlock() 
    {
        // 각 block 프리팹 List를 하나의 List로 묶기
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

        // ##TODO 저장기능
        // L 누르면 building 저장
        if (Input.GetKeyDown(KeyCode.L))
            SaveManager.Instance.F_SaveBuilding(_parentTransform.transform);
    }

    public void F_GetbuildType( int v_type = 0 , int v_detail = 1) 
    {
        // 0. Ui상 잘못된 idx가 넘어왔을 때 
        if (v_type < 0 && v_detail < 0)
            return;

        // 1. index 초기화
        _buildTypeIdx = v_type;
        _buildDetailIdx = v_detail;

        // 2. 임시 오브젝트 확인
        if (_TempObjectBuilding != null)
            Destroy(_TempObjectBuilding);
        _TempObjectBuilding = null;

        // 초반에 1회 실행
        _mybuildCheck.F_BuildingStart();

        // 3. 동작 시작 
        StopAllCoroutines();
        StartCoroutine( F_TempBuild() );
    }

    IEnumerator F_TempBuild() 
    {
        // 0. index에 해당하는 게임 오브젝트 return
        GameObject _currBuild = F_GetCurBuild();
        // 0.1. 내 블럭 타입에 따라 검사할 layer 정하기
        F_TempRaySetting( _mySelectBuildType );         

        while (true) 
        {
            // 1. index에 해당하는 게임오브젝트 생성
            F_CreateTempPrefab(_currBuild);

            // 2. 해당 블럭이랑 같은 Layer만 raycast
            F_Raycast(_nowTempLayer);

            // 3. temp 오브젝트의 콜라이더 검사
            F_CheckCollision();

            // 4. 설치
            if (Input.GetMouseButtonDown(0))
                F_FinishBuild();

            // update 효과 
            yield return null;     
        }
    }

    #region ray , snap 동작

    private void F_Raycast( LayerMask v_layer ) 
    {
        // 넘어온 Layer에 따라 raycast
        RaycastHit _hit;

        if (Physics.Raycast( _player.transform.position  , _player.transform.forward * 10 , out _hit , 5f , v_layer)) // 타입 : LayerMask
        {
            _TempObjectBuilding.transform.position = _hit.point;
        }
    }

    private void F_CheckCollision()
    {
        Collider[] _coll = Physics.OverlapSphere(_TempObjectBuilding.transform.position, 1f, _nowTempLayer);    // 타입 : LayerMask

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

        // 설치 가능한 위치가 없으면
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
                _nowTempLayer = _tempUnderBlockLayer[0].Item1;          // floor 레이어
                break;
            case MySelectedBuildType.celling:
                _nowTempLayer = _tempUnderBlockLayer[1].Item1;          // celling 레이어
                break;
            case MySelectedBuildType.wall:                              // window, door, window는 같은 wall 레이어 사용 
            case MySelectedBuildType.door:
            case MySelectedBuildType.window:
                _nowTempLayer = _tempUnderBlockLayer[2].Item1;          // wall 레이어
                break;
        }

    }


    private void F_CreateTempPrefab( GameObject v_temp) 
    {
        // 실행조건
        // temp오브젝트가 null이되면 바로 생성됨!
        if (_TempObjectBuilding == null)
        {
            // 1. 생성
            _TempObjectBuilding = Instantiate(v_temp);

            // 2. 부모 trs 초기화
            _tempUnderParentTrs = new List<Transform>
            {
                _TempObjectBuilding.transform.GetChild(0),      // model parent
                _TempObjectBuilding.transform.GetChild(1),      // floor parent
                _TempObjectBuilding.transform.GetChild(2),      // celling parent
                _TempObjectBuilding.transform.GetChild(3),      // wall parent
            };

            // 2. 각 Parent 밑의 오브젝트의 레이어 바꾸기
            for (int i = 1; i < _tempUnderParentTrs.Count; i++) 
            {
                F_ChangeLayer(_tempUnderParentTrs[i], _dontRaycastLayer);        
            }  
            F_ChangeLayer(_tempUnderParentTrs[0], _buildingBlocklayer , true);       // model의 layer 변경 , 다른것과 충돌 안되게

            // 3. 원래 material 저장
            _oriMaterial = _tempUnderParentTrs[0].GetChild(0).GetComponent<MeshRenderer>().material;

            // 4. mybuilding check의 동작실행
            F_BuldingInitCheckBuild();

            // 5. modeld의 Material 바꾸기
            F_ChangeMaterial(_tempUnderParentTrs[0], _nowBuildMaterial);
        }
    }

    private void F_BuldingInitCheckBuild() 
    {
        // 1. 초기화
        _mybuildCheck.F_BuildingStart();

        // 2. 재료가 충분한지? 충분하면 true,  아니면 false
        _isEnoughResource = _mybuildCheck.F_WholeSourseIsEnough();

        // 3. 재료충분도에 따른 material 변화
        if (_isEnoughResource)
            _nowBuildMaterial = _greenMaterial;
        else
            _nowBuildMaterial = _redMaterial;
    }

    #endregion

    #region building 동작 끝 (설치)

    private void F_FinishBuild() 
    {
        // 1. 인벤 내 재료가 충분하면 -> 짓기 
        if (_isEnoughResource == true)
        {
            // 2. 짓기
            F_BuildTemp();

            // 3. 인벤토리 업데이트
            _mybuildCheck.F_UpdateInvenToBuilding();
        }
        else
            return;
    }
    
    private void F_BuildTemp() 
    { 
        if( _TempObjectBuilding != null && _isTempValidPosition == true) 
        { 
            // 0. 생성
            GameObject _nowbuild = Instantiate(F_GetCurBuild(), _TempObjectBuilding.transform.position , _TempObjectBuilding.transform.rotation , _parentTransform);

            // 1. destory
            Destroy( _TempObjectBuilding);
            _TempObjectBuilding = null;

            // 3. model의 material 변경
            F_ChangeMaterial(_nowbuild.transform.GetChild(0) , _oriMaterial);

            // 4. model의 layer (buildFinished로) 변경
            F_ChangeLayer( _nowbuild.transform.GetChild(0) , _buildFinishedLayer , true );

            // 5. 각 오브젝트에 맞는 temp 레이어로 변환
            for (int i = 1; i < _nowbuild.transform.childCount - 1; i++) 
            {
                F_ChangeLayer( _nowbuild.transform.GetChild(i) , _tempUnderBlockLayer[i-1].Item2 );   
            }

            // 6. 현재 새로만든 block Connector 업데이트
            F_ConeectorUpdate(_nowbuild.transform);

            // 8. 현재 새로 만든 block에 MyBuildingBlock 추가
            if (_nowbuild.GetComponent<MyBuildingBlock>() == null)
            {
                // null 이면 추가하기 
                _nowbuild.AddComponent<MyBuildingBlock>();
            }

            // 9. nowBuild와 충돌한 커넥터들 업데이트
            MyBuildingBlock _nowBuildBlock = _nowbuild.GetComponent<MyBuildingBlock>();
            _nowBuildBlock.F_BlockCollisionConnector();

            // 8-1. block에 필드 초기화 
            _nowBuildBlock.F_SetBlockFeild(_buildTypeIdx, _buildDetailIdx % 10, _mybuildCheck._myblock.BlockHp);

        }
    }

    // 커넥터 업데이트 
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

    #region saveManager (초기 9개 floor 생성하기)
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

    // 레이어 변경
    // 부모 레이어 변경 시 v_flag를 true
    private void F_ChangeLayer( Transform v_pa , int v_layer , bool v_flag = false )    // 게임오브젝트의 레이어를 바꿀 때는 int 형으로
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
    
    // 건설 도구 내렸을 때 초기화 함수 
    public void F_InitBuildngMode() 
    {
        // 0. 현재 실행하고 있는 building 코루틴 종료 
        StopAllCoroutines();
    
        // 1. buildingProgressUi 끄기
        HousingUiManager.Instance._buildingProgressUi.gameObject.SetActive(false);

        // 2. 혹시나 켜져있을 housing ui도 끄기 
        //HousingUiManager.Instance._buildingCanvas.gameObject.SetActive(false);

        // 3. preview 오브젝트 끄기
        if (_TempObjectBuilding != null)
            Destroy(_TempObjectBuilding);
        _TempObjectBuilding = null;

    }

    #endregion
}
